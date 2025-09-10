using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Helpers;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Types;
using Xunit;
using File = Redmine.Net.Api.Types.File;
using Version = Redmine.Net.Api.Types.Version;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class FileTestsAsync(RedmineTestContainerFixture fixture)
{
    private const string PROJECT_ID = "1";

    [Fact]
    public async Task CreateFile_Should_Succeed()
    {
        var (_, token) = await UploadFileAsync();

        var filePayload = new File
        {
            Token = token,
        };

        var createdFile = await fixture.RedmineManager.CreateAsync(filePayload, PROJECT_ID, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Null(createdFile);

        var files = await fixture.RedmineManager.GetProjectFilesAsync(PROJECT_ID, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        Assert.NotNull(files);
        Assert.NotEmpty(files.Items);
    }

    [Fact]
    public async Task CreateFile_Without_Token_Should_Fail()
    {
        var ex =await Assert.ThrowsAsync<RedmineApiException>(() => fixture.RedmineManager.CreateAsync(new File { Filename = "VBpMc.txt" }, PROJECT_ID, cancellationToken: TestContext.Current.CancellationToken));
        
        Assert.NotNull(ex);
        Assert.Equal(HttpConstants.StatusCodes.NotFound, ex.HttpStatusCode);
    }

    [Fact]
    public async Task CreateFile_With_OptionalParameters_Should_Succeed()
    {
        var (fileName, token) = await UploadFileAsync();

        var filePayload = new File
        {
            Token = token,
            Filename = fileName,
            Description = RandomHelper.GenerateText(9),
            ContentType = "text/plain",
        };

        var createdFile = await fixture.RedmineManager.CreateAsync(filePayload, PROJECT_ID, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Null(createdFile);
    }

    [Fact]
    public async Task CreateFile_With_Version_Should_Succeed()
    {
        var (initialFileName, initialToken) = await UploadFileAsync();

        var initialFilePayload = new File
        {
            Token = initialToken,
            Filename = initialFileName,
            Description = RandomHelper.GenerateText(9),
            ContentType = "text/plain",
        };

        _ = await fixture.RedmineManager.CreateAsync(initialFilePayload, PROJECT_ID, cancellationToken: TestContext.Current.CancellationToken);

        var (_, token) = await UploadFileAsync(initialFileName);
        
        initialFilePayload.Token = token;
        initialFilePayload.Version = new IdentifiableName(2, null);
        
        var versionFile = await fixture.RedmineManager.CreateAsync(initialFilePayload, PROJECT_ID, cancellationToken: TestContext.Current.CancellationToken);
        
        Assert.Null(versionFile);
    }

    private async Task<(string,string)> UploadFileAsync(string? filename = null)
    {
        var bytes = "Hello World!"u8.ToArray();
        var fileName = filename ?? $"{RandomHelper.GenerateText(5)}.txt";
        var upload = await fixture.RedmineManager.UploadFileAsync(bytes, fileName);

        Assert.NotNull(upload);
        Assert.NotNull(upload.Token);

        return (fileName, upload.Token);
    }
}

