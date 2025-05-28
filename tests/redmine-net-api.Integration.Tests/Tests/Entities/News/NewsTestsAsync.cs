using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Helpers;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api.Extensions;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.News;

[Collection(Constants.RedmineTestContainerCollection)]
public class NewsTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task GetAllNews_Should_Succeed()
    {
        // Arrange
        _ = await fixture.RedmineManager.AddProjectNewsAsync(TestConstants.Projects.DefaultProjectIdentifier, TestEntityFactory.CreateRandomNewsPayload());
        
        _ = await fixture.RedmineManager.AddProjectNewsAsync("2", TestEntityFactory.CreateRandomNewsPayload());
        
        // Act
        var news = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.News>();

        // Assert
        Assert.NotNull(news);
    }
    
    [Fact]
    public async Task GetProjectNews_Should_Succeed()
    {
        // Arrange
        var newsCreated = await fixture.RedmineManager.AddProjectNewsAsync(TestConstants.Projects.DefaultProjectIdentifier, TestEntityFactory.CreateRandomNewsPayload());
        
        // Act
        var news = await fixture.RedmineManager.GetProjectNewsAsync(TestConstants.Projects.DefaultProjectIdentifier);

        // Assert
        Assert.NotNull(news);
    }
    
    [Fact]
    public async Task News_AddWithUploads_Should_Succeed()
    {
        // Arrange
        var newsPayload = TestEntityFactory.CreateRandomNewsPayload();
        var newsCreated = await fixture.RedmineManager.AddProjectNewsAsync(TestConstants.Projects.DefaultProjectIdentifier, newsPayload);
        
        // Act
        var news = await fixture.RedmineManager.GetProjectNewsAsync(TestConstants.Projects.DefaultProjectIdentifier);

        // Assert
        Assert.NotNull(news);
    }
}