using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class TimeEntryTestsAsync(RedmineTestContainerFixture fixture)
{
    private async Task<TimeEntry> CreateTestTimeEntryAsync()
    {
        var project = await fixture.RedmineManager.GetAsync<Project>(1.ToInvariantString());
        var issue = await fixture.RedmineManager.GetAsync<Issue>(1.ToInvariantString());
        
        var timeEntry = new TimeEntry
        {
            Project = project,
            Issue = issue.ToIdentifiableName(),
            SpentOn = DateTime.Now.Date,
            Hours = 1.5m,
            Activity = 8.ToIdentifier(),
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
        var createdTimeEntry = await CreateTestTimeEntryAsync();
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
        var createdTimeEntry = await CreateTestTimeEntryAsync();
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
        var createdTimeEntry = await CreateTestTimeEntryAsync();
        Assert.NotNull(createdTimeEntry);

        var timeEntryId = createdTimeEntry.Id.ToInvariantString();

        //Act
        await fixture.RedmineManager.DeleteAsync<TimeEntry>(timeEntryId);

        //Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await fixture.RedmineManager.GetAsync<TimeEntry>(timeEntryId));
    }
}