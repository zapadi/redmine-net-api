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

namespace UnitTestRedmineNetApi
{
    [TestClass]
    public class TimeEntryTests
    {
        #region Constants
        private const string timeEntryId = "19";

        //timeEntryData - used for create
        private const int newTimeEntryIssueId = 19;
        private const int newTimeEntryProjectId = 10;
        private DateTime newTimeEntryDate = DateTime.Now;
        private const int newTimeEntryHours = 1;
        private const int newTimeEntryActivityId = 16;
        private const string newTimeEntryComments = "Added time entry on project";

        //timeEntryData - used for update
        private const string updatedTimeEntryId = "28";
        private const int updatedTimeEntryIssueId = 20;
        private const int updatedTimeEntryProjectId = 10;
        private DateTime updatedTimeEntryDate = DateTime.Now.AddDays(-2);
        private const int updatedTimeEntryHours = 3;
        private const int updatedTimeEntryActivityId = 17;
        private const string updatedTimeEntryComments = "Time entry updated";

        private const string deletedTimeEntryId = "28";
        #endregion Constants

        #region Properties
        private RedmineManager redmineManager;
        private string uri;
        private string apiKey;
        #endregion Properties

        #region Initialize
        [TestInitialize]
        public void Initialize()
        {
            uri = ConfigurationManager.AppSettings["uri"];
            apiKey = ConfigurationManager.AppSettings["apiKey"];

            SetMimeTypeJSON();
            SetMimeTypeXML();
        }

        [Conditional("JSON")]
        private void SetMimeTypeJSON()
        {
            redmineManager = new RedmineManager(uri, apiKey, MimeFormat.json);
        }

        [Conditional("XML")]
        private void SetMimeTypeXML()
        {
            redmineManager = new RedmineManager(uri, apiKey, MimeFormat.xml);
        }
        #endregion Initialize

        #region Tests
        [TestMethod]
        public void RedmineTimeEntries_ShouldGetAll()
        {
            var timeEntries = redmineManager.GetObjectList<TimeEntry>(null);

            Assert.IsNotNull(timeEntries);
        }

        [TestMethod]
        public void RedmineTimeEntries_ShouldGetEntityById()
        {
            var timeEntry = redmineManager.GetObject<TimeEntry>(timeEntryId, null);

            Assert.IsNotNull(timeEntry);
        }

        [TestMethod]
        public void RedmineTimeEntries_ShouldAdd()
        {
            TimeEntry timeEntry = new TimeEntry();
            timeEntry.Issue = new IdentifiableName { Id = newTimeEntryIssueId };
            timeEntry.Project = new IdentifiableName { Id = newTimeEntryProjectId };
            timeEntry.SpentOn = newTimeEntryDate;
            timeEntry.Hours = newTimeEntryHours;
            timeEntry.Activity = new IdentifiableName { Id = newTimeEntryActivityId };
            timeEntry.Comments = newTimeEntryComments;

            TimeEntry savedTimeEntry = redmineManager.CreateObject<TimeEntry>(timeEntry);

            Assert.AreEqual(timeEntry.Comments, savedTimeEntry.Comments);
        }

        [TestMethod]
        public void RedmineTimeEntries_ShouldUpdate()
        {
            var timeEntry = redmineManager.GetObject<TimeEntry>(updatedTimeEntryId, null);
            timeEntry.Project.Id = updatedTimeEntryProjectId;
            timeEntry.Issue.Id = updatedTimeEntryIssueId;
            timeEntry.SpentOn = updatedTimeEntryDate;
            timeEntry.Hours = updatedTimeEntryHours;
            timeEntry.Comments = updatedTimeEntryComments;
            timeEntry.Activity.Id = updatedTimeEntryActivityId;

            redmineManager.UpdateObject<TimeEntry>(updatedTimeEntryId, timeEntry);

            var updatedTimeEntry = redmineManager.GetObject<TimeEntry>(updatedTimeEntryId, null);

            Assert.AreEqual<TimeEntry>(timeEntry, updatedTimeEntry);
        }

        [TestMethod]
        public void RedmineTimeEntries_ShouldDelete()
        {
            try
            {
                redmineManager.DeleteObject<TimeEntry>(deletedTimeEntryId, null);
            }
            catch (RedmineException)
            {
                Assert.Fail("Time entry could not be deleted.");
                return;
            }

            try
            {
                TimeEntry timeEntry = redmineManager.GetObject<TimeEntry>(deletedTimeEntryId, null);
            }
            catch (RedmineException exc)
            {
                StringAssert.Contains(exc.Message, "Not Found");
                return;
            }
            Assert.Fail("Test failed");
        }

        [TestMethod]
        public void RedmineTimeEntries_ShouldCompare()
        {
            var timeEntry = redmineManager.GetObject<TimeEntry>(timeEntryId, null);
            var timeEntryToCompare = redmineManager.GetObject<TimeEntry>(timeEntryId, null);

            Assert.IsTrue(timeEntry.Equals(timeEntryToCompare));
        }
        #endregion Tests
    }
}
