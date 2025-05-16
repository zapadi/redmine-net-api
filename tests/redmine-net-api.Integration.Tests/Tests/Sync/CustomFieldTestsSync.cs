using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Sync;

[Collection(Constants.RedmineTestContainerCollection)]
public class CustomFieldTests(RedmineTestContainerFixture fixture)
{
    [Fact]
    public void GetAllCustomFields_Should_Return_Null()
    {
        // Act
        var customFields = fixture.RedmineManager.Get<CustomField>();

        // Assert
        Assert.Null(customFields);
    }
}