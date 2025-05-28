using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Extensions;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.Search;

[Collection(Constants.RedmineTestContainerCollection)]
public class SearchTests(RedmineTestContainerFixture fixture)
{
    [Fact]
    public void Search_Should_Succeed()
    {
        // Arrange
        var searchBuilder = new SearchFilterBuilder
        {
            IncludeIssues = true,
            IncludeWikiPages = true
        };

        // Act
        var results = fixture.RedmineManager.Search("query_string",100, searchFilter:searchBuilder);

        // Assert
        Assert.NotNull(results);
        Assert.Null(results.Items);
    }
}