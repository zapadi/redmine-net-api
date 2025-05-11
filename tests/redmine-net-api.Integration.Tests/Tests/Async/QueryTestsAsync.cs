using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class QueryTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task GetAllQueries_Should_Succeed()
    {
        // Act
        var queries = await fixture.RedmineManager.GetAsync<Query>();

        // Assert
        Assert.NotNull(queries);
    }
}