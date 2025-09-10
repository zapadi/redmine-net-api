
using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Sync;

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