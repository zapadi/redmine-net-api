using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class IssueAttachmentTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task UploadAttachmentAndAttachToIssue_Should_Succeed()
    {
        // Arrange
        var issue = new Issue
        {
            Project = new IdentifiableName { Id = 1 },
            Tracker = new IdentifiableName { Id = 1 },
            Status = new IssueStatus() { Id = 1 },
            Priority = new IdentifiableName { Id = 4 },
            Subject = $"Test issue for attachment {Guid.NewGuid()}",
            Description = "Test issue description"
        };
        
        var createdIssue = await fixture.RedmineManager.CreateAsync(issue);
        Assert.NotNull(createdIssue);
        
        // Upload a file
        var fileContent = "Test attachment content"u8.ToArray();
        var filename = "test_attachment.txt";
        
        var upload = await fixture.RedmineManager.UploadFileAsync(fileContent, filename);
        Assert.NotNull(upload);
        Assert.NotEmpty(upload.Token);
        
        // Prepare issue with attachment
        var updateIssue = new Issue
        {
            Subject = $"Test issue for attachment {ThreadSafeRandom.GenerateText(5)}",
            Uploads = [upload]
        };

        // Act
        await fixture.RedmineManager.UpdateAsync(createdIssue.Id.ToString(), updateIssue);
        
        var retrievedIssue =
            await fixture.RedmineManager.GetAsync<Issue>(createdIssue.Id.ToString(), RequestOptions.Include("attachments"));

        // Assert
        Assert.NotNull(retrievedIssue);
        Assert.NotNull(retrievedIssue.Attachments);
        Assert.NotEmpty(retrievedIssue.Attachments);
        Assert.Contains(retrievedIssue.Attachments, a => a.FileName == filename);
    }
}