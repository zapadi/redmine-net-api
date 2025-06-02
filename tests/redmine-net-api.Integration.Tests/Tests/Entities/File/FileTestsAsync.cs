using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Helpers;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Common;
using Redmine.Net.Api;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Http;
using Redmine.Net.Api.Http.Extensions;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.File;

[Collection(Constants.RedmineTestContainerCollection)]
public class FileTestsAsync(RedmineTestContainerFixture fixture)
{
    private const string PROJECT_ID = TestConstants.Projects.DefaultProjectIdentifier;

    [Fact]
    public async Task CreateFile_Should_Succeed()
    {
        var (_, token) = await UploadFileAsync();
        
        var filePayload = new Redmine.Net.Api.Types.File
        {
            Token = token,
        };

        var createdFile = await fixture.RedmineManager.CreateAsync<Redmine.Net.Api.Types.File>(filePayload, PROJECT_ID);
        Assert.Null(createdFile);
        
        var files = await fixture.RedmineManager.GetProjectFilesAsync(PROJECT_ID, 
            new RequestOptions(){ QueryString = RedmineKeys.LIMIT.WithInt(1)});
        
        //Assert
        Assert.NotNull(files);
        Assert.NotEmpty(files.Items);
    }
    
    [Fact]
    public async Task CreateFile_Without_Token_Should_Fail()
    {
        await Assert.ThrowsAsync<RedmineNotFoundException>(() => fixture.RedmineManager.CreateAsync<Redmine.Net.Api.Types.File>(
            new Redmine.Net.Api.Types.File { Filename = "VBpMc.txt" }, PROJECT_ID));
    }
    
    [Fact]
    public async Task CreateFile_With_OptionalParameters_Should_Succeed()
    {
        // Arrange
        var (fileName, token) = await UploadFileAsync();
      
        var filePayload = new Redmine.Net.Api.Types.File
        {
            Token = token,
            Filename = fileName,
            Description = RandomHelper.GenerateText(9),
            ContentType = "text/plain",
        };

        // Act
        _ = await fixture.RedmineManager.CreateAsync<Redmine.Net.Api.Types.File>(filePayload, PROJECT_ID);
        var files = await fixture.RedmineManager.GetProjectFilesAsync(PROJECT_ID);
        var file = files.Items.FirstOrDefault(x => x.Filename == fileName);
        
        Assert.NotNull(file);
        Assert.True(file.Id > 0);
        Assert.NotEmpty(file.Digest);
        Assert.Equal(filePayload.Description, file.Description);
        Assert.Equal(filePayload.ContentType, file.ContentType);
        Assert.EndsWith($"/attachments/download/{file.Id}/{fileName}", file.ContentUrl);
        Assert.Equal(filePayload.Version, file.Version);
    }
    
    [Fact]
    public async Task CreateFile_With_Version_Should_Succeed()
    {
        // Arrange
        var versionPayload = TestEntityFactory.CreateRandomVersionPayload();
        var version = await fixture.RedmineManager.CreateAsync(versionPayload, TestConstants.Projects.DefaultProjectIdentifier);
        Assert.NotNull(version);
        
        var (fileName, token) = await UploadFileAsync();
      
        var filePayload = new Redmine.Net.Api.Types.File
        {
            Token = token,
            Filename = fileName,
            Description = RandomHelper.GenerateText(9),
            ContentType = "text/plain",
            Version = version
        };

        // Act
        _ = await fixture.RedmineManager.CreateAsync<Redmine.Net.Api.Types.File>(filePayload, PROJECT_ID);
        var files = await fixture.RedmineManager.GetProjectFilesAsync(PROJECT_ID);
        var file = files.Items.FirstOrDefault(x => x.Filename == fileName);

        // Assert
        Assert.NotNull(file);
        Assert.True(file.Id > 0);
        Assert.NotEmpty(file.Digest);
        Assert.Equal(filePayload.Description, file.Description);
        Assert.Equal(filePayload.ContentType, file.ContentType);
        Assert.EndsWith($"/attachments/download/{file.Id}/{fileName}", file.ContentUrl);
        Assert.Equal(filePayload.Version.Id, file.Version.Id);
    }

    [Fact]
    public async Task File_UploadLargeFile_Should_Succeed()
    {
        // Arrange & Act
        var upload = await FileTestHelper.UploadRandom1MbFileAsync(fixture.RedmineManager);
        
        // Assert
        Assert.NotNull(upload);
        Assert.NotEmpty(upload.Token);
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