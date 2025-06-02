using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Helpers;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Common;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.Version;

[Collection(Constants.RedmineTestContainerCollection)]
public class VersionTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task CreateVersion_Should_Succeed()
    {
        // Arrange
        var versionPayload = TestEntityFactory.CreateRandomVersionPayload();

        // Act
        var version = await fixture.RedmineManager.CreateAsync(versionPayload, TestConstants.Projects.DefaultProjectIdentifier);

        // Assert
        Assert.NotNull(version);
        Assert.True(version.Id > 0);
        Assert.Equal(versionPayload.Name, version.Name);
        Assert.Equal(versionPayload.Description, version.Description);
        Assert.Equal(versionPayload.Status, version.Status);
        Assert.Equal(TestConstants.Projects.DefaultProjectId, version.Project.Id);
    }

    [Fact]
    public async Task GetVersion_Should_Succeed()
    {
        // Arrange
        var versionPayload = TestEntityFactory.CreateRandomVersionPayload();
        var version = await fixture.RedmineManager.CreateAsync(versionPayload, TestConstants.Projects.DefaultProjectIdentifier);
        Assert.NotNull(version);

        // Act
        var retrievedVersion = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.Version>(version.Id.ToInvariantString());

        // Assert
        Assert.NotNull(retrievedVersion);
        Assert.Equal(version.Id, retrievedVersion.Id);
        Assert.Equal(version.Name, retrievedVersion.Name);
        Assert.Equal(version.Description, retrievedVersion.Description);
    }

    [Fact]
    public async Task UpdateVersion_Should_Succeed()
    {
        // Arrange
        var versionPayload = TestEntityFactory.CreateRandomVersionPayload();
        var version = await fixture.RedmineManager.CreateAsync(versionPayload, TestConstants.Projects.DefaultProjectIdentifier);
        Assert.NotNull(version);

        version.Description = RandomHelper.GenerateText(20);
        version.Status = VersionStatus.Locked;

        // Act
        await fixture.RedmineManager.UpdateAsync(version.Id.ToString(), version);
        var retrievedVersion = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.Version>(version.Id.ToInvariantString());

        // Assert
        Assert.NotNull(retrievedVersion);
        Assert.Equal(version.Id, retrievedVersion.Id);
        Assert.Equal(version.Description, retrievedVersion.Description);
        Assert.Equal(version.Status, retrievedVersion.Status);
    }

    [Fact]
    public async Task DeleteVersion_Should_Succeed()
    {
        // Arrange
        var versionPayload = TestEntityFactory.CreateRandomVersionPayload();
        var version = await fixture.RedmineManager.CreateAsync(versionPayload, TestConstants.Projects.DefaultProjectIdentifier);
        Assert.NotNull(version);

        // Act
        await fixture.RedmineManager.DeleteAsync<Redmine.Net.Api.Types.Version>(version);

        // Assert
        await Assert.ThrowsAsync<RedmineNotFoundException>(async () =>
            await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.Version>(version));
    }
}