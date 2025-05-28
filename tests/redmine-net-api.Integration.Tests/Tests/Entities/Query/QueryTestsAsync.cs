using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.Query;

[Collection(Constants.RedmineTestContainerCollection)]
public class QueryTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task GetAllQueries_Should_Succeed()
    {
        // Act
        var queries = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.Query>();

        // Assert
        Assert.NotNull(queries);
    }
}