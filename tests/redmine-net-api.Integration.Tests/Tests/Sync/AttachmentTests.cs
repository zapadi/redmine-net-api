using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Helpers;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Sync;

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
            RequestOptions.Include("attachments"));
        
        var attachment = retrievedIssue.Attachments.FirstOrDefault();
        Assert.NotNull(attachment);

        var downloadedAttachment = fixture.RedmineManager.Get<Attachment>(attachment.Id.ToString());

        // Assert
        Assert.NotNull(downloadedAttachment);
        Assert.Equal(attachment.Id,       downloadedAttachment.Id);
        Assert.Equal(attachment.FileName, downloadedAttachment.FileName);
    }
}