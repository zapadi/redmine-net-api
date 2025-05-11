using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class CustomFieldTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task GetAllCustomFields_Should_Return_Null()
    {
        // Act
        var customFields = await fixture.RedmineManager.GetAsync<CustomField>();

        // Assert
        Assert.Null(customFields);
    }
}