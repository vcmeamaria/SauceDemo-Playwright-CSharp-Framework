using Allure.Net.Commons;
using Allure.NUnit;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using SauceDemo.Playwright.CSharp.Config;
using SauceDemo.Playwright.CSharp.Pages;
using SauceDemo.Playwright.CSharp.Utilities;
using Serilog;

namespace SauceDemo.Playwright.CSharp.Tests;

[AllureNUnit]
[Parallelizable(ParallelScope.Fixtures)]
public abstract class BaseTest : PageTest
{
    protected static readonly TestSettings Settings = TestSettings.Load();
    private bool _traceStarted;

    protected LoginPage LoginPage => new(Page);
    protected InventoryPage InventoryPage => new(Page);
    protected ProductDetailsPage ProductDetailsPage => new(Page);
    protected CartPage CartPage => new(Page);
    protected CheckoutInformationPage CheckoutInformationPage => new(Page);
    protected CheckoutOverviewPage CheckoutOverviewPage => new(Page);
    protected CheckoutCompletePage CheckoutCompletePage => new(Page);
    protected MenuComponent Menu => new(Page);

    public override BrowserNewContextOptions ContextOptions()
    {
        ArtifactPaths.EnsureCreated();
        return new BrowserNewContextOptions
        {
            BaseURL = Settings.BaseUrl,
            ViewportSize = new ViewportSize
            {
                Width = Settings.ViewportWidth,
                Height = Settings.ViewportHeight
            },
            IgnoreHTTPSErrors = false,
            RecordVideoDir = Settings.RecordVideo ? ArtifactPaths.Videos : null
        };
    }

    [OneTimeSetUp]
    public void GlobalSetup()
    {
        LogManager.Configure();
    }

    [SetUp]
    public async Task BeforeEach()
    {
        Page.SetDefaultTimeout(Settings.DefaultTimeoutMs);
        Page.SetDefaultNavigationTimeout(Settings.NavigationTimeoutMs);

        Page.Console += (_, msg) => Log.Information("Browser console [{Type}]: {Text}", msg.Type, msg.Text);
        Page.PageError += (_, error) => Log.Error("Browser page error: {Error}", error);

        if (Settings.TraceOnFailure)
        {
            await Context.Tracing.StartAsync(new()
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });
            _traceStarted = true;
        }
    }

    [TearDown]
    public async Task AfterEach()
    {
        var failed = TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed;
        var testName = ArtifactPaths.SafeFileName(TestContext.CurrentContext.Test.Name);
        var stamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");

        try
        {
            if (failed && Settings.ScreenshotOnFailure)
            {
                var screenshot = Path.Combine(ArtifactPaths.Screenshots, $"{testName}-{stamp}.png");
                await Page.ScreenshotAsync(new() { Path = screenshot, FullPage = true });
                TestContext.AddTestAttachment(screenshot, "Failure screenshot");
                AllureApi.AddAttachment("Failure screenshot", "image/png", File.ReadAllBytes(screenshot));
            }

            if (_traceStarted)
            {
                var trace = Path.Combine(ArtifactPaths.Traces, $"{testName}-{stamp}.zip");
                await Context.Tracing.StopAsync(new() { Path = failed ? trace : null });
                if (failed && File.Exists(trace))
                    TestContext.AddTestAttachment(trace, "Playwright trace");
            }
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Unable to collect failure artifacts.");
        }
    }

    protected async Task LoginAsAsync(string profile = "standard")
    {
        var user = Settings.User(profile);
        await LoginPage.OpenAsync(Settings.BaseUrl);
        await LoginPage.LoginAsync(user.Username, user.Password);
        await InventoryPage.AssertLoadedAsync();
    }

    protected async Task AddAndOpenCartAsync(params string[] products)
    {
        foreach (var product in products)
            await InventoryPage.AddProductAsync(product);

        await InventoryPage.OpenCartAsync();
        await CartPage.AssertLoadedAsync();
    }

    protected async Task ReachOverviewAsync(params string[] products)
    {
        await LoginAsAsync();
        await AddAndOpenCartAsync(products);
        await CartPage.CheckoutAsync();
        await CheckoutInformationPage.ContinueAsync(Models.CheckoutCustomer.ValidUkCustomer);
    }
}
