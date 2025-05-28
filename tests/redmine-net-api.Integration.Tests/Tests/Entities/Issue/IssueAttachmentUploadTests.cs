using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Helpers;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Http;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.Issue;

[Collection(Constants.RedmineTestContainerCollection)]
public class IssueAttachmentTests(RedmineTestContainerFixture fixture)
{
    [Fact]
    public void UploadAttachmentAndAttachToIssue_Should_Succeed()
    {
        // Arrange
        var (issue, _)  = IssueTestHelper.CreateRandomIssue(fixture.RedmineManager);
        
        var content   = "Test attachment content"u8.ToArray();
        var fileName  = "test_attachment.txt";
        var upload    = fixture.RedmineManager.UploadFile(content, fileName);
        Assert.NotNull(upload);
        Assert.NotEmpty(upload.Token);

        // Act
       var updateIssue = new Redmine.Net.Api.Types.Issue
        {
            Subject = $"Test issue for attachment {RandomHelper.GenerateText(5)}",
            Uploads = [upload]
        };
        fixture.RedmineManager.Update(issue.Id.ToString(), updateIssue);

        
        var retrievedIssue = fixture.RedmineManager.Get<Redmine.Net.Api.Types.Issue>(
            issue.Id.ToString(),
            RequestOptions.Include(RedmineKeys.ATTACHMENTS));

        // Assert
        Assert.NotNull(retrievedIssue);
        Assert.NotEmpty(retrievedIssue.Attachments);
        Assert.Contains(retrievedIssue.Attachments, a => a.FileName == fileName);
    }
}