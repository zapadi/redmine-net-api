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
        private RedmineManager redmineManager;
        private string uri;
        private string apiKey;

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

        [TestMethod]
        public void RedmineTimeEntries_ShouldGetAll()
        {
            var timeEntries = redmineManager.GetObjectList<TimeEntry>(null);

            Assert.IsNotNull(timeEntries);
        }

        [TestMethod]
        public void RedmineTimeEntries_ShouldGetEntityById()
        {
            var timeEntryId = "19";

            var timeEntry = redmineManager.GetObject<TimeEntry>(timeEntryId, null);

            Assert.IsNotNull(timeEntry);
        }

        [TestMethod]
        public void RedmineTimeEntries_ShouldAdd()
        {
            TimeEntry timeEntry = new TimeEntry();
            timeEntry.Issue = new IdentifiableName { Id = 19 };
            timeEntry.Project = new IdentifiableName { Id = 10 };
            timeEntry.SpentOn = DateTime.Now;
            timeEntry.Hours = 1;
            timeEntry.Activity = new IdentifiableName { Id = 16 };
            timeEntry.Comments = "Added time entry on project";

            TimeEntry savedTimeEntry = redmineManager.CreateObject<TimeEntry>(timeEntry);

            Assert.AreEqual(timeEntry.Comments, savedTimeEntry.Comments);
        }

        [TestMethod]
        public void RedmineTimeEntries_ShouldUpdate()
        {
            var timeEntryId = "26";

            var timeEntry = redmineManager.GetObject<TimeEntry>(timeEntryId, null);
            timeEntry.Project.Id = 10;
            timeEntry.Issue.Id = 20;
            timeEntry.SpentOn = DateTime.Now.AddDays(-2);
            timeEntry.Hours = 3;
            timeEntry.Comments = "Time entry updated";
            timeEntry.Activity.Id = 17;

            redmineManager.UpdateObject<TimeEntry>(timeEntryId, timeEntry);

            var updatedTimeEntry = redmineManager.GetObject<TimeEntry>(timeEntryId, null);

            Assert.AreEqual<TimeEntry>(timeEntry, updatedTimeEntry);
        }

        [TestMethod]
        public void RedmineTimeEntries_ShouldDelete()
        {
            var timeEntryId = "26";

            try
            {
                redmineManager.DeleteObject<TimeEntry>(timeEntryId, null);
            }
            catch (RedmineException)
            {
                Assert.Fail("Time entry could not be deleted.");
                return;
            }

            try
            {
                TimeEntry timeEntry = redmineManager.GetObject<TimeEntry>(timeEntryId, null);
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
