using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Helpers;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Http;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.Issue;

[Collection(Constants.RedmineTestContainerCollection)]
public class IssueTests(RedmineTestContainerFixture fixture)
{
    [Fact]
    public void CreateIssue_Should_Succeed()
    {
        //Arrange
        var (issue, _) = IssueTestHelper.CreateRandomIssue(fixture.RedmineManager);

        // Assert
        Assert.NotNull(issue);
        Assert.True(issue.Id > 0);
    }

    [Fact]
    public void CreateIssue_With_IssueCustomField_Should_Succeed()
    {
        //Arrange
        var (issue, _) = IssueTestHelper.CreateRandomIssue(fixture.RedmineManager, customFields:
        [
            TestEntityFactory.CreateRandomIssueCustomFieldWithSingleValuePayload()
        ]);

        // Assert
        Assert.NotNull(issue);
        Assert.True(issue.Id > 0);
    }

    [Fact]
    public void GetIssue_Should_Succeed()
    {
        //Arrange
        var (issue, issuePayload) = IssueTestHelper.CreateRandomIssue(fixture.RedmineManager);

        Assert.NotNull(issue);
        Assert.True(issue.Id > 0);

        var issueId = issue.Id.ToInvariantString();

        //Act
        var retrievedIssue = fixture.RedmineManager.Get<Redmine.Net.Api.Types.Issue>(issueId);

        //Assert
        IssueTestHelper.AssertBasic(issuePayload, retrievedIssue);
    }

    [Fact]
    public void UpdateIssue_Should_Succeed()
    {
        //Arrange
        var (issue, _) = IssueTestHelper.CreateRandomIssue(fixture.RedmineManager);

        issue.Subject = RandomHelper.GenerateText(9);
        issue.Description = RandomHelper.GenerateText(18);
        issue.Status = 2.ToIssueStatusIdentifier();
        issue.Notes = RandomHelper.GenerateText("Note");

        var issueId = issue.Id.ToInvariantString();

        //Act
        fixture.RedmineManager.Update(issueId, issue);
        var updatedIssue = fixture.RedmineManager.Get<Redmine.Net.Api.Types.Issue>(issueId);

        //Assert
        IssueTestHelper.AssertBasic(issue, updatedIssue);
        Assert.Equal(issue.Subject, updatedIssue.Subject);
        Assert.Equal(issue.Description, updatedIssue.Description);
        Assert.Equal(issue.Status.Id, updatedIssue.Status.Id);
    }

    [Fact]
    public void DeleteIssue_Should_Succeed()
    {
        //Arrange
        var (issue, _) = IssueTestHelper.CreateRandomIssue(fixture.RedmineManager);

        var issueId = issue.Id.ToInvariantString();

        //Act
        fixture.RedmineManager.Delete<Redmine.Net.Api.Types.Issue>(issueId);

        //Assert
        Assert.Throws<NotFoundException>(() => fixture.RedmineManager.Get<Redmine.Net.Api.Types.Issue>(issueId));
    }

    [Fact]
    public void GetIssue_With_Watchers_And_Relations_Should_Succeed()
    {
        var userPayload = TestEntityFactory.CreateRandomUserPayload();
        var createdUser = fixture.RedmineManager.Create(userPayload);
        Assert.NotNull(createdUser);

        var userId = createdUser.Id;

        var (firstIssue, _) = IssueTestHelper.CreateRandomIssue(fixture.RedmineManager, customFields:
            [
                IssueCustomField.CreateMultiple(1, RandomHelper.GenerateText(8),
                    [RandomHelper.GenerateText(4), RandomHelper.GenerateText(4)])
            ], watchers:
            [new Watcher() { Id = 1 }, new Watcher() { Id = userId }]);

        var (secondIssue, _) = IssueTestHelper.CreateRandomIssue(fixture.RedmineManager,
            customFields: [TestEntityFactory.CreateRandomIssueCustomFieldWithMultipleValuesPayload()],
            watchers: [new Watcher() { Id = 1 }, new Watcher() { Id = userId }]);

        var issueRelation = new IssueRelation()
        {
            Type = IssueRelationType.Relates,
            IssueToId = firstIssue.Id,
        };
        _ = fixture.RedmineManager.Create(issueRelation, secondIssue.Id.ToInvariantString());

        //Act
        var retrievedIssue = fixture.RedmineManager.Get<Redmine.Net.Api.Types.Issue>(secondIssue.Id.ToInvariantString(),
            RequestOptions.Include($"{Include.Issue.Watchers},{Include.Issue.Relations}"));

        //Assert
        IssueTestHelper.AssertBasic(secondIssue, retrievedIssue);
        Assert.NotNull(retrievedIssue.Watchers);
        Assert.NotNull(retrievedIssue.Relations);
    }
}