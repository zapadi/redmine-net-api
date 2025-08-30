using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Padi.RedmineApi.Types;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class CustomFieldTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task GetAllCustomFields_Should_Succeed()
    {
        // Act
        var customFields = await fixture.RedmineManager.GetAsync<CustomField>();

        // Assert
        Assert.NotNull(customFields);
        Assert.NotEmpty(customFields);
    }
}