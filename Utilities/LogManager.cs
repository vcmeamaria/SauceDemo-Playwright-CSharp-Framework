using Serilog;

namespace SauceDemo.Playwright.CSharp.Utilities;

public static class LogManager
{
    private static bool _configured;
    private static readonly object Sync = new();

    public static void Configure()
    {
        lock (Sync)
        {
            if (_configured) return;

            ArtifactPaths.EnsureCreated();
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(
                    Path.Combine(ArtifactPaths.Logs, "playwright-.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 14,
                    shared: true)
                .CreateLogger();

            _configured = true;
        }
    }
}
