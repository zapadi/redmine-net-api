using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests;

[Collection(Constants.RedmineTestContainerCollection)]
public partial class ProgressTests(RedmineTestContainerFixture fixture)
{
    private const string TEST_DOWNLOAD_URL = "attachments/download/86/Manual_de_control_fiscal_versiune%20finala_RO_24_07_2023.pdf";

    [Fact]
    public void DownloadFile_Sync_ReportsProgress()
    {
        // Arrange
        var progressTracker = new ProgressTracker();

        // Act
        var result = fixture.RedmineManager.DownloadFile(TEST_DOWNLOAD_URL, progressTracker);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Length > 0, "Downloaded content should not be empty");

        AssertProgressWasReported(progressTracker);
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

    private sealed class ProgressTracker : IProgress<int>
    {
        public List<int> ProgressValues { get; } = [];
        public int ReportCount => ProgressValues.Count;

        public event EventHandler<ProgressReportedEventArgs> OnProgressReported;

        public void Report(int value)
        {
            ProgressValues.Add(value);
            OnProgressReported?.Invoke(this, new ProgressReportedEventArgs(value));
        }

        public sealed class ProgressReportedEventArgs(int value) : EventArgs
        {
            public int Value { get; } = value;
        }
    }
}