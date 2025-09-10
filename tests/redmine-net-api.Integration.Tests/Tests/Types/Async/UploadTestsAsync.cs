using System.Text;
using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Helpers;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class UploadTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task Upload_Attachment_To_Issue_Should_Succeed()
    {
        var bytes = "Hello World!"u8.ToArray();
        var uploadFile = await fixture.RedmineManager.UploadFileAsync(bytes, "hello-world.txt", cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(uploadFile);
        Assert.NotNull(uploadFile.Token);

        var issue = await fixture.RedmineManager.CreateAsync(new Issue()
        {
            Project = 1.ToIdentifier(),
            Subject = "Creating an issue with an uploaded file",
            Tracker = 1.ToIdentifier(),
            Status = 1.ToIssueStatusIdentifier(),
            Uploads = [
                new Upload()
                {
                    Token = uploadFile.Token,
                    ContentType = "text/plain",
                    Description = "An optional description here",
                    FileName = "hello-world.txt"
                }
            ]
        }, cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(issue);

        var files = await fixture.RedmineManager.GetAsync<Issue>(issue.Id.ToInvariantString(), RequestOptions.Include(RedmineKeys.ATTACHMENTS), TestContext.Current.CancellationToken);

        Assert.NotNull(files);
        Assert.NotNull(files.Attachments);
        Assert.Single(files.Attachments);
    }

    [Fact]
    public async Task Upload_Attachment_To_Wiki_Should_Succeed()
    {
        var bytes = Encoding.UTF8.GetBytes(RandomHelper.GenerateText("Hello Wiki!",10));
        var fileName = $"{RandomHelper.GenerateText("wiki-",5)}.txt";
        var uploadFile = await fixture.RedmineManager.UploadFileAsync(bytes, fileName, cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(uploadFile);
        Assert.NotNull(uploadFile.Token);

        var wikiPageName = RandomHelper.GenerateText(7);

        var wikiPageInfo = new WikiPage()
        {
            Version = 0,
            Comments = RandomHelper.GenerateText(15),
            Text = RandomHelper.GenerateText(10),
            Uploads =
            [
                new Upload()
                {
                    Token = uploadFile.Token,
                    ContentType = "text/plain",
                    Description = RandomHelper.GenerateText(15),
                    FileName = fileName,
                }
            ]
        };

        var wiki = await fixture.RedmineManager.CreateWikiPageAsync(1.ToInvariantString(), wikiPageName, wikiPageInfo, cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(wiki);

        var files = await fixture.RedmineManager.GetWikiPageAsync(1.ToInvariantString(), wikiPageName, RequestOptions.Include(RedmineKeys.ATTACHMENTS), cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(files);
        Assert.NotNull(files.Attachments);
        Assert.Single(files.Attachments);
    }
}
