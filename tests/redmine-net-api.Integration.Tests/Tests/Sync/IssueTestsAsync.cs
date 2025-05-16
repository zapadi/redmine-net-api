using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Helpers;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Sync;

[Collection(Constants.RedmineTestContainerCollection)]
public class IssueTests(RedmineTestContainerFixture fixture)
{
    [Fact]
    public void CreateIssue_Should_Succeed()
    {
        //Arrange
        var issue = IssueTestHelper.CreateIssue();
        var createdIssue = fixture.RedmineManager.Create(issue);
        
        // Assert
        Assert.NotNull(createdIssue);
        Assert.True(createdIssue.Id > 0);
    }
    
    [Fact]
    public void CreateIssue_With_IssueCustomField_Should_Succeed()
    {
        //Arrange
        var issue = IssueTestHelper.CreateIssue(customFields:
        [
            IssueCustomField.CreateSingle(1, RandomHelper.GenerateText(8), RandomHelper.GenerateText(4))
        ]);
        var createdIssue = fixture.RedmineManager.Create(issue);
        // Assert
        Assert.NotNull(createdIssue);
        Assert.True(createdIssue.Id > 0);
    }

    [Fact]
    public void GetIssue_Should_Succeed()
    {
        //Arrange
        var issue = IssueTestHelper.CreateIssue();
        var createdIssue = fixture.RedmineManager.Create(issue);
        
        Assert.NotNull(createdIssue);
        Assert.True(createdIssue.Id > 0);

        var issueId = issue.Id.ToInvariantString();

        //Act
        var retrievedIssue = fixture.RedmineManager.Get<Issue>(issueId);

        //Assert
        IssueTestHelper.AssertBasic(issue, retrievedIssue);
    }

    [Fact]
    public void UpdateIssue_Should_Succeed()
    {
        //Arrange
        var issue = IssueTestHelper.CreateIssue();
        Assert.NotNull(issue);

        var updatedSubject = RandomHelper.GenerateText(9);
        var updatedDescription = RandomHelper.GenerateText(18);
        var updatedStatusId = 2;

        issue.Subject = updatedSubject;
        issue.Description = updatedDescription;
        issue.Status = updatedStatusId.ToIssueStatusIdentifier();
        issue.Notes = RandomHelper.GenerateText("Note");

        var issueId = issue.Id.ToInvariantString();

        //Act
        fixture.RedmineManager.Update(issueId, issue);
        var retrievedIssue = fixture.RedmineManager.Get<Issue>(issueId);

        //Assert
        IssueTestHelper.AssertBasic(issue, retrievedIssue);
        Assert.Equal(updatedSubject, retrievedIssue.Subject);
        Assert.Equal(updatedDescription, retrievedIssue.Description);
        Assert.Equal(updatedStatusId, retrievedIssue.Status.Id);
    }

    [Fact]
    public void DeleteIssue_Should_Succeed()
    {
        //Arrange
        var issue = IssueTestHelper.CreateIssue();
        Assert.NotNull(issue);

        var issueId = issue.Id.ToInvariantString();

        //Act
        fixture.RedmineManager.Delete<Issue>(issueId);

        //Assert
        Assert.Throws<NotFoundException>(() => fixture.RedmineManager.Get<Issue>(issueId));
    }

    [Fact]
    public void GetIssue_With_Watchers_And_Relations_Should_Succeed()
    {
        var issue = IssueTestHelper.CreateIssue(
            [
                IssueCustomField.CreateMultiple(1, RandomHelper.GenerateText(8),
                    [RandomHelper.GenerateText(4), RandomHelper.GenerateText(4)])
            ],
            [new Watcher() { Id = 1 }, new Watcher() { Id = 2 }]);

        Assert.NotNull(issue);

        //Act
        var retrievedIssue = fixture.RedmineManager.Get<Issue>(issue.Id.ToInvariantString(),
            RequestOptions.Include($"{Include.Issue.Watchers},{Include.Issue.Relations}"));

        //Assert
        IssueTestHelper.AssertBasic(issue, retrievedIssue);
        Assert.NotNull(retrievedIssue.Relations);
        Assert.NotNull(retrievedIssue.Watchers);
    }
}