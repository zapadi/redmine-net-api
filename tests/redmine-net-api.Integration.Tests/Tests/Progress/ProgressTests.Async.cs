using Padi.RedmineAPI.Integration.Tests.Helpers;
using Padi.RedmineAPI.Integration.Tests.Tests.Common;
using Redmine.Net.Api.Extensions;
using Xunit;
using File =  Redmine.Net.Api.Types.File;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Progress;

public partial class ProgressTests
{
    [Fact]
    public async Task DownloadFileAsync_ReportsProgress()
    {
        // Arrange
        var (upload, fileName,_) =  await FileTestHelper.UploadRandom1MbFileAsync(fixture.RedmineManager);
        var filePayload = new File
        {
            Token = upload.Token,
            Filename = fileName,
        };

        _ = await fixture.RedmineManager.CreateAsync(filePayload, TestConstants.Project.DefaultIdentifier,cancellationToken: TestContext.Current.CancellationToken);
        
        var files = await fixture.RedmineManager.GetProjectFilesAsync(TestConstants.Project.DefaultIdentifier, cancellationToken: TestContext.Current.CancellationToken);
        
        var progressTracker = new ProgressTracker();
        Assert.NotEmpty(files.Items);
        
        // Act
        var file = files.Items.ToList().First();
        
        var result = await fixture.RedmineManager.DownloadFileAsync(
            string.Format(DOWNLOAD_URL_FORMAT,file.Id, file.Filename),
            null,
            progressTracker,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Length > 0, "Downloaded content should not be empty");

        AssertProgressWasReported(progressTracker);
    }
}