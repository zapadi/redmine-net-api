using Padi.RedmineApi.Extensions;
using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Helpers;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Padi.RedmineApi.Types;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class NewsTestsAsync(RedmineTestContainerFixture fixture)
{
    private const string PROJECT_ID = "1";

    private static News CreateRandomNews()
    {
        return new News() { Title = RandomHelper.GenerateText(5), Summary = RandomHelper.GenerateText(10), Description = RandomHelper.GenerateText(20), };
    }

    [Fact]
    public async Task GetAllNews_Should_Succeed()
    {
        // Arrange
        _ = await fixture.RedmineManager.AddProjectNewsAsync(PROJECT_ID, CreateRandomNews());

        var entity = new Project
        {
            Identifier = RandomHelper.GenerateText(3, lowerCase: true),
            Name = RandomHelper.GenerateText(4),
        };

        var project = await fixture.RedmineManager.CreateAsync(entity);


        _ = await fixture.RedmineManager.AddProjectNewsAsync(project.Identifier, CreateRandomNews());

        // Act
        var news = await fixture.RedmineManager.GetAsync<News>();

        // Assert
        Assert.NotNull(news);
    }

    [Fact]
    public async Task GetProjectNews_Should_Succeed()
    {
        // Arrange
        var newsCreated = await fixture.RedmineManager.AddProjectNewsAsync(PROJECT_ID, CreateRandomNews());

        // Act
        var news = await fixture.RedmineManager.GetProjectNewsAsync(PROJECT_ID);

        // Assert
        Assert.NotNull(news);
    }
}
