using System;
using Xunit;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using Redmine.Net.Api.Exceptions;

namespace xUnitTestredminenet45api
{
	[Collection("RedmineCollection")]
	public class TimeEntryTests
	{
		//timeEntryData - used for create
		private const int NEW_TIME_ENTRY_ISSUE_ID = 18;
		private const int NEW_TIME_ENTRY_PROJECT_ID = 9;
		private DateTime NEW_TIME_ENTRY_DATE = DateTime.Now;
		private const int NEW_TIME_ENTRY_HOURS = 1;
		private const int NEW_TIME_ENTRY_ACTIVITY_ID = 16;
		private const string NEW_TIME_ENTRY_COMMENTS = "Added time entry on project";

		private const string TIME_ENTRY_ID = "30";

		//timeEntryData - used for update
		private const string UPDATED_TIME_ENTRY_ID = "31";
		private const int UPDATED_TIME_ENTRY_ISSUE_ID = 18;
		private const int UPDATED_TIME_ENTRY_PROJECT_ID = 9;
		private DateTime UPDATED_TIME_ENTRY_DATE = DateTime.Now.AddDays(-2);
		private const int UPDATED_TIME_ENTRY_HOURS = 3;
		private const int UPDATED_TIME_ENTRY_ACTIVITY_ID = 17;
		private const string UPDATED_TIME_ENTRY_COMMENTS = "Time entry updated";

		private const string DELETED_TIME_ENTRY_ID = "43";

		RedmineFixture fixture;
		public TimeEntryTests (RedmineFixture fixture)
		{
			this.fixture = fixture;
		}

		[Fact]
		public void Should_Get_All_Time_Entries()
		{
			var timeEntries = fixture.redmineManager.GetObjects<TimeEntry>(null);

			Assert.NotNull(timeEntries);
			Assert.NotEmpty (timeEntries);
			Assert.All (timeEntries, t => Assert.IsType<TimeEntry> (t));
		}

		[Fact]
		public void Should_Create_Time_Entry()
		{
			TimeEntry timeEntry = new TimeEntry();
			timeEntry.Issue = new IdentifiableName { Id = NEW_TIME_ENTRY_ISSUE_ID };
			timeEntry.Project = new IdentifiableName { Id = NEW_TIME_ENTRY_PROJECT_ID };
			timeEntry.SpentOn = NEW_TIME_ENTRY_DATE;
			timeEntry.Hours = NEW_TIME_ENTRY_HOURS;
			timeEntry.Activity = new IdentifiableName { Id = NEW_TIME_ENTRY_ACTIVITY_ID };
			timeEntry.Comments = NEW_TIME_ENTRY_COMMENTS;

			TimeEntry savedTimeEntry = fixture.redmineManager.CreateObject<TimeEntry>(timeEntry);

			Assert.NotNull (savedTimeEntry);
			Assert.NotNull(savedTimeEntry.Issue);
			Assert.True(savedTimeEntry.Issue.Id == NEW_TIME_ENTRY_ISSUE_ID, "Issue id is invalid.");
			Assert.NotNull(savedTimeEntry.Project);
			Assert.True(savedTimeEntry.Project.Id == NEW_TIME_ENTRY_PROJECT_ID, "Project id is invalid.");
			Assert.NotNull(savedTimeEntry.SpentOn);
			Assert.True(DateTime.Compare(savedTimeEntry.SpentOn.Value.Date, NEW_TIME_ENTRY_DATE.Date) ==0, "Date is invalid.");
			Assert.NotNull(savedTimeEntry.Hours);
			Assert.True(savedTimeEntry.Hours == NEW_TIME_ENTRY_HOURS, "Hours value is not valid.");
			Assert.NotNull(savedTimeEntry.Activity);
			Assert.True(savedTimeEntry.Activity.Id == NEW_TIME_ENTRY_ACTIVITY_ID, "Activity id is invalid.");
			Assert.NotNull(savedTimeEntry.Comments);
			Assert.True(savedTimeEntry.Comments.Equals(NEW_TIME_ENTRY_COMMENTS), "Coments value is invalid.");
		}

		[Fact]
		public void Should_Get_Time_Entry_By_Id()
		{
			var timeEntry = fixture.redmineManager.GetObject<TimeEntry>(TIME_ENTRY_ID, null);

			Assert.NotNull(timeEntry);
			Assert.IsType<TimeEntry>(timeEntry);
			Assert.NotNull(timeEntry.Project);
			Assert.NotNull(timeEntry.SpentOn);
			Assert.NotNull(timeEntry.Hours);
			Assert.NotNull(timeEntry.Activity);
		}

		[Fact]
		public void Should_Compare_Time_Entries()
		{
			var timeEntry = fixture.redmineManager.GetObject<TimeEntry>(TIME_ENTRY_ID, null);
			var timeEntryToCompare = fixture.redmineManager.GetObject<TimeEntry>(TIME_ENTRY_ID, null);

			Assert.NotNull(timeEntry);
			Assert.True(timeEntry.Equals(timeEntryToCompare), "Time entries are not equal.");
		}

		[Fact]
		public void Should_Update_Time_Entry()
		{
			var timeEntry = fixture.redmineManager.GetObject<TimeEntry>(UPDATED_TIME_ENTRY_ID, null);
			timeEntry.Project.Id = UPDATED_TIME_ENTRY_PROJECT_ID;
			timeEntry.Issue.Id = UPDATED_TIME_ENTRY_ISSUE_ID;
			timeEntry.SpentOn = UPDATED_TIME_ENTRY_DATE;
			timeEntry.Hours = UPDATED_TIME_ENTRY_HOURS;
			timeEntry.Comments = UPDATED_TIME_ENTRY_COMMENTS;

			if (timeEntry.Activity == null) timeEntry.Activity = new IdentifiableName();
			timeEntry.Activity.Id = UPDATED_TIME_ENTRY_ACTIVITY_ID;

			fixture.redmineManager.UpdateObject<TimeEntry>(UPDATED_TIME_ENTRY_ID, timeEntry);

			var updatedTimeEntry = fixture.redmineManager.GetObject<TimeEntry>(UPDATED_TIME_ENTRY_ID, null);

			Assert.NotNull(updatedTimeEntry);
			Assert.True (updatedTimeEntry.Project.Id == timeEntry.Project.Id, "Time entry project was not updated.");
			Assert.True (updatedTimeEntry.Issue.Id == timeEntry.Issue.Id, "Time entry issue was not updated.");
			Assert.True (updatedTimeEntry.SpentOn != null && timeEntry.SpentOn != null && DateTime.Compare(updatedTimeEntry.SpentOn.Value.Date, timeEntry.SpentOn.Value.Date) == 0, "Time entry spent on field was not updated.");
			Assert.True (updatedTimeEntry.Hours == timeEntry.Hours, "Time entry hours was not updated.");
			Assert.True (updatedTimeEntry.Comments.Equals(timeEntry.Comments), "Time entry comments was not updated.");
		}

		[Fact]
		public void Should_Delete_Time_Entry()
		{
			RedmineException exception = (RedmineException)Record.Exception(() => fixture.redmineManager.DeleteObject<TimeEntry>(DELETED_TIME_ENTRY_ID, null));
			Assert.Null (exception);
			Assert.Throws<NotFoundException>(() => fixture.redmineManager.GetObject<TimeEntry>(DELETED_TIME_ENTRY_ID, null));
		}
	}
}

