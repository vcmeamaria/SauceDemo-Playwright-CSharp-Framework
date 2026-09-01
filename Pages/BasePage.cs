using Microsoft.Playwright;
using Serilog;

namespace SauceDemo.Playwright.CSharp.Pages;

public abstract class BasePage
{
    protected BasePage(IPage page)
    {
        Page = page;
    }

    protected IPage Page { get; }

    protected async Task ClickAsync(ILocator locator, string description)
    {
        Log.Information("Click: {Description}", description);
        await locator.ClickAsync();
    }

    protected async Task FillAsync(ILocator locator, string value, string description, bool sensitive = false)
    {
        Log.Information("Fill: {Description} = {Value}", description, sensitive ? "***" : value);
        await locator.FillAsync(value);
    }

    public Task<string> TitleAsync() => Page.TitleAsync();
}
