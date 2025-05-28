using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.CustomField;

[Collection(Constants.RedmineTestContainerCollection)]
public class CustomFieldTests(RedmineTestContainerFixture fixture)
{
    [Fact]
    public void GetAllCustomFields_Should_Return_Null()
    {
        // Act
        var customFields = fixture.RedmineManager.Get<Redmine.Net.Api.Types.CustomField>();

        // Assert
        Assert.Null(customFields);
    }
}