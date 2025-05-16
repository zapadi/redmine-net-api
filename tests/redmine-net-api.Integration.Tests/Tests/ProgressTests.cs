using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests;

[Collection(Constants.RedmineTestContainerCollection)]
public class ProgressTests(RedmineTestContainerFixture fixture)
{
    private const string TestDownloadUrl = "attachments/download/123";

    [Fact]
    public void DownloadFile_Sync_ReportsProgress()
    {
        // Arrange
        var progressTracker = new ProgressTracker();

        // Act
        var result = fixture.RedmineManager.DownloadFile(
            TestDownloadUrl,
            progressTracker);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Length > 0, "Downloaded content should not be empty");

        AssertProgressWasReported(progressTracker);
    }

    [Fact]
    public async Task DownloadFileAsync_ReportsProgress()
    {
        // Arrange
        var progressTracker = new ProgressTracker();

        // Act
        var result = await fixture.RedmineManager.DownloadFileAsync(
            TestDownloadUrl,
            null, // No custom request options
            progressTracker,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Length > 0, "Downloaded content should not be empty");

        // Verify progress reporting
        AssertProgressWasReported(progressTracker);
    }

    [Fact]
    public async Task DownloadFileAsync_WithCancellation_StopsDownload()
    {
        // Arrange
        var progressTracker = new ProgressTracker();
        using var cts = new CancellationTokenSource();

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
                TestDownloadUrl,
                null,
                progressTracker,
                cts.Token);
        });

        // Should have received at least one progress report
        Assert.True(progressTracker.ReportCount > 0, "Progress should have been reported at least once");
    }

    private static void AssertProgressWasReported(ProgressTracker tracker)
    {
        Assert.True(tracker.ReportCount > 0, "Progress should have been reported at least once");

        Assert.Contains(100, tracker.ProgressValues);

        for (var i = 0; i < tracker.ProgressValues.Count - 1; i++)
        {
            Assert.True(tracker.ProgressValues[i] <= tracker.ProgressValues[i + 1],
                $"Progress should not decrease: {tracker.ProgressValues[i]} -> {tracker.ProgressValues[i + 1]}");
        }
    }

    private class ProgressTracker : IProgress<int>
    {
        public List<int> ProgressValues { get; } = [];
        public int ReportCount => ProgressValues.Count;

        public event EventHandler<ProgressReportedEventArgs> OnProgressReported;

        public void Report(int value)
        {
            ProgressValues.Add(value);
            OnProgressReported?.Invoke(this, new ProgressReportedEventArgs(value));
        }

        public class ProgressReportedEventArgs(int value) : EventArgs
        {
            public int Value { get; } = value;
        }
    }
}