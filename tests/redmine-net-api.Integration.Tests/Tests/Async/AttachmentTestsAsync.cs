using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class AttachmentTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task UploadAndGetAttachment_Should_Succeed()
    {
        // Arrange
        var fileContent = "Test attachment content"u8.ToArray();
        var filename = "test_attachment.txt";
        
        // Upload the file
        var upload = await fixture.RedmineManager.UploadFileAsync(fileContent, filename);
        Assert.NotNull(upload);
        Assert.NotEmpty(upload.Token);
        
        // Create an issue with the attachment
        var issue = new Issue
        {
            Project = new IdentifiableName { Id = 1 },
            Tracker = new IdentifiableName { Id = 1 },
            Status = new IssueStatus { Id = 1 },
            Priority = new IdentifiableName { Id = 4 },
            Subject = $"Test issue with attachment {Guid.NewGuid()}",
            Description = "Test issue description",
            Uploads = [upload]
        };
        
        var createdIssue = await fixture.RedmineManager.CreateAsync(issue);
        Assert.NotNull(createdIssue);
        
        // Get the issue with attachments
        var retrievedIssue = await fixture.RedmineManager.GetAsync<Issue>(createdIssue.Id.ToString(), RequestOptions.Include("attachments"));
        
        // Act
        var attachment = retrievedIssue.Attachments.FirstOrDefault();
        Assert.NotNull(attachment);
        
        var downloadedAttachment = await fixture.RedmineManager.GetAsync<Attachment>(attachment.Id.ToString());

        // Assert
        Assert.NotNull(downloadedAttachment);
        Assert.Equal(attachment.Id, downloadedAttachment.Id);
        Assert.Equal(attachment.FileName, downloadedAttachment.FileName);
    }
}