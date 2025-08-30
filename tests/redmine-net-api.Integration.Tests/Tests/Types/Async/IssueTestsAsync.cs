using Padi.RedmineApi.Exceptions;
using Padi.RedmineApi.Extensions;
using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Helpers;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Padi.RedmineAPI.Integration.Tests.Tests.Types.Base;
using Padi.RedmineApi.Internals;
using Padi.RedmineApi.Types;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class IssueTestsAsync : IssueTestsBase
{
    public IssueTestsAsync(RedmineTestContainerFixture fixture) : base(fixture)
    {
    }

    private async Task<Issue> CreateTestIssueAsync()
    {
        var issue = CreateTestIssueData();
        return await Fixture.RedmineManager.CreateAsync(issue);
    }

    [Fact]
    public async Task CreateIssue_Should_Succeed()
    {
        //Arrange
        var issueData = CreateFullIssueData();

        //Act
        var cr = await Fixture.RedmineManager.CreateAsync(issueData);
        var createdIssue = await Fixture.RedmineManager.GetAsync<Issue>(cr.Id.ToString());

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
        var retrievedIssue = await Fixture.RedmineManager.GetAsync<Issue>(issueId);

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
        await Fixture.RedmineManager.UpdateAsync(issueId, createdIssue);
        var retrievedIssue = await Fixture.RedmineManager.GetAsync<Issue>(issueId);

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
        await Fixture.RedmineManager.DeleteAsync<Issue>(issueId);

        //Assert
        var ex = await Assert.ThrowsAsync<RedmineApiException>(
            async () => await Fixture.RedmineManager.GetAsync<Issue>(issueId));
        Assert.NotNull(ex);
        Assert.Equal(HttpConstants.StatusCodes.NotFound, ex.HttpStatusCode);
    }
}