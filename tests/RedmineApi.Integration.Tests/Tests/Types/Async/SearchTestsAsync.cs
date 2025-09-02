using Padi.RedmineApi.Builders;
using Padi.RedmineApi.Extensions;
using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Helpers;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class SearchTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task Search_With_Q_Randomly_Set_Should_Return_Null()
    {
        // Arrange
        var searchBuilder = new SearchFilterBuilder
        {
            IncludeIssues = true,
            IncludeWikiPages = true
        };

        // Act
        var results = await fixture.RedmineManager.SearchAsync(RandomHelper.GenerateText(15),100, searchFilter:searchBuilder);

        // Assert
        Assert.NotNull(results);
        Assert.Null(results.Items);
    }

    [Fact]
    public async Task Search_With_Q_Set_Should_Return_Values()
    {
        // Arrange
        var searchBuilder = new SearchFilterBuilder
        {
            IncludeIssues = true,
            IncludeWikiPages = true
        };

        // Act
        var results = await fixture.RedmineManager.SearchAsync("Updated",100, searchFilter:searchBuilder);

        // Assert
        Assert.NotNull(results);
        Assert.NotEmpty(results.Items);
    }
}
