namespace SauceDemo.Playwright.CSharp.Utilities;

public static class ArtifactPaths
{
    public static readonly string Root = Path.Combine(Directory.GetCurrentDirectory(), "artifacts");
    public static readonly string Screenshots = Path.Combine(Root, "screenshots");
    public static readonly string Traces = Path.Combine(Root, "traces");
    public static readonly string Videos = Path.Combine(Root, "videos");
    public static readonly string Logs = Path.Combine(Root, "logs");

    public static void EnsureCreated()
    {
        Directory.CreateDirectory(Root);
        Directory.CreateDirectory(Screenshots);
        Directory.CreateDirectory(Traces);
        Directory.CreateDirectory(Videos);
        Directory.CreateDirectory(Logs);
    }

    public static string SafeFileName(string value)
    {
        var invalid = Path.GetInvalidFileNameChars();
        return string.Concat(value.Select(c => invalid.Contains(c) ? '_' : c));
    }
}
