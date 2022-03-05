/*
   Copyright 2011 - 2022 Adrian Popescu

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using Padi.RedmineApi.Tests.Infrastructure;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineApi.Tests.Tests.Sync
{
	[Trait("Redmine-Net-Api", "TimeEntries")]
#if !(NET20 || NET40)
    [Collection("RedmineCollection")]
#endif
    public class TimeEntryTests
    {
        public TimeEntryTests(RedmineFixture fixture)
        {
            this.fixture = fixture;
        }

	    private readonly RedmineFixture fixture;

        [Fact, Order(1)]
        public void Should_Create_Time_Entry()
        {
	        const int NEW_TIME_ENTRY_ISSUE_ID = 18;
	        const int NEW_TIME_ENTRY_PROJECT_ID = 9;
	        var newTimeEntryDate = DateTime.Now;
	        const int NEW_TIME_ENTRY_HOURS = 1;
	        const int NEW_TIME_ENTRY_ACTIVITY_ID = 16;
	        const string NEW_TIME_ENTRY_COMMENTS = "Added time entry on project";

	        var timeEntry = new TimeEntry
            {
                Issue = IdentifiableName.Create<IdentifiableName>(NEW_TIME_ENTRY_ISSUE_ID),
                Project = IdentifiableName.Create<IdentifiableName>(NEW_TIME_ENTRY_PROJECT_ID),
                SpentOn = newTimeEntryDate,
                Hours = NEW_TIME_ENTRY_HOURS,
                Activity = IdentifiableName.Create<IdentifiableName>(NEW_TIME_ENTRY_ACTIVITY_ID),
                Comments = NEW_TIME_ENTRY_COMMENTS
            };

            var savedTimeEntry = fixture.RedmineManager.CreateObject(timeEntry);

            Assert.NotNull(savedTimeEntry);
            Assert.NotNull(savedTimeEntry.Issue);
            Assert.True(savedTimeEntry.Issue.Id == NEW_TIME_ENTRY_ISSUE_ID, "Issue id is invalid.");
            Assert.NotNull(savedTimeEntry.Project);
            Assert.True(savedTimeEntry.Project.Id == NEW_TIME_ENTRY_PROJECT_ID, "Project id is invalid.");
            Assert.NotNull(savedTimeEntry.SpentOn);
            Assert.True(DateTime.Compare(savedTimeEntry.SpentOn.Value.Date, newTimeEntryDate.Date) == 0,
                "Date is invalid.");
            Assert.True(savedTimeEntry.Hours == NEW_TIME_ENTRY_HOURS, "Hours value is not valid.");
            Assert.NotNull(savedTimeEntry.Activity);
            Assert.True(savedTimeEntry.Activity.Id == NEW_TIME_ENTRY_ACTIVITY_ID, "Activity id is invalid.");
            Assert.NotNull(savedTimeEntry.Comments);
            Assert.True(savedTimeEntry.Comments.Equals(NEW_TIME_ENTRY_COMMENTS), "Coments value is invalid.");
        }

        [Fact, Order(99)]
        public void Should_Delete_Time_Entry()
        {
	        const string DELETED_TIME_ENTRY_ID = "43";
	        var exception =
                (RedmineException)
                    Record.Exception(() => fixture.RedmineManager.DeleteObject<TimeEntry>(DELETED_TIME_ENTRY_ID));
            Assert.Null(exception);
            Assert.Throws<NotFoundException>(
                () => fixture.RedmineManager.GetObject<TimeEntry>(DELETED_TIME_ENTRY_ID, null));
        }

        [Fact, Order(2)]
        public void Should_Get_All_Time_Entries()
        {
            var timeEntries = fixture.RedmineManager.GetObjects<TimeEntry>();

            Assert.NotNull(timeEntries);
            Assert.NotEmpty(timeEntries);
        }

        [Fact, Order(3)]
        public void Should_Get_Time_Entry_By_Id()
        {
	        const string TIME_ENTRY_ID = "30";

	        var timeEntry = fixture.RedmineManager.GetObject<TimeEntry>(TIME_ENTRY_ID, null);

            Assert.NotNull(timeEntry);
            Assert.IsType<TimeEntry>(timeEntry);
            Assert.NotNull(timeEntry.Project);
            Assert.NotNull(timeEntry.SpentOn);
            Assert.NotNull(timeEntry.Activity);
        }

        [Fact, Order(4)]
        public void Should_Update_Time_Entry()
        {
	        const string UPDATED_TIME_ENTRY_ID = "31";
	        const int UPDATED_TIME_ENTRY_ISSUE_ID = 18;
	        const int UPDATED_TIME_ENTRY_PROJECT_ID = 9;
	        const int UPDATED_TIME_ENTRY_HOURS = 3;
	        const int UPDATED_TIME_ENTRY_ACTIVITY_ID = 17;
	        const string UPDATED_TIME_ENTRY_COMMENTS = "Time entry updated";
	        var updatedTimeEntryDate = DateTime.Now.AddDays(-2);

	        var timeEntry = fixture.RedmineManager.GetObject<TimeEntry>(UPDATED_TIME_ENTRY_ID, null);
            timeEntry.Project = IdentifiableName.Create<IdentifiableName>(UPDATED_TIME_ENTRY_PROJECT_ID);
            timeEntry.Issue = IdentifiableName.Create<IdentifiableName>(UPDATED_TIME_ENTRY_ISSUE_ID);
            timeEntry.SpentOn = updatedTimeEntryDate;
            timeEntry.Hours = UPDATED_TIME_ENTRY_HOURS;
            timeEntry.Comments = UPDATED_TIME_ENTRY_COMMENTS;

            if (timeEntry.Activity == null) timeEntry.Activity = IdentifiableName.Create<IdentifiableName>(UPDATED_TIME_ENTRY_ACTIVITY_ID);

            fixture.RedmineManager.UpdateObject(UPDATED_TIME_ENTRY_ID, timeEntry);

            var updatedTimeEntry = fixture.RedmineManager.GetObject<TimeEntry>(UPDATED_TIME_ENTRY_ID, null);

            Assert.NotNull(updatedTimeEntry);
            Assert.True(updatedTimeEntry.Project.Id == timeEntry.Project.Id, "Time entry project was not updated.");
            Assert.True(updatedTimeEntry.Issue.Id == timeEntry.Issue.Id, "Time entry issue was not updated.");
            Assert.True(
                updatedTimeEntry.SpentOn != null && timeEntry.SpentOn != null &&
                DateTime.Compare(updatedTimeEntry.SpentOn.Value.Date, timeEntry.SpentOn.Value.Date) == 0,
                "Time entry spent on field was not updated.");
            Assert.True(updatedTimeEntry.Hours == timeEntry.Hours, "Time entry hours was not updated.");
            Assert.True(updatedTimeEntry.Comments.Equals(timeEntry.Comments), "Time entry comments was not updated.");
        }
    }
}