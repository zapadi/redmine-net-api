using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Helpers;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class SearchTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task Search_With_Q_Randomly_Set_Should_Return_Null()
    {
        // Arrange
        var searchBuilder = new SearchFilterBuilder
        {
            IncludeIssues = true,
            IncludeWikiPages = true
        };

        // Act
        var results = await fixture.RedmineManager.SearchAsync(RandomHelper.GenerateText(15), 100, searchFilter: searchBuilder, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(results);
        Assert.Null(results.Items);
    }

    [Fact]
    public async Task Search_With_Q_Set_Should_Return_Values()
    {
        var issue = new Issue
        {
            Project = 1.ToIdentifier(),
            Subject = $"Update {RandomHelper.GenerateText(9)}",
            Description = RandomHelper.GenerateText(18),
            Tracker = 1.ToIdentifier(),
            Status = 1.ToIssueStatusIdentifier(),
            Priority = 2.ToIdentifier(),
        };
        
        _ = await fixture.RedmineManager.CreateAsync(issue, cancellationToken: TestContext.Current.CancellationToken);
        
        // Wait for Redmine search index to update
        await Task.Delay(TimeSpan.FromSeconds(2), TestContext.Current.CancellationToken);
        
        // Arrange
        var searchBuilder = new SearchFilterBuilder
        {
            IncludeIssues = true,
            IncludeWikiPages = true
        };

        // Act
        var results = await fixture.RedmineManager.SearchAsync("Update", 100, searchFilter: searchBuilder, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(results);
        Assert.NotEmpty(results.Items);
    }
}
