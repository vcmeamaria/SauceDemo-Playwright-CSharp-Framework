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
            if (_configured)
                return;

            ArtifactPaths.EnsureCreated();

            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");

            var logPath = Path.Combine(
                ArtifactPaths.Logs,
                $"playwright-{timestamp}.log");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(
                    logPath,
                    shared: true)
                .CreateLogger();

            _configured = true;
        }
    }
}