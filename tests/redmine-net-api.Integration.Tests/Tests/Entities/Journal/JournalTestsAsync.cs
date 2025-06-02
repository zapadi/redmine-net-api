using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Common;
using Redmine.Net.Api;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Http;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.Journal;

[Collection(Constants.RedmineTestContainerCollection)]
public class JournalTestsAsync(RedmineTestContainerFixture fixture)
{
    private async Task<Redmine.Net.Api.Types.Issue> CreateRandomIssueAsync()
    {
        var issuePayload = TestEntityFactory.CreateRandomIssuePayload();
        return await fixture.RedmineManager.CreateAsync(issuePayload);
    }
    
    [Fact]
    public async Task Get_Issue_With_Journals_Should_Succeed()
    {
        //Arrange
        var testIssue = await CreateRandomIssueAsync();
        Assert.NotNull(testIssue);
        
        var issueIdToTest = testIssue.Id.ToInvariantString();

        testIssue.Notes = "This is a test note to create a journal entry.";
        await fixture.RedmineManager.UpdateAsync(issueIdToTest, testIssue);
        
        //Act
        var issueWithJournals = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.Issue>(
            issueIdToTest, 
            RequestOptions.Include(RedmineKeys.JOURNALS));

        //Assert
        Assert.NotNull(issueWithJournals);
        Assert.NotNull(issueWithJournals.Journals);
        Assert.True(issueWithJournals.Journals.Count > 0, "Issue should have journal entries.");
        Assert.Contains(issueWithJournals.Journals, j => j.Notes == testIssue.Notes);
    }
}