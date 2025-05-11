using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class EnumerationTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task GetDocumentCategories_Should_Succeed()
    {
        // Act
        var categories = await fixture.RedmineManager.GetAsync<DocumentCategory>();

        // Assert
        Assert.NotNull(categories);
    }
    
    [Fact]
    public async Task GetIssuePriorities_Should_Succeed()
    {
        // Act
        var issuePriorities = await fixture.RedmineManager.GetAsync<IssuePriority>();

        // Assert
        Assert.NotNull(issuePriorities);
    }
    
    [Fact]
    public async Task GetTimeEntryActivities_Should_Succeed()
    {
        // Act
        var activities = await fixture.RedmineManager.GetAsync<TimeEntryActivity>();

        // Assert
        Assert.NotNull(activities);
    }
}