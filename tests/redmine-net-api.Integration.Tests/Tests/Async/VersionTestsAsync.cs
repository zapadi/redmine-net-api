using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;
using Version = Redmine.Net.Api.Types.Version;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class VersionTestsAsync(RedmineTestContainerFixture fixture)
{
    private const string PROJECT_ID = "1";
    
    private async Task<Version> CreateTestVersionAsync()
    {
        var version = new Version
        {
            Name = ThreadSafeRandom.GenerateText(10),
            Description = ThreadSafeRandom.GenerateText(15),
            Status = VersionStatus.Open,
            Sharing = VersionSharing.None,
            DueDate = DateTime.Now.Date.AddDays(30)
        };
        return await fixture.RedmineManager.CreateAsync(version, PROJECT_ID);
    }

    [Fact]
    public async Task CreateVersion_Should_Succeed()
    {
        //Arrange
        var versionSuffix = Guid.NewGuid().ToString("N");
        var versionData = new Version
        {
            Name = $"Test Version Create {versionSuffix}",
            Description = $"Initial create test description {Guid.NewGuid()}",
            Status = VersionStatus.Open,
            Sharing = VersionSharing.System,
            DueDate = DateTime.Now.Date.AddDays(10)
        };

        //Act
        var createdVersion = await fixture.RedmineManager.CreateAsync(versionData, PROJECT_ID);

        //Assert
        Assert.NotNull(createdVersion);
        Assert.True(createdVersion.Id > 0);
        Assert.Equal(versionData.Name, createdVersion.Name);
        Assert.Equal(versionData.Description, createdVersion.Description);
        Assert.Equal(versionData.Status, createdVersion.Status);
        Assert.Equal(PROJECT_ID, createdVersion.Project.Id.ToInvariantString());
    }

    [Fact]
    public async Task GetVersion_Should_Succeed()
    {

        //Arrange
        var createdVersion = await CreateTestVersionAsync();
        Assert.NotNull(createdVersion);

        //Act
        var retrievedVersion = await fixture.RedmineManager.GetAsync<Version>(createdVersion.Id.ToInvariantString());

        //Assert
        Assert.NotNull(retrievedVersion);
        Assert.Equal(createdVersion.Id, retrievedVersion.Id);
        Assert.Equal(createdVersion.Name, retrievedVersion.Name);
        Assert.Equal(createdVersion.Description, retrievedVersion.Description);
    }

    [Fact]
    public async Task UpdateVersion_Should_Succeed()
    {
        //Arrange
        var createdVersion = await CreateTestVersionAsync();
        Assert.NotNull(createdVersion);

        var updatedDescription = ThreadSafeRandom.GenerateText(20);
        var updatedStatus = VersionStatus.Locked;
        createdVersion.Description = updatedDescription;
        createdVersion.Status = updatedStatus;

        //Act
        await fixture.RedmineManager.UpdateAsync(createdVersion.Id.ToInvariantString(), createdVersion);
        var retrievedVersion = await fixture.RedmineManager.GetAsync<Version>(createdVersion.Id.ToInvariantString());

        //Assert
        Assert.NotNull(retrievedVersion);
        Assert.Equal(createdVersion.Id, retrievedVersion.Id);
        Assert.Equal(updatedDescription, retrievedVersion.Description);
        Assert.Equal(updatedStatus, retrievedVersion.Status);
    }

    [Fact]
    public async Task DeleteVersion_Should_Succeed()
    {
        //Arrange
        var createdVersion = await CreateTestVersionAsync();
        Assert.NotNull(createdVersion);
        var versionId = createdVersion.Id.ToInvariantString();

        //Act
        await fixture.RedmineManager.DeleteAsync<Version>(versionId);

        //Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await fixture.RedmineManager.GetAsync<Version>(versionId));
    }
}