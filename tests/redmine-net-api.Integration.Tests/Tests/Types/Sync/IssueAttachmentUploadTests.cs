using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Helpers;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Sync;

[Collection(Constants.RedmineTestContainerCollection)]
public class IssueAttachmentTests(RedmineTestContainerFixture fixture)
{
    [Fact]
    public void UploadAttachmentAndAttachToIssue_Should_Succeed()
    {
        // Arrange – create issue
        var issue = IssueTestHelper.CreateIssue();
        var createdIssue = fixture.RedmineManager.Create(issue);
        Assert.NotNull(createdIssue);

        // Upload a file
        var content   = "Test attachment content"u8.ToArray();
        var fileName  = "test_attachment.txt";
        var upload    = fixture.RedmineManager.UploadFile(content, fileName);
        Assert.NotNull(upload);
        Assert.NotEmpty(upload.Token);

        // Update issue with upload token
        var updateIssue = new Issue
        {
            Subject = $"Test issue for attachment {RandomHelper.GenerateText(5)}",
            Uploads = [upload]
        };
        fixture.RedmineManager.Update(createdIssue.Id.ToString(), updateIssue);

        // Act
        var retrievedIssue = fixture.RedmineManager.Get<Issue>(
            createdIssue.Id.ToString(),
            RequestOptions.Include("attachments"));

        // Assert
        Assert.NotNull(retrievedIssue);
        Assert.NotEmpty(retrievedIssue.Attachments);
        Assert.Contains(retrievedIssue.Attachments, a => a.FileName == fileName);
    }
}