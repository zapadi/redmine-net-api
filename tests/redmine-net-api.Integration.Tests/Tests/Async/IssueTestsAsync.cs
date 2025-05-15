using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class IssueTestsAsync(RedmineTestContainerFixture fixture)
{
    private static readonly IdentifiableName ProjectIdName = IdentifiableName.Create<Project>(1);

    private async Task<Issue> CreateTestIssueAsync(List<IssueCustomField> customFields = null, List<Watcher> watchers = null)
    {
        var issue = new Issue
        {
            Project = ProjectIdName,
            Subject = ThreadSafeRandom.GenerateText(9),
            Description = ThreadSafeRandom.GenerateText(18),
            Tracker = 1.ToIdentifier(),
            Status = 1.ToIssueStatusIdentifier(),
            Priority = 2.ToIdentifier(),
            CustomFields = customFields,
            Watchers = watchers
        };
        return await fixture.RedmineManager.CreateAsync(issue);
    }

    [Fact]
    public async Task CreateIssue_Should_Succeed()
    {
        //Arrange
        var issueData = new Issue
        {
            Project = ProjectIdName,
            Subject = ThreadSafeRandom.GenerateText(9),
            Description = ThreadSafeRandom.GenerateText(18),
            Tracker = 2.ToIdentifier(),
            Status = 1.ToIssueStatusIdentifier(),
            Priority = 3.ToIdentifier(),
            StartDate = DateTime.Now.Date,
            DueDate = DateTime.Now.Date.AddDays(7),
            EstimatedHours = 8,
            CustomFields =
            [
                IssueCustomField.CreateSingle(1, ThreadSafeRandom.GenerateText(8), ThreadSafeRandom.GenerateText(4)) 
            ]
        };

        //Act
        var cr = await fixture.RedmineManager.CreateAsync(issueData);
        var createdIssue = await fixture.RedmineManager.GetAsync<Issue>(cr.Id.ToString());

        //Assert
        Assert.NotNull(createdIssue);
        Assert.True(createdIssue.Id > 0);
        Assert.Equal(issueData.Subject, createdIssue.Subject);
        Assert.Equal(issueData.Description, createdIssue.Description);
        Assert.Equal(issueData.Project.Id, createdIssue.Project.Id);
        Assert.Equal(issueData.Tracker.Id, createdIssue.Tracker.Id);
        Assert.Equal(issueData.Status.Id, createdIssue.Status.Id);
        Assert.Equal(issueData.Priority.Id, createdIssue.Priority.Id);
        Assert.Equal(issueData.StartDate, createdIssue.StartDate);
        Assert.Equal(issueData.DueDate, createdIssue.DueDate);
        // Assert.Equal(issueData.EstimatedHours, createdIssue.EstimatedHours);
    }

    [Fact]
    public async Task GetIssue_Should_Succeed()
    {
        //Arrange
        var createdIssue = await CreateTestIssueAsync();
        Assert.NotNull(createdIssue);
        
        var issueId = createdIssue.Id.ToInvariantString();

        //Act
        var retrievedIssue = await fixture.RedmineManager.GetAsync<Issue>(issueId);

        //Assert
        Assert.NotNull(retrievedIssue);
        Assert.Equal(createdIssue.Id, retrievedIssue.Id);
        Assert.Equal(createdIssue.Subject, retrievedIssue.Subject);
        Assert.Equal(createdIssue.Description, retrievedIssue.Description);
        Assert.Equal(createdIssue.Project.Id, retrievedIssue.Project.Id);
    }

    [Fact]
    public async Task UpdateIssue_Should_Succeed()
    {
        //Arrange
        var createdIssue = await CreateTestIssueAsync();
        Assert.NotNull(createdIssue);

        var updatedSubject = ThreadSafeRandom.GenerateText(9);
        var updatedDescription = ThreadSafeRandom.GenerateText(18);
        var updatedStatusId = 2; 
        
        createdIssue.Subject = updatedSubject;
        createdIssue.Description = updatedDescription;
        createdIssue.Status = updatedStatusId.ToIssueStatusIdentifier(); 
        createdIssue.Notes = ThreadSafeRandom.GenerateText("Note");

        var issueId = createdIssue.Id.ToInvariantString();

        //Act
        await fixture.RedmineManager.UpdateAsync(issueId, createdIssue);
        var retrievedIssue = await fixture.RedmineManager.GetAsync<Issue>(issueId);

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
        await fixture.RedmineManager.DeleteAsync<Issue>(issueId);

        //Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await fixture.RedmineManager.GetAsync<Issue>(issueId));
    }

    [Fact]
    public async Task GetIssue_With_Watchers_And_Relations_Should_Succeed()
    {
        var createdIssue = await CreateTestIssueAsync(
        [
            IssueCustomField.CreateMultiple(1, ThreadSafeRandom.GenerateText(8), 
                [ThreadSafeRandom.GenerateText(4), ThreadSafeRandom.GenerateText(4)]) 
        ],
    [new Watcher() { Id = 1 }, new Watcher(){Id = 2}]);
        
        Assert.NotNull(createdIssue);
        
        //Act
        var retrievedIssue = await fixture.RedmineManager.GetAsync<Issue>(createdIssue.Id.ToInvariantString(), 
            RequestOptions.Include($"{Include.Issue.Watchers},{Include.Issue.Relations}"));

        //Assert
        Assert.NotNull(retrievedIssue);
    }
}