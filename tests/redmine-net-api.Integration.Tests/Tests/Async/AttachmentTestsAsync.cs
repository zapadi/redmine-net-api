using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Helpers;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class AttachmentTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task CreateIssueWithAttachment_Should_Succeed()
    {
        // Arrange
        var upload = FileTestHelper.UploadRandom500KbFile(fixture.RedmineManager);
        Assert.NotNull(upload);
        
        // Act
        var issue = IssueTestHelper.CreateIssue(uploads: [upload]);
        var createdIssue = await fixture.RedmineManager.CreateAsync(issue);
        
        // Assert
        Assert.NotNull(createdIssue);
        Assert.True(createdIssue.Id > 0);
    }
    
    [Fact]
    public async Task GetIssueWithAttachments_Should_Succeed()
    {
        // Arrange
        var upload = FileTestHelper.UploadRandom500KbFile(fixture.RedmineManager);
        var issue = IssueTestHelper.CreateIssue(uploads: [upload]);
        var createdIssue = await fixture.RedmineManager.CreateAsync(issue);
        
        // Act
        var retrievedIssue = await fixture.RedmineManager.GetAsync<Issue>(
            createdIssue.Id.ToString(), 
            RequestOptions.Include("attachments"));
        
        // Assert
        Assert.NotNull(retrievedIssue);
        Assert.NotNull(retrievedIssue.Attachments);
        Assert.NotEmpty(retrievedIssue.Attachments);
    }
    
    [Fact]
    public async Task GetAttachmentById_Should_Succeed()
    {
        // Arrange
        var upload = FileTestHelper.UploadRandom500KbFile(fixture.RedmineManager);
        var issue = IssueTestHelper.CreateIssue(uploads: [upload]);
        var createdIssue = await fixture.RedmineManager.CreateAsync(issue);
        
        var retrievedIssue = await fixture.RedmineManager.GetAsync<Issue>(
            createdIssue.Id.ToString(), 
            RequestOptions.Include("attachments"));
        
        var attachment = retrievedIssue.Attachments.FirstOrDefault();
        Assert.NotNull(attachment);
        
        // Act
        var downloadedAttachment = await fixture.RedmineManager.GetAsync<Attachment>(attachment.Id.ToString());

        // Assert
        Assert.NotNull(downloadedAttachment);
        Assert.Equal(attachment.Id, downloadedAttachment.Id);
        Assert.Equal(attachment.FileName, downloadedAttachment.FileName);
    }

    [Fact]
    public async Task UploadLargeFile_Should_Succeed()
    {
        // Arrange & Act
        var upload = await FileTestHelper.UploadRandom1MbFileAsync(fixture.RedmineManager);
        
        // Assert
        Assert.NotNull(upload);
        Assert.NotEmpty(upload.Token);
    }
    
    [Fact]
    public async Task UploadMultipleFiles_Should_Succeed()
    {
        // Arrange & Act
        var upload1 = await FileTestHelper.UploadRandom500KbFileAsync(fixture.RedmineManager);
        Assert.NotNull(upload1);
        Assert.NotEmpty(upload1.Token);
        
        var upload2 = await FileTestHelper.UploadRandom500KbFileAsync(fixture.RedmineManager);
        Assert.NotNull(upload2);
        Assert.NotEmpty(upload2.Token);

        // Assert
        var issue = IssueTestHelper.CreateIssue(uploads: [upload1, upload2]);
        var createdIssue = await fixture.RedmineManager.CreateAsync(issue);
        
        var retrievedIssue = await fixture.RedmineManager.GetAsync<Issue>(
            createdIssue.Id.ToString(), 
            RequestOptions.Include("attachments"));
        
        Assert.Equal(2, retrievedIssue.Attachments.Count);
    }
}