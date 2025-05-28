using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.CustomField;

[Collection(Constants.RedmineTestContainerCollection)]
public class CustomFieldTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task GetAllCustomFields_Should_Return_Null()
    {
        // Act
        var customFields = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.CustomField>();

        // Assert
        Assert.Null(customFields);
    }
}