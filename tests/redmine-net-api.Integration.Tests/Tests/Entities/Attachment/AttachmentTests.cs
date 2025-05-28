using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Helpers;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Http;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.Attachment;

[Collection(Constants.RedmineTestContainerCollection)]
public class AttachmentTests(RedmineTestContainerFixture fixture)
{
    [Fact]
    public void Attachment_UploadToIssue_Should_Succeed()
    {
        // Arrange
        var upload = FileTestHelper.UploadRandom500KbFile(fixture.RedmineManager);
        Assert.NotNull(upload);
        Assert.NotEmpty(upload.Token);

        var (issue, _) = IssueTestHelper.CreateRandomIssue(fixture.RedmineManager,uploads: [upload]);
        Assert.NotNull(issue);

        // Act
        var retrievedIssue = fixture.RedmineManager.Get<Redmine.Net.Api.Types.Issue>(
            issue.Id.ToString(),
            RequestOptions.Include(RedmineKeys.ATTACHMENTS));
        
        var attachment = retrievedIssue.Attachments.FirstOrDefault();
        Assert.NotNull(attachment);

        var downloadedAttachment = fixture.RedmineManager.Get<Redmine.Net.Api.Types.Attachment>(attachment.Id.ToString());

        // Assert
        Assert.NotNull(downloadedAttachment);
        Assert.Equal(attachment.Id,       downloadedAttachment.Id);
        Assert.Equal(attachment.FileName, downloadedAttachment.FileName);
    }
}