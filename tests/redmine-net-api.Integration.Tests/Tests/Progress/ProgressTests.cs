using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Helpers;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Padi.RedmineAPI.Integration.Tests.Tests.Common;
using Redmine.Net.Api.Extensions;
using Xunit;
using File =  Redmine.Net.Api.Types.File;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Progress;

[Collection(Constants.RedmineTestContainerCollection)]
public partial class ProgressTests(RedmineTestContainerFixture fixture)
{
    private const string DOWNLOAD_URL_FORMAT = "attachments/download/{0}/{1}";

    [Fact]
    public void DownloadFile_Sync_ReportsProgress()
    {
        // Arrange
        var (upload, fileName,_) =   FileTestHelper.UploadRandom1MbFile(fixture.RedmineManager);
        var filePayload = new File
        {
            Token = upload.Token,
            Filename = fileName,
        };

        _ = fixture.RedmineManager.Create(filePayload, TestConstants.Project.DefaultIdentifier);
        
        var files = fixture.RedmineManager.GetProjectFiles(TestConstants.Project.DefaultIdentifier);

        Assert.NotEmpty(files.Items);
        
        var progressTracker = new ProgressTracker();

        // Act
        var file = files.Items.ToList().First();

        var address = string.Format(DOWNLOAD_URL_FORMAT, file.Id, file.Filename);
        var result = fixture.RedmineManager.DownloadFile(address, progress: progressTracker);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Length > 0, "Downloaded content should not be empty");

        AssertProgressWasReported(progressTracker);
    }

    private static void AssertProgressWasReported(ProgressTracker tracker)
    {
        Assert.True(tracker.ReportCount > 0, "Progress should have been reported at least once");

        Assert.Contains(100, tracker.ProgressValues);
    }

    private sealed class ProgressTracker : IProgress<int>
    {
        public List<int> ProgressValues { get; } = [];
        public int ReportCount => ProgressValues.Count;

        public event EventHandler<ProgressReportedEventArgs>? OnProgressReported;

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