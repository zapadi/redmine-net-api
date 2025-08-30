using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Padi.RedmineApi.Types;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Async;

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