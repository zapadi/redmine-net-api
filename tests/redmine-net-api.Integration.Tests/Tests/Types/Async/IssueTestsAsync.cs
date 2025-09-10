using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Helpers;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Padi.RedmineAPI.Integration.Tests.Tests.Types.Base;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class IssueTestsAsync : IssueTestsBase
{
    public IssueTestsAsync(RedmineTestContainerFixture fixture) : base(fixture)
    {
    }

    private async Task<Issue> CreateTestIssueAsync(bool withCustomFields = false)
    {
        var issue = CreateTestIssueData(withCustomFields);
        return await Fixture.RedmineManager.CreateAsync(issue);
    }

    [Fact]
    public async Task CreateIssue_Should_Succeed()
    {
        //Arrange
        var issueData = CreateFullIssueData();

        //Act
        var cr = await Fixture.RedmineManager.CreateAsync(issueData, cancellationToken: TestContext.Current.CancellationToken);
        var createdIssue = await Fixture.RedmineManager.GetAsync<Issue>(cr.Id.ToString(), cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        AssertIssueEquals(issueData, createdIssue);
    }

    [Fact]
    public async Task GetIssue_Should_Succeed()
    {
        //Arrange
        var createdIssue = await CreateTestIssueAsync();
        Assert.NotNull(createdIssue);
        
        var issueId = createdIssue.Id.ToInvariantString();

        //Act
        var retrievedIssue = await Fixture.RedmineManager.GetAsync<Issue>(issueId, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        AssertIssueEquals(createdIssue, retrievedIssue);
    }

    [Fact]
    public async Task UpdateIssue_Should_Succeed()
    {
        //Arrange
        var createdIssue = await CreateTestIssueAsync();
        Assert.NotNull(createdIssue);

        var updatedSubject = RandomHelper.GenerateText(9);
        var updatedDescription = RandomHelper.GenerateText(18);
        var updatedStatusId = 2;
        
        createdIssue.Subject = updatedSubject;
        createdIssue.Description = updatedDescription;
        createdIssue.Status = updatedStatusId.ToIssueStatusIdentifier();
        createdIssue.Notes = RandomHelper.GenerateText("Note");

        var issueId = createdIssue.Id.ToInvariantString();

        //Act
        await Fixture.RedmineManager.UpdateAsync(issueId, createdIssue, cancellationToken: TestContext.Current.CancellationToken);
        var retrievedIssue = await Fixture.RedmineManager.GetAsync<Issue>(issueId, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        Assert.NotNull(retrievedIssue);
        Assert.Equal(createdIssue.Id, retrievedIssue.Id);
        Assert.Equal(updatedSubject, retrievedIssue.Subject);
        Assert.Equal(updatedDescription, retrievedIssue.Description);
        Assert.Equal(updatedStatusId, retrievedIssue.Status.Id);
    }

    [Fact]
    public async Task DeleteIssue_Should_Succeed()
    {
        //Arrange
        var createdIssue = await CreateTestIssueAsync();
        Assert.NotNull(createdIssue);
        
        var issueId = createdIssue.Id.ToInvariantString();

        //Act
        await Fixture.RedmineManager.DeleteAsync<Issue>(issueId, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        var ex = await Assert.ThrowsAsync<RedmineApiException>(
            async () => await Fixture.RedmineManager.GetAsync<Issue>(issueId, cancellationToken: TestContext.Current.CancellationToken));
        Assert.NotNull(ex);
        Assert.Equal(HttpConstants.StatusCodes.NotFound, ex.HttpStatusCode);
    }
}
