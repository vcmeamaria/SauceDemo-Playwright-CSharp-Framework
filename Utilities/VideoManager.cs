using Microsoft.Playwright;

namespace SauceDemo.Playwright.CSharp.Utilities;

public static class VideoManager
{
    private const int MaximumVideos = 3;

    private static readonly SemaphoreSlim Sync = new(1, 1);

    public static void Prepare()
    {
        ArtifactPaths.EnsureCreated();

        // Remove raw Playwright-generated videos left over
        // from previous runs.
        foreach (var file in Directory.EnumerateFiles(
                     ArtifactPaths.Videos,
                     "*.webm"))
        {
            var fileName = Path.GetFileName(file);

            if (fileName.StartsWith(
                    "page@",
                    StringComparison.OrdinalIgnoreCase))
            {
                File.Delete(file);
            }
        }

        EnforceRetention();
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

            // Save the completed Playwright video using
            // our own readable filename.
            await video.SaveAsAsync(destinationPath);

            // Remove Playwright's original randomly named copy.
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
        if (!Directory.Exists(ArtifactPaths.Videos))
        {
            return;
        }

        var videos = Directory
            .EnumerateFiles(
                ArtifactPaths.Videos,
                "*.webm")
            .Select(path => new FileInfo(path))
            .OrderByDescending(file =>
                file.LastWriteTimeUtc)
            .ToList();

        foreach (var video in videos.Skip(MaximumVideos))
        {
            video.Delete();
        }
    }
}