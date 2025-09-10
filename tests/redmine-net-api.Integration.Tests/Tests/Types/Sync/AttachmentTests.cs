using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Helpers;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Sync;

[Collection(Constants.RedmineTestContainerCollection)]
public class AttachmentTests(RedmineTestContainerFixture fixture)
{
    [Fact]
    public void UploadAndGetAttachment_Should_Succeed()
    {
        // Arrange
        var upload = FileTestHelper.UploadRandom500KbFile(fixture.RedmineManager);
        Assert.NotNull(upload);
        Assert.NotEmpty(upload.Token);

        var issue = IssueTestHelper.CreateIssue(uploads: [upload]);
        var createdIssue = fixture.RedmineManager.Create(issue);
        Assert.NotNull(createdIssue);

        // Act
        var retrievedIssue = fixture.RedmineManager.Get<Issue>(
            createdIssue.Id.ToString(),
            RequestOptions.Include(RedmineKeys.ATTACHMENTS));

        var attachment = retrievedIssue.Attachments.FirstOrDefault();
        Assert.NotNull(attachment);

        var downloadedAttachment = fixture.RedmineManager.Get<Attachment>(attachment.Id.ToString());

        // Assert
        Assert.NotNull(downloadedAttachment);
        Assert.Equal(attachment.Id,       downloadedAttachment.Id);
        Assert.Equal(attachment.FileName, downloadedAttachment.FileName);
    }
}
