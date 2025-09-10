using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class EnumerationTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task GetDocumentCategories_Should_Succeed()
    {
        // Act
        var categories = await fixture.RedmineManager.GetAsync<DocumentCategory>(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(categories);
    }
    
    [Fact]
    public async Task GetIssuePriorities_Should_Succeed()
    {
        // Act
        var issuePriorities = await fixture.RedmineManager.GetAsync<IssuePriority>(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(issuePriorities);
    }
    
    [Fact]
    public async Task GetTimeEntryActivities_Should_Succeed()
    {
        // Act
        var activities = await fixture.RedmineManager.GetAsync<TimeEntryActivity>(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(activities);
    }
}