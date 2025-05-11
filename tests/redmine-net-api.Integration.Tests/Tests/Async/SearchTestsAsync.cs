using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Redmine.Net.Api;
using Redmine.Net.Api.Extensions;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class SearchTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task Search_Should_Succeed()
    {
        // Arrange
        var searchBuilder = new SearchFilterBuilder
        {
            IncludeIssues = true,
            IncludeWikiPages = true
        };

        // Act
        var results = await fixture.RedmineManager.SearchAsync("query_string",100, searchFilter:searchBuilder);

        // Assert
        Assert.NotNull(results);
        Assert.Empty(results.Items);
    }
}