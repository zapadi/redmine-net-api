using Padi.RedmineApi.Exceptions;
using Padi.RedmineApi.Extensions;
using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Padi.RedmineApi.Internals;
using Padi.RedmineApi.Types;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class TimeEntryTestsAsync(RedmineTestContainerFixture fixture)
{
    private async Task<TimeEntry> CreateRandomlyTestTimeEntryAsync(int? projectId = null, int? issueId = null, int? activityId = null)
    {
        if (!projectId.HasValue)
        {
            var project = await fixture.RedmineManager.GetAsync<Project>(1.ToInvariantString());
            projectId = project.Id;
        }

        if (!issueId.HasValue)
        {
            var issue = await fixture.RedmineManager.GetAsync<Issue>(1.ToInvariantString());
            issueId = issue.Id;
        }

        if (!activityId.HasValue)
        {
            //TODO: define the default activity or create one
            //await fixture.RedmineManager.CreateAsync(new Activity)
            activityId = 8;
        }

        var timeEntry = new TimeEntry
        {
            Project = projectId.Value.ToIdentifier(),
            Issue = issueId.Value.ToIdentifier(),
            SpentOn = DateTime.Now.Date,
            Hours = 1.5m,
            Activity = activityId.Value.ToIdentifier(),
            Comments = $"Test time entry comments {Guid.NewGuid()}",
        };
        return await fixture.RedmineManager.CreateAsync(timeEntry);
    }

    [Fact]
    public async Task CreateTimeEntry_Should_Succeed()
    {
        //Arrange
        var timeEntryData = new TimeEntry
        {
            Project = 1.ToIdentifier(),
            Issue = 1.ToIdentifier(),
            SpentOn = DateTime.Now.Date,
            Hours = 1.5m,
            Activity = 8.ToIdentifier(),
            Comments = $"Initial create test comments {Guid.NewGuid()}",
        };

        //Act
        var createdTimeEntry = await fixture.RedmineManager.CreateAsync(timeEntryData);

        //Assert
        Assert.NotNull(createdTimeEntry);
        Assert.True(createdTimeEntry.Id > 0);
        Assert.Equal(timeEntryData.Hours, createdTimeEntry.Hours);
        Assert.Equal(timeEntryData.Comments, createdTimeEntry.Comments);
        Assert.Equal(timeEntryData.Project.Id, createdTimeEntry.Project.Id);
        Assert.Equal(timeEntryData.Issue.Id, createdTimeEntry.Issue.Id);
        Assert.Equal(timeEntryData.Activity.Id, createdTimeEntry.Activity.Id);
    }

    [Fact]
    public async Task GetTimeEntry_Should_Succeed()
    {
        //Arrange
        var createdTimeEntry = await CreateRandomlyTestTimeEntryAsync();
        Assert.NotNull(createdTimeEntry);

        //Act
        var retrievedTimeEntry = await fixture.RedmineManager.GetAsync<TimeEntry>(createdTimeEntry.Id.ToInvariantString());

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
        var createdTimeEntry = await CreateRandomlyTestTimeEntryAsync();
        Assert.NotNull(createdTimeEntry);

        var updatedComments = $"Updated test time entry comments {Guid.NewGuid()}";
        var updatedHours = 2.5m;
        createdTimeEntry.Comments = updatedComments;
        createdTimeEntry.Hours = updatedHours;

        //Act
        await fixture.RedmineManager.UpdateAsync(createdTimeEntry.Id.ToInvariantString(), createdTimeEntry);
        var retrievedTimeEntry = await fixture.RedmineManager.GetAsync<TimeEntry>(createdTimeEntry.Id.ToInvariantString());

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
        var createdTimeEntry = await CreateRandomlyTestTimeEntryAsync(projectId: 1);
        Assert.NotNull(createdTimeEntry);

        var timeEntryId = createdTimeEntry.Id.ToInvariantString();

        //Act
        await fixture.RedmineManager.DeleteAsync<TimeEntry>(timeEntryId);

        //Assert
        var ex = await Assert.ThrowsAsync<RedmineApiException>(() => fixture.RedmineManager.GetAsync<TimeEntry>(timeEntryId));
        
        Assert.NotNull(ex);
        Assert.Equal(HttpConstants.StatusCodes.NotFound, ex.HttpStatusCode);
    }
}
