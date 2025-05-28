using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.Query;

[Collection(Constants.RedmineTestContainerCollection)]
public class QueryTests(RedmineTestContainerFixture fixture)
{
    [Fact]
    public void GetAllQueries_Should_Succeed()
    {
        // Act
        var queries = fixture.RedmineManager.Get<Redmine.Net.Api.Types.Query>();

        // Assert
        Assert.NotNull(queries);
    }
}