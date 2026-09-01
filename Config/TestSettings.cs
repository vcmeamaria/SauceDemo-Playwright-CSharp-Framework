using System.Text.Json;

namespace SauceDemo.Playwright.CSharp.Config;

public sealed record UserCredential(string Username, string Password);

public sealed class TestSettings
{
    public string BaseUrl { get; init; } = "https://www.saucedemo.com/";
    public string Browser { get; init; } = "chromium";
    public bool Headless { get; init; } = true;
    public int SlowMoMs { get; init; }
    public float DefaultTimeoutMs { get; init; } = 10_000;
    public float NavigationTimeoutMs { get; init; } = 20_000;
    public bool RecordVideo { get; init; }
    public bool TraceOnFailure { get; init; } = true;
    public bool ScreenshotOnFailure { get; init; } = true;
    public int ViewportWidth { get; init; } = 1440;
    public int ViewportHeight { get; init; } = 900;
    public Dictionary<string, UserCredential> Users { get; init; } = new(StringComparer.OrdinalIgnoreCase);

    public static TestSettings Load()
    {
        var candidates = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "appsettings.json"),
            Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json")
        };

        var path = candidates.FirstOrDefault(File.Exists)
                   ?? throw new FileNotFoundException("appsettings.json was not found.");

        var json = File.ReadAllText(path);
        var settings = JsonSerializer.Deserialize<TestSettings>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? throw new InvalidOperationException("Unable to deserialize appsettings.json.");

        return settings with
        {
            BaseUrl = Environment.GetEnvironmentVariable("BASE_URL") ?? settings.BaseUrl,
            Browser = Environment.GetEnvironmentVariable("BROWSER") ?? settings.Browser,
            Headless = bool.TryParse(Environment.GetEnvironmentVariable("HEADLESS"), out var h) ? h : settings.Headless
        };
    }

    public UserCredential User(string name) =>
        Users.TryGetValue(name, out var user)
            ? user
            : throw new KeyNotFoundException($"User profile '{name}' is not configured.");
}
