using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Helpers;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Http;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.Attachment;

[Collection(Constants.RedmineTestContainerCollection)]
public class AttachmentTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task Attachment_GetIssueWithAttachments_Should_Succeed()
    {
        // Arrange
        var upload = await FileTestHelper.UploadRandom500KbFileAsync(fixture.RedmineManager);
        var (issue, _) = await IssueTestHelper.CreateRandomIssueAsync(fixture.RedmineManager, uploads: [upload]);
        
        // Act
        var retrievedIssue = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.Issue>(
            issue.Id.ToString(), 
            RequestOptions.Include(RedmineKeys.ATTACHMENTS));
        
        // Assert
        Assert.NotNull(retrievedIssue);
        Assert.NotNull(retrievedIssue.Attachments);
        Assert.NotEmpty(retrievedIssue.Attachments);
    }
    
    [Fact]
    public async Task Attachment_GetByIssueId_Should_Succeed()
    {
        // Arrange
        var upload = await FileTestHelper.UploadRandom500KbFileAsync(fixture.RedmineManager);
        var (issue, _) = await IssueTestHelper.CreateRandomIssueAsync(fixture.RedmineManager, uploads: [upload]);
        
        var retrievedIssue = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.Issue>(
            issue.Id.ToString(), 
            RequestOptions.Include(RedmineKeys.ATTACHMENTS));
        
        var attachment = retrievedIssue.Attachments.FirstOrDefault();
        Assert.NotNull(attachment);
        
        // Act
        var downloadedAttachment = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.Attachment>(attachment.Id.ToString());

        // Assert
        Assert.NotNull(downloadedAttachment);
        Assert.Equal(attachment.Id, downloadedAttachment.Id);
        Assert.Equal(attachment.FileName, downloadedAttachment.FileName);
    }

    [Fact]
    public async Task Attachment_Upload_MultipleFiles_Should_Succeed()
    {
        // Arrange & Act
        var upload1 = await FileTestHelper.UploadRandom500KbFileAsync(fixture.RedmineManager);
        Assert.NotNull(upload1);
        Assert.NotEmpty(upload1.Token);
        
        var upload2 = await FileTestHelper.UploadRandom500KbFileAsync(fixture.RedmineManager);
        Assert.NotNull(upload2);
        Assert.NotEmpty(upload2.Token);

        // Assert
        var (issue, _) = await IssueTestHelper.CreateRandomIssueAsync(fixture.RedmineManager, uploads: [upload1, upload2]);
        
        var retrievedIssue = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.Issue>(
            issue.Id.ToString(), 
            RequestOptions.Include(RedmineKeys.ATTACHMENTS));
        
        Assert.Equal(2, retrievedIssue.Attachments.Count);
    }
}