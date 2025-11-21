using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Helpers;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api.Extensions;
using Xunit;
using File = Redmine.Net.Api.Types.File;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Sync;

[Collection(Constants.RedmineTestContainerCollection)]
public class FileTests(RedmineTestContainerFixture fixture)
{
    private const string PROJECT_ID = "1";

    [Fact]
    public void CreateFile_Should_Succeed()
    {
        var (_, token) = UploadFile();

        var filePayload = new File { Token = token };

        var createdFile = fixture.RedmineManager.Create<File>(filePayload, PROJECT_ID);
        Assert.Null(createdFile); // the API returns null on success when no extra fields were provided

        var files = fixture.RedmineManager.GetProjectFiles(PROJECT_ID);

        // Assert
        Assert.NotNull(files);
        Assert.NotEmpty(files.Items);
    }

    [Fact]
    public void CreateFile_Without_Token_Should_Fail()
    {
        Assert.ThrowsAny<Exception>(() =>
            fixture.RedmineManager.Create(new File { Filename = "project_file.zip" }, PROJECT_ID));
    }

    [Fact]
    public void CreateFile_With_OptionalParameters_Should_Succeed()
    {
        var (fileName, token) = UploadFile();

        var filePayload = new File
        {
            Token = token,
            Filename = fileName,
            Description = RandomHelper.GenerateText(9),
            ContentType = "text/plain",
        };

        var createdFile = fixture.RedmineManager.Create<File>(filePayload, PROJECT_ID);
        Assert.Null(createdFile);
    }

    [Fact]
    public void CreateFile_With_Version_Should_Succeed()
    {
        var (fileName, initialToken) = UploadFile();

        var filePayload = new File
        {
            Token = initialToken,
            Filename = fileName,
            Description = RandomHelper.GenerateText(9),
            ContentType = "text/plain",
            Version = 1.ToIdentifier(),
        };

        _ = fixture.RedmineManager.Create<File>(filePayload, PROJECT_ID);
        var (_, token) =  UploadFile(fileName);
        
        filePayload.Token = token;
        filePayload.Version = 2.ToIdentifier();
        
        var versionFile =  fixture.RedmineManager.Create(filePayload, PROJECT_ID);
        
        Assert.Null(versionFile);
    }

    private (string fileName, string token) UploadFile(string? filename = null)
    {
        var bytes = "Hello World!"u8.ToArray();
        var fileName = $"{RandomHelper.GenerateText(5)}.txt";
        var upload = fixture.RedmineManager.UploadFile(bytes, fileName);

        Assert.NotNull(upload);
        Assert.NotNull(upload.Token);

        return (fileName, upload.Token);
    }
}