using Microsoft.Playwright;
using System.Text.RegularExpressions;

namespace SauceDemo.Playwright.CSharp.Utilities;

public static class VideoManager
{
    private const int MaximumVideos = 3;

    private static readonly SemaphoreSlim Sync =
        new(1, 1);

    private static readonly object PrepareSync =
        new();

    private static bool _prepared;

    private static readonly Regex ManagedVideoPattern =
        new(
            @".+-\d{8}-\d{6}-\d{3}\.webm$",
            RegexOptions.Compiled |
            RegexOptions.IgnoreCase);

    public static void Prepare()
    {
        lock (PrepareSync)
        {
            if (_prepared)
            {
                return;
            }

            ArtifactPaths.EnsureCreated();

            // Clean up raw Playwright files left behind
            // by an interrupted previous run.
            foreach (var file in Directory.EnumerateFiles(
                         ArtifactPaths.Videos,
                         "page@*.webm"))
            {
                try
                {
                    File.Delete(file);
                }
                catch
                {
                    // Ignore stale files that are temporarily locked.
                }
            }

            EnforceRetention();

            _prepared = true;
        }
    }

    public static async Task SaveAsync(
        IVideo video,
        string testName,
        string timestamp)
    {
        await Sync.WaitAsync();

        try
        {
            ArtifactPaths.EnsureCreated();

            var safeTestName =
                ArtifactPaths.SafeFileName(testName);

            var destinationPath = Path.Combine(
                ArtifactPaths.Videos,
                $"{safeTestName}-{timestamp}.webm");

            await video.SaveAsAsync(
                destinationPath);

            await video.DeleteAsync();

            EnforceRetention();
        }
        finally
        {
            Sync.Release();
        }
    }

    private static void EnforceRetention()
    {
        if (!Directory.Exists(
                ArtifactPaths.Videos))
        {
            return;
        }

        // IMPORTANT:
        // Only manage completed videos using our naming
        // convention. Never delete page@... files that
        // another parallel test may still be recording.
        var videos = Directory
            .EnumerateFiles(
                ArtifactPaths.Videos,
                "*.webm")
            .Where(path =>
                ManagedVideoPattern.IsMatch(
                    Path.GetFileName(path)))
            .Select(path =>
                new FileInfo(path))
            .OrderByDescending(file =>
                file.LastWriteTimeUtc)
            .ToList();

        foreach (var video in
                 videos.Skip(MaximumVideos))
        {
            video.Delete();
        }
    }
}