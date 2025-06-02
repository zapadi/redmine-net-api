using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Helpers;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Common;
using Redmine.Net.Api.Extensions;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.File;

[Collection(Constants.RedmineTestContainerCollection)]
public class FileTests(RedmineTestContainerFixture fixture)
{
    [Fact]
    public void CreateFile_Should_Succeed()
    {
        var (_, token) = UploadFile();

        var filePayload = new Redmine.Net.Api.Types.File { Token = token };

        var createdFile = fixture.RedmineManager.Create<Redmine.Net.Api.Types.File>(filePayload, TestConstants.Projects.DefaultProjectIdentifier);
        Assert.Null(createdFile); // the API returns null on success when no extra fields were provided

        var files = fixture.RedmineManager.GetProjectFiles(TestConstants.Projects.DefaultProjectIdentifier);

        // Assert
        Assert.NotNull(files);
        Assert.NotEmpty(files.Items);
    }

    [Fact]
    public void CreateFile_Without_Token_Should_Fail()
    {
        Assert.ThrowsAny<Exception>(() =>
            fixture.RedmineManager.Create(new Redmine.Net.Api.Types.File { Filename = "project_file.zip" }, TestConstants.Projects.DefaultProjectIdentifier));
    }

    [Fact]
    public void CreateFile_With_OptionalParameters_Should_Succeed()
    {
        var (fileName, token) = UploadFile();

        var filePayload = new Redmine.Net.Api.Types.File
        {
            Token = token,
            Filename = fileName,
            Description = RandomHelper.GenerateText(9),
            ContentType = "text/plain",
        };

        _ = fixture.RedmineManager.Create<Redmine.Net.Api.Types.File>(filePayload, TestConstants.Projects.DefaultProjectIdentifier);

        var files = fixture.RedmineManager.GetProjectFiles(TestConstants.Projects.DefaultProjectIdentifier);

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
    public void CreateFile_With_Version_Should_Succeed()
    {
        var (fileName, token) = UploadFile();

        var filePayload = new Redmine.Net.Api.Types.File
        {
            Token = token,
            Filename = fileName,
            Description = RandomHelper.GenerateText(9),
            ContentType = "text/plain",
        };

        _ = fixture.RedmineManager.Create<Redmine.Net.Api.Types.File>(filePayload, TestConstants.Projects.DefaultProjectIdentifier);
        
        var files = fixture.RedmineManager.GetProjectFiles(TestConstants.Projects.DefaultProjectIdentifier);
        var file = files.Items.FirstOrDefault(x => x.Filename == fileName);
        
        Assert.NotNull(file);
        Assert.True(file.Id > 0);
        Assert.NotEmpty(file.Digest);
        Assert.Equal(filePayload.Description, file.Description);
        Assert.Equal(filePayload.ContentType, file.ContentType);
        Assert.EndsWith($"/attachments/download/{file.Id}/{fileName}", file.ContentUrl);
    }

    private (string fileName, string token) UploadFile()
    {
        var bytes = "Hello World!"u8.ToArray();
        var fileName = $"{RandomHelper.GenerateText(5)}.txt";
        var upload = fixture.RedmineManager.UploadFile(bytes, fileName);

        Assert.NotNull(upload);
        Assert.NotNull(upload.Token);

        return (fileName, upload.Token);
    }
}