using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest_redmine_net40_api
{
    [TestClass]
    public class TimeEntryTests
    {
        private RedmineManager redmineManager;

        //timeEntryData - used for create
        private const int NEW_TIME_ENTRY_ISSUE_ID = 1;
        private const int NEW_TIME_ENTRY_PROJECT_ID = 1;
        private DateTime NEW_TIME_ENTRY_DATE = DateTime.Now;
        private const int NEW_TIME_ENTRY_HOURS = 1;
        private const int NEW_TIME_ENTRY_ACTIVITY_ID = 9;
        private const string NEW_TIME_ENTRY_COMMENTS = "Added time entry on project";

        private const string TIME_ENTRY_ID = "5";

        //timeEntryData - used for update
        private const string UPDATED_TIME_ENTRY_ID = "4";
        private const int UPDATED_TIME_ENTRY_ISSUE_ID = 2;
        private const int UPDATED_TIME_ENTRY_PROJECT_ID = 1;
        private DateTime UPDATED_TIME_ENTRY_DATE = DateTime.Now.AddDays(-2);
        private const int UPDATED_TIME_ENTRY_HOURS = 3;
        private const int UPDATED_TIME_ENTRY_ACTIVITY_ID = 8;
        private const string UPDATED_TIME_ENTRY_COMMENTS = "Time entry updated";

        private const string DELETED_TIME_ENTRY_ID = "4";
 
        [TestInitialize]
        public void Initialize()
        {
            SetMimeTypeJSON();
            SetMimeTypeXML();
        }

        [Conditional("JSON")]
        private void SetMimeTypeJSON()
        {
            redmineManager = new RedmineManager(Helper.Uri, Helper.ApiKey, MimeFormat.json);
        }

        [Conditional("XML")]
        private void SetMimeTypeXML()
        {
            redmineManager = new RedmineManager(Helper.Uri, Helper.ApiKey, MimeFormat.xml);
        }
     
        [TestMethod]
        public void Should_Get_All_Time_Entries()
        {
            var timeEntries = redmineManager.GetObjects<TimeEntry>(null);

            Assert.IsNotNull(timeEntries, "Get all time entries returned null");
            CollectionAssert.AllItemsAreNotNull(timeEntries, "Time entries list contains null items.");
            CollectionAssert.AllItemsAreUnique(timeEntries, "Time entries items are not unique.");
            CollectionAssert.AllItemsAreInstancesOfType(timeEntries, typeof(TimeEntry), "Not all items are of type TimeEntry.");
        }

        [TestMethod]
        public void Should_Create_Time_Entry()
        {
            TimeEntry timeEntry = new TimeEntry();
            timeEntry.Issue = new IdentifiableName { Id = NEW_TIME_ENTRY_ISSUE_ID };
            timeEntry.Project = new IdentifiableName { Id = NEW_TIME_ENTRY_PROJECT_ID };
            timeEntry.SpentOn = NEW_TIME_ENTRY_DATE;
            timeEntry.Hours = NEW_TIME_ENTRY_HOURS;
            timeEntry.Activity = new IdentifiableName { Id = NEW_TIME_ENTRY_ACTIVITY_ID };
            timeEntry.Comments = NEW_TIME_ENTRY_COMMENTS;

            TimeEntry savedTimeEntry = redmineManager.CreateObject<TimeEntry>(timeEntry);

            Assert.IsNotNull(savedTimeEntry, "Create time entry returned null.");
            Assert.IsNotNull(savedTimeEntry.Issue, "Saved time entry issue is null.");
            Assert.AreEqual(savedTimeEntry.Issue.Id, NEW_TIME_ENTRY_ISSUE_ID, "Issue id is invalid.");
            Assert.IsNotNull(savedTimeEntry.Project, "Saved time entry project is null.");
            Assert.AreEqual(savedTimeEntry.Project.Id, NEW_TIME_ENTRY_PROJECT_ID, "Project id is invalid.");
            Assert.IsNotNull(savedTimeEntry.SpentOn, "Saved time entry date is null.");
            Assert.AreEqual(savedTimeEntry.SpentOn.Value.Date, NEW_TIME_ENTRY_DATE.Date, "Date is invalid.");
            Assert.IsNotNull(savedTimeEntry.Hours, "Saved time entry hours is null.");
            Assert.AreEqual(savedTimeEntry.Hours, NEW_TIME_ENTRY_HOURS, "Hours value is not valid.");
            Assert.IsNotNull(savedTimeEntry.Activity, "Saved time entry activity is null.");
            Assert.AreEqual(savedTimeEntry.Activity.Id, NEW_TIME_ENTRY_ACTIVITY_ID, "Activity id is invalid.");
            Assert.IsNotNull(savedTimeEntry.Comments, "Saved time entry comment is null.");
            Assert.AreEqual(savedTimeEntry.Comments, NEW_TIME_ENTRY_COMMENTS, "Coments value is invalid.");
        }

        [TestMethod]
        public void Should_Get_Time_Entry_By_Id()
        {
            var timeEntry = redmineManager.GetObject<TimeEntry>(TIME_ENTRY_ID, null);

            Assert.IsNotNull(timeEntry, "Get object returned null.");
            Assert.IsInstanceOfType(timeEntry, typeof(TimeEntry), "Returned object is not of type TimeEntry.");
            Assert.IsNotNull(timeEntry.Project, "Project is null.");
            Assert.IsNotNull(timeEntry.SpentOn, "Spent on date is null.");
            Assert.IsNotNull(timeEntry.Hours, "Hours is null.");
            Assert.IsNotNull(timeEntry.Activity, "Activity is null.");
        }

        [TestMethod]
        public void Should_Compare_Time_Entries()
        {
            var timeEntry = redmineManager.GetObject<TimeEntry>(TIME_ENTRY_ID, null);
            var timeEntryToCompare = redmineManager.GetObject<TimeEntry>(TIME_ENTRY_ID, null);

            Assert.IsNotNull(timeEntry, "Time entry is null.");
            Assert.IsTrue(timeEntry.Equals(timeEntryToCompare), "Time entries are not equal.");
        }

        [TestMethod]
        public void Should_Update_Time_Entry()
        {
            var timeEntry = redmineManager.GetObject<TimeEntry>(UPDATED_TIME_ENTRY_ID, null);
            timeEntry.Project.Id = UPDATED_TIME_ENTRY_PROJECT_ID;
            timeEntry.Issue.Id = UPDATED_TIME_ENTRY_ISSUE_ID;
            timeEntry.SpentOn = UPDATED_TIME_ENTRY_DATE;
            timeEntry.Hours = UPDATED_TIME_ENTRY_HOURS;
            timeEntry.Comments = UPDATED_TIME_ENTRY_COMMENTS;

            if (timeEntry.Activity == null) timeEntry.Activity = new IdentifiableName();
            timeEntry.Activity.Id = UPDATED_TIME_ENTRY_ACTIVITY_ID;

            redmineManager.UpdateObject<TimeEntry>(UPDATED_TIME_ENTRY_ID, timeEntry);

            var updatedTimeEntry = redmineManager.GetObject<TimeEntry>(UPDATED_TIME_ENTRY_ID, null);

            Assert.IsNotNull(updatedTimeEntry, "Updated time entry is null.");
            Assert.AreEqual<TimeEntry>(timeEntry, updatedTimeEntry, "Time entry was not properly updated.");
        }

        [TestMethod]
        public void Should_Delete_Time_Entry()
        {
            try
            {
                redmineManager.DeleteObject<TimeEntry>(DELETED_TIME_ENTRY_ID, null);
            }
            catch (RedmineException)
            {
                Assert.Fail("Time entry could not be deleted.");
                return;
            }

            try
            {
                TimeEntry timeEntry = redmineManager.GetObject<TimeEntry>(DELETED_TIME_ENTRY_ID, null);
            }
            catch (RedmineException exc)
            {
                StringAssert.Contains(exc.Message, "Not Found");
                return;
            }
            Assert.Fail("Test failed");
        }
    }
}
