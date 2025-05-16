using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Redmine.Net.Api.Extensions;
using File = Redmine.Net.Api.Types.File;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Async;

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

        var createdFile = await fixture.RedmineManager.CreateAsync<File>(filePayload, PROJECT_ID);
        Assert.Null(createdFile);
        
        var files = await fixture.RedmineManager.GetProjectFilesAsync(PROJECT_ID);
        
        //Assert
        Assert.NotNull(files);
        Assert.NotEmpty(files.Items);
    }
    
    [Fact]
    public async Task CreateFile_Without_Token_Should_Fail()
    {
        await Assert.ThrowsAsync<Exception>(() => fixture.RedmineManager.CreateAsync<File>(
            new File { Filename = "VBpMc.txt" }, PROJECT_ID));
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

        var createdFile = await fixture.RedmineManager.CreateAsync<File>(filePayload, PROJECT_ID);
        Assert.Null(createdFile);
    }
    
    [Fact]
    public async Task CreateFile_With_Version_Should_Succeed()
    {
        var (fileName, token) = await UploadFileAsync();
      
        var filePayload = new File
        {
            Token = token,
            Filename = fileName,
            Description = RandomHelper.GenerateText(9),
            ContentType = "text/plain",
            Version = 1.ToIdentifier(),
        };

        var createdFile = await fixture.RedmineManager.CreateAsync<File>(filePayload, PROJECT_ID);
        Assert.Null(createdFile);
    }

    private async Task<(string,string)> UploadFileAsync()
    {
        var bytes = "Hello World!"u8.ToArray();
        var fileName = $"{RandomHelper.GenerateText(5)}.txt";
        var upload = await fixture.RedmineManager.UploadFileAsync(bytes, fileName);
        
        Assert.NotNull(upload);
        Assert.NotNull(upload.Token);

        return (fileName, upload.Token);
    }
}