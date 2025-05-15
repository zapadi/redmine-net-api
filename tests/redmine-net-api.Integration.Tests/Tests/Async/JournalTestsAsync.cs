using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Redmine.Net.Api;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class JournalTestsAsync(RedmineTestContainerFixture fixture)
{
    private async Task<Issue> CreateTestIssueAsync()
    {
        var issue = new Issue
        {
            Project = IdentifiableName.Create<Project>(1),
            Subject = RandomHelper.GenerateText(13),
            Description = RandomHelper.GenerateText(19),
            Tracker = 1.ToIdentifier(),
            Status = 1.ToIssueStatusIdentifier(),
            Priority = 2.ToIdentifier(),
        };
        return await fixture.RedmineManager.CreateAsync(issue);
    }
    
    [Fact]
    public async Task Get_Issue_With_Journals_Should_Succeed()
    {
        //Arrange
        var testIssue = await CreateTestIssueAsync();
        Assert.NotNull(testIssue);
        
        var issueIdToTest = testIssue.Id.ToInvariantString();

        testIssue.Notes = "This is a test note to create a journal entry.";
        await fixture.RedmineManager.UpdateAsync(issueIdToTest, testIssue);
        
        //Act
        var issueWithJournals = await fixture.RedmineManager.GetAsync<Issue>(
            issueIdToTest, 
            RequestOptions.Include(RedmineKeys.JOURNALS));

        //Assert
        Assert.NotNull(issueWithJournals);
        Assert.NotNull(issueWithJournals.Journals);
        Assert.True(issueWithJournals.Journals.Count > 0, "Issue should have journal entries.");
        Assert.Contains(issueWithJournals.Journals, j => j.Notes == testIssue.Notes);
    }
}