using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Common;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.TimeEntry;

[Collection(Constants.RedmineTestContainerCollection)]
public class TimeEntryTestsAsync(RedmineTestContainerFixture fixture)
{
    private async Task<(Redmine.Net.Api.Types.TimeEntry, Redmine.Net.Api.Types.TimeEntry payload)> CreateRandomTestTimeEntryAsync()
    {
        var (issue, _)  = await IssueTestHelper.CreateRandomIssueAsync(fixture.RedmineManager);
        
        var timeEntry = TestEntityFactory.CreateRandomTimeEntryPayload(TestConstants.Projects.DefaultProjectId, issue.Id, activityId: 8);
        return (await fixture.RedmineManager.CreateAsync(timeEntry), timeEntry);
    }

    [Fact]
    public async Task CreateTimeEntry_Should_Succeed()
    {
        //Arrange & Act
        var (timeEntry, timeEntryPayload) = await CreateRandomTestTimeEntryAsync();

        //Assert
        Assert.NotNull(timeEntry);
        Assert.True(timeEntry.Id > 0);
        Assert.Equal(timeEntryPayload.Hours, timeEntry.Hours);
        Assert.Equal(timeEntryPayload.Comments, timeEntry.Comments);
        Assert.Equal(timeEntryPayload.Project.Id, timeEntry.Project.Id);
        Assert.Equal(timeEntryPayload.Issue.Id, timeEntry.Issue.Id);
        Assert.Equal(timeEntryPayload.Activity.Id, timeEntry.Activity.Id);
    }

    [Fact]
    public async Task GetTimeEntry_Should_Succeed()
    {
        //Arrange
        var (createdTimeEntry,_) = await CreateRandomTestTimeEntryAsync();
        Assert.NotNull(createdTimeEntry);

        //Act
        var retrievedTimeEntry = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.TimeEntry>(createdTimeEntry.Id.ToInvariantString());

        //Assert
        Assert.NotNull(retrievedTimeEntry);
        Assert.Equal(createdTimeEntry.Id, retrievedTimeEntry.Id);
        Assert.Equal(createdTimeEntry.Hours, retrievedTimeEntry.Hours);
        Assert.Equal(createdTimeEntry.Comments, retrievedTimeEntry.Comments);
    }

    [Fact]
    public async Task UpdateTimeEntry_Should_Succeed()
    {
        //Arrange
        var (createdTimeEntry,_) = await CreateRandomTestTimeEntryAsync();
        Assert.NotNull(createdTimeEntry);

        var updatedComments = $"Updated test time entry comments {Guid.NewGuid()}";
        var updatedHours = 2.5m;
        createdTimeEntry.Comments = updatedComments;
        createdTimeEntry.Hours = updatedHours;

        //Act
        await fixture.RedmineManager.UpdateAsync(createdTimeEntry.Id.ToInvariantString(), createdTimeEntry);
        var retrievedTimeEntry = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.TimeEntry>(createdTimeEntry.Id.ToInvariantString());

        //Assert
        Assert.NotNull(retrievedTimeEntry);
        Assert.Equal(createdTimeEntry.Id, retrievedTimeEntry.Id);
        Assert.Equal(updatedComments, retrievedTimeEntry.Comments);
        Assert.Equal(updatedHours, retrievedTimeEntry.Hours);
    }

    [Fact]
    public async Task DeleteTimeEntry_Should_Succeed()
    {
        //Arrange
        var (createdTimeEntry,_) = await CreateRandomTestTimeEntryAsync();
        Assert.NotNull(createdTimeEntry);

        var timeEntryId = createdTimeEntry.Id.ToInvariantString();

        //Act
        await fixture.RedmineManager.DeleteAsync<Redmine.Net.Api.Types.TimeEntry>(timeEntryId);

        //Assert
        await Assert.ThrowsAsync<RedmineNotFoundException>(async () => await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.TimeEntry>(timeEntryId));
    }
}