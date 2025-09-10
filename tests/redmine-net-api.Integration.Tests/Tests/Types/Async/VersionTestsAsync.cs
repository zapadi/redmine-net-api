using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Helpers;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Types;
using Xunit;
using Version = Redmine.Net.Api.Types.Version;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class VersionTestsAsync(RedmineTestContainerFixture fixture)
{
    private const string PROJECT_ID = "1";
    
    private async Task<Version> CreateTestVersionAsync()
    {
        var version = new Version
        {
            Name = RandomHelper.GenerateText(10),
            Description = RandomHelper.GenerateText(15),
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
        var versionSuffix = RandomHelper.GenerateText(6);
        var versionData = new Version
        {
            Name = $"Test Version Create {versionSuffix}",
            Description = $"Initial create test description {Guid.NewGuid()}",
            Status = VersionStatus.Open,
            Sharing = VersionSharing.System,
            DueDate = DateTime.Now.Date.AddDays(10)
        };

        //Act
        var createdVersion = await fixture.RedmineManager.CreateAsync(versionData, PROJECT_ID, cancellationToken: TestContext.Current.CancellationToken);

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
        var retrievedVersion = await fixture.RedmineManager.GetAsync<Version>(createdVersion.Id.ToInvariantString(), cancellationToken: TestContext.Current.CancellationToken);

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

        var updatedDescription = RandomHelper.GenerateText(20);
        var updatedStatus = VersionStatus.Locked;
        createdVersion.Description = updatedDescription;
        createdVersion.Status = updatedStatus;

        //Act
        await fixture.RedmineManager.UpdateAsync(createdVersion.Id.ToInvariantString(), createdVersion, cancellationToken: TestContext.Current.CancellationToken);
        var retrievedVersion = await fixture.RedmineManager.GetAsync<Version>(createdVersion.Id.ToInvariantString(), cancellationToken: TestContext.Current.CancellationToken);

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
        await fixture.RedmineManager.DeleteAsync<Version>(versionId, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        var ex = await Assert.ThrowsAsync<RedmineApiException>(async () =>
            await fixture.RedmineManager.GetAsync<Version>(versionId, cancellationToken: TestContext.Current.CancellationToken));
        
        Assert.NotNull(ex);
        Assert.Equal(HttpConstants.StatusCodes.NotFound, ex.HttpStatusCode);
    }
}