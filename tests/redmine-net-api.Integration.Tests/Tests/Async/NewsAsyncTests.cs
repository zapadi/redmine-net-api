using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class NewsTestsAsync(RedmineTestContainerFixture fixture)
{
    private const string PROJECT_ID = "1";
    
    [Fact]
    public async Task GetAllNews_Should_Succeed()
    {
        // Arrange
        _ = await fixture.RedmineManager.AddProjectNewsAsync(PROJECT_ID, new News()
        {
            Title = ThreadSafeRandom.GenerateText(5),
            Summary = ThreadSafeRandom.GenerateText(10),
            Description = ThreadSafeRandom.GenerateText(20),
        });
        
        _ = await fixture.RedmineManager.AddProjectNewsAsync("2", new News()
        {
            Title = ThreadSafeRandom.GenerateText(5),
            Summary = ThreadSafeRandom.GenerateText(10),
            Description = ThreadSafeRandom.GenerateText(20),
        });
        
        
        // Act
        var news = await fixture.RedmineManager.GetAsync<News>();

        // Assert
        Assert.NotNull(news);
    }
    
    [Fact]
    public async Task GetProjectNews_Should_Succeed()
    {
        // Arrange
        var newsCreated = await fixture.RedmineManager.AddProjectNewsAsync(PROJECT_ID, new News()
        {
            Title = ThreadSafeRandom.GenerateText(5),
            Summary = ThreadSafeRandom.GenerateText(10),
            Description = ThreadSafeRandom.GenerateText(20),
        });
        
        // Act
        var news = await fixture.RedmineManager.GetProjectNewsAsync(PROJECT_ID);

        // Assert
        Assert.NotNull(news);
    }
}