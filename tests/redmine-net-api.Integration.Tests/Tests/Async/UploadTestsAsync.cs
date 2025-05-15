using System.Text;
using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Redmine.Net.Api;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class UploadTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task Upload_Attachment_To_Issue_Should_Succeed()
    {
        var bytes = "Hello World!"u8.ToArray();
        var uploadFile = await fixture.RedmineManager.UploadFileAsync(bytes, "hello-world.txt");

        Assert.NotNull(uploadFile);
        Assert.NotNull(uploadFile.Token);
        
        var issue = await fixture.RedmineManager.CreateAsync(new Issue()
        {
            Project = 1.ToIdentifier(),
            Subject = "Creating an issue with a uploaded file",
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
        });
        
        Assert.NotNull(issue);

        var files = await fixture.RedmineManager.GetAsync<Issue>(issue.Id.ToInvariantString(), RequestOptions.Include(RedmineKeys.ATTACHMENTS));

        Assert.NotNull(files);
        Assert.Single(files.Attachments);
    }
    
    [Fact]
    public async Task Upload_Attachment_To_Wiki_Should_Succeed()
    {
        var bytes = Encoding.UTF8.GetBytes(RandomHelper.GenerateText("Hello Wiki!",10));
        var fileName = $"{RandomHelper.GenerateText("wiki-",5)}.txt";
        var uploadFile = await fixture.RedmineManager.UploadFileAsync(bytes, fileName);

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
        
        var wiki = await fixture.RedmineManager.CreateWikiPageAsync(1.ToInvariantString(), wikiPageName, wikiPageInfo);
        
        Assert.NotNull(wiki);

        var files = await fixture.RedmineManager.GetWikiPageAsync(1.ToInvariantString(), wikiPageName, RequestOptions.Include(RedmineKeys.ATTACHMENTS));

        Assert.NotNull(files);
        Assert.Single(files.Attachments);
    }
}