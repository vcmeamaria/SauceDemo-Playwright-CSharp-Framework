namespace SauceDemo.Playwright.CSharp.Utilities;

public static class ArtifactPaths
{
    private static readonly string ProjectRoot = FindProjectRoot();

    public static readonly string Root =
        Path.Combine(ProjectRoot, "artifacts");

    public static readonly string Screenshots =
        Path.Combine(Root, "screenshots");

    public static readonly string Traces =
        Path.Combine(Root, "traces");

    public static readonly string Videos =
        Path.Combine(Root, "videos");

    public static readonly string Logs =
        Path.Combine(Root, "logs");

    public static readonly string TestResults =
        Path.Combine(Root, "test-results");

    public static void EnsureCreated()
    {
        Directory.CreateDirectory(Root);
        Directory.CreateDirectory(Screenshots);
        Directory.CreateDirectory(Traces);
        Directory.CreateDirectory(Videos);
        Directory.CreateDirectory(Logs);
        Directory.CreateDirectory(TestResults);
    }

    public static string SafeFileName(string value)
    {
        var invalid = Path.GetInvalidFileNameChars();

        return string.Concat(
            value.Select(c => invalid.Contains(c) ? '_' : c));
    }

    private static string FindProjectRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            var projectFile = Path.Combine(
                directory.FullName,
                "SauceDemo.Playwright.CSharp.csproj");

            if (File.Exists(projectFile))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Unable to locate the SauceDemo.Playwright.CSharp project root.");
    }
}