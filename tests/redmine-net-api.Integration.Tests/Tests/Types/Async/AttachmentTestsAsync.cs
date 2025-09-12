using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Helpers;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class AttachmentTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task CreateIssueWithAttachment_Should_Succeed()
    {
        // Arrange
        var (upload,_,_) = FileTestHelper.UploadRandom500KbFile(fixture.RedmineManager);
        Assert.NotNull(upload);

        // Act
        var issue = IssueTestHelper.CreateIssue(uploads: [upload]);
        var createdIssue = await fixture.RedmineManager.CreateAsync(issue, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(createdIssue);
        Assert.True(createdIssue.Id > 0);
    }

    [Fact]
    public async Task GetIssueWithAttachments_Should_Succeed()
    {
        // Arrange
        var (upload,_,_) = FileTestHelper.UploadRandom500KbFile(fixture.RedmineManager);
        var issue = IssueTestHelper.CreateIssue(uploads: [upload]);
        var createdIssue = await fixture.RedmineManager.CreateAsync(issue, cancellationToken: TestContext.Current.CancellationToken);

        // Act
        var retrievedIssue = await fixture.RedmineManager.GetAsync<Issue>(createdIssue.Id.ToString(), RequestOptions.Include(RedmineKeys.ATTACHMENTS), TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(retrievedIssue);
        Assert.NotNull(retrievedIssue.Attachments);
        Assert.NotEmpty(retrievedIssue.Attachments);
    }

    [Fact]
    public async Task GetAttachmentById_Should_Succeed()
    {
        // Arrange
        var (upload,_,_) = FileTestHelper.UploadRandom500KbFile(fixture.RedmineManager);
        var issue = IssueTestHelper.CreateIssue(uploads: [upload]);
        var createdIssue = await fixture.RedmineManager.CreateAsync(issue, cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(createdIssue);

        // Act
        var retrievedIssue = await fixture.RedmineManager.GetAsync<Issue>(createdIssue.Id.ToInvariantString(), RequestOptions.Include(RedmineKeys.ATTACHMENTS), TestContext.Current.CancellationToken);

        var attachment = retrievedIssue.Attachments?.FirstOrDefault();
        Assert.NotNull(attachment);

        var downloadedAttachment = await fixture.RedmineManager.GetAsync<Attachment>(attachment.Id.ToInvariantString(), cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(downloadedAttachment);
        Assert.Equal(attachment.Id, downloadedAttachment.Id);
        Assert.Equal(attachment.FileName, downloadedAttachment.FileName);
    }

    [Fact]
    public async Task UploadLargeFile_Should_Succeed()
    {
        // Arrange & Act
        var (upload,_,_) = await FileTestHelper.UploadRandom1MbFileAsync(fixture.RedmineManager);

        // Assert
        Assert.NotNull(upload);
        Assert.NotEmpty(upload.Token);
    }

    [Fact]
    public async Task UploadMultipleFiles_Should_Succeed()
    {
        // Arrange & Act
        var (upload1,_,_) = await FileTestHelper.UploadRandom500KbFileAsync(fixture.RedmineManager);
        Assert.NotNull(upload1);
        Assert.NotEmpty(upload1.Token);

        var (upload2,_,_) = await FileTestHelper.UploadRandom500KbFileAsync(fixture.RedmineManager);
        Assert.NotNull(upload2);
        Assert.NotEmpty(upload2.Token);

        var issue = IssueTestHelper.CreateIssue(uploads: [upload1, upload2]);
        var createdIssue = await fixture.RedmineManager.CreateAsync(issue, cancellationToken: TestContext.Current.CancellationToken);

        var retrievedIssue = await fixture.RedmineManager.GetAsync<Issue>(createdIssue.Id.ToString(), RequestOptions.Include(RedmineKeys.ATTACHMENTS), TestContext.Current.CancellationToken);

        // Assert
        IssueTestHelper.AssertBasic(createdIssue, retrievedIssue);

        Assert.NotNull(retrievedIssue.Attachments);
        Assert.Equal(2, retrievedIssue.Attachments.Count);
    }
}


