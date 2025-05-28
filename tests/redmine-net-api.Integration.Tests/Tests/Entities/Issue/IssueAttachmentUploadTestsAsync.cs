using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Helpers;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Http;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.Issue;

[Collection(Constants.RedmineTestContainerCollection)]
public class IssueAttachmentTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task UploadAttachmentAndAttachToIssue_Should_Succeed()
    {
        // Arrange
        var (issue, _) = await IssueTestHelper.CreateRandomIssueAsync(fixture.RedmineManager);
        
        var fileContent = "Test attachment content"u8.ToArray();
        var filename = "test_attachment.txt";
        var upload = await fixture.RedmineManager.UploadFileAsync(fileContent, filename);
        Assert.NotNull(upload);
        Assert.NotEmpty(upload.Token);
        
        // Prepare issue with attachment
        var updateIssue = new Redmine.Net.Api.Types.Issue
        {
            Subject = $"Test issue for attachment {RandomHelper.GenerateText(5)}",
            Uploads = [upload]
        };

        // Act
        await fixture.RedmineManager.UpdateAsync(issue.Id.ToString(), updateIssue);
        
        var retrievedIssue =
            await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.Issue>(issue.Id.ToString(), RequestOptions.Include(RedmineKeys.ATTACHMENTS));

        // Assert
        Assert.NotNull(retrievedIssue);
        Assert.NotNull(retrievedIssue.Attachments);
        Assert.NotEmpty(retrievedIssue.Attachments);
        Assert.Contains(retrievedIssue.Attachments, a => a.FileName == filename);
    }
}