using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Helpers;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Padi.RedmineAPI.Integration.Tests.Tests.Types.Base;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Sync;

[Collection(Constants.RedmineTestContainerCollection)]
public class IssueTests : IssueTestsBase
{
    public IssueTests(RedmineTestContainerFixture fixture) : base(fixture)
    {
    }

    private Issue CreateTestIssue()
    {
        var issue = CreateTestIssueData();
        return Fixture.RedmineManager.Create(issue);
    }

    [Fact]
    public void CreateIssue_Should_Succeed()
    {
        //Arrange
        var issueData = CreateFullIssueData();

        //Act
        var cr = Fixture.RedmineManager.Create(issueData);
        var createdIssue = Fixture.RedmineManager.Get<Issue>(cr.Id.ToString());

        //Assert
        AssertIssueEquals(issueData, createdIssue);
    }

    [Fact]
    public void GetIssue_Should_Succeed()
    {
        //Arrange
        var createdIssue = CreateTestIssue();
        Assert.NotNull(createdIssue);
        
        var issueId = createdIssue.Id.ToInvariantString();

        //Act
        var retrievedIssue = Fixture.RedmineManager.Get<Issue>(issueId);

        //Assert
        AssertIssueEquals(createdIssue, retrievedIssue);
    }

    [Fact]
    public void UpdateIssue_Should_Succeed()
    {
        //Arrange
        var createdIssue = CreateTestIssue();
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
        Fixture.RedmineManager.Update(issueId, createdIssue);
        var retrievedIssue = Fixture.RedmineManager.Get<Issue>(issueId);

        //Assert
        Assert.NotNull(retrievedIssue);
        Assert.Equal(createdIssue.Id, retrievedIssue.Id);
        Assert.Equal(updatedSubject, retrievedIssue.Subject);
        Assert.Equal(updatedDescription, retrievedIssue.Description);
        Assert.Equal(updatedStatusId, retrievedIssue.Status.Id);
    }

    [Fact]
    public void DeleteIssue_Should_Succeed()
    {
        //Arrange
        var createdIssue = CreateTestIssue();
        Assert.NotNull(createdIssue);
        
        var issueId = createdIssue.Id.ToInvariantString();

        //Act
        Fixture.RedmineManager.Delete<Issue>(issueId);

        //Assert
        var ex = Assert.Throws<RedmineApiException>(
            () => Fixture.RedmineManager.Get<Issue>(issueId));
        Assert.NotNull(ex);
        Assert.Equal(HttpConstants.StatusCodes.NotFound, ex.HttpStatusCode);
    }
}
