using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Helpers;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class IssueAttachmentTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task UploadAttachmentAndAttachToIssue_Should_Succeed()
    {
        // Upload a file
        var fileContent = "Test attachment content"u8.ToArray();
        var fileName = $"{RandomHelper.GenerateText(5)}.txt";
        var upload = await fixture.RedmineManager.UploadFileAsync(fileContent, fileName, cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(upload);
        Assert.NotEmpty(upload.Token);

        // Arrange
        var issue = new Issue
        {
            Project = new IdentifiableName { Id = 1 },
            Tracker = new IdentifiableName { Id = 1 },
            Status = new IssueStatus() { Id = 1 },
            Priority = new IdentifiableName { Id = 4 },
            Subject = $"Test issue for attachment {Guid.NewGuid()}",
            Description = "Test issue description",
            Uploads = [upload]
        };

        var createdIssue = await fixture.RedmineManager.CreateAsync(issue, cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(createdIssue);

        var retrievedIssue = await fixture.RedmineManager.GetAsync<Issue>(createdIssue.Id.ToString(), RequestOptions.Include(RedmineKeys.ATTACHMENTS), TestContext.Current.CancellationToken);

         fileName = $"{RandomHelper.GenerateText(5)}.txt";
         upload = await fixture.RedmineManager.UploadFileAsync(fileContent, fileName, cancellationToken: TestContext.Current.CancellationToken);

        retrievedIssue.Subject = $"Test issue for attachment {RandomHelper.GenerateText(5)}";
        retrievedIssue.Uploads = [upload];


        // Act
        await fixture.RedmineManager.UpdateAsync(createdIssue.Id.ToString(), retrievedIssue, cancellationToken: TestContext.Current.CancellationToken);

         retrievedIssue =
            await fixture.RedmineManager.GetAsync<Issue>(createdIssue.Id.ToString(), RequestOptions.Include(RedmineKeys.ATTACHMENTS), TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(retrievedIssue);
        Assert.NotNull(retrievedIssue.Attachments);
        Assert.NotEmpty(retrievedIssue.Attachments);
        Assert.Contains(retrievedIssue.Attachments, a => a.FileName == fileName);
    }
}
