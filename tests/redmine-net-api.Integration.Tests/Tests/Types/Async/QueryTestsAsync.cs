using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class QueryTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task GetAllQueries_Should_Succeed()
    {
        // Act
        var queries = await fixture.RedmineManager.GetAsync<Query>(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(queries);
    }
}