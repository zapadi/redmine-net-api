namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests;

public partial class ProgressTests
{
    [Fact]
    public async Task DownloadFileAsync_ReportsProgress()
    {
        // Arrange
        var progressTracker = new ProgressTracker();

        // Act
        var result = await fixture.RedmineManager.DownloadFileAsync(
            TEST_DOWNLOAD_URL,
            null,
            progressTracker,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Length > 0, "Downloaded content should not be empty");

        AssertProgressWasReported(progressTracker);
    }

    [Fact]
    public async Task DownloadFileAsync_WithCancellation_StopsDownload()
    {
        // Arrange
        var progressTracker = new ProgressTracker();
        var cts = new CancellationTokenSource();

        try
        {
            progressTracker.OnProgressReported += (sender, args) =>
            {
                if (args.Value > 0 && !cts.IsCancellationRequested)
                {
                    cts.Cancel();
                }
            };

            // Act & Assert
            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            {
                await fixture.RedmineManager.DownloadFileAsync(
                    TEST_DOWNLOAD_URL,
                    null,
                    progressTracker,
                    cts.Token);
            });

            Assert.True(progressTracker.ReportCount > 0, "Progress should have been reported at least once");
        }
        finally
        {
            cts.Dispose();
        }
    }
}