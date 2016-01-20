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
    public class TimeEntryActivityTests
    {
        private RedmineManager redmineManager;

        private const int NUMBER_OF_TIME_ENTRY_ACTIVITIES = 4;
        private const bool EXISTS_DEFAULT_TIME_ENTRY_ACTIVITIES = true;

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
        public void Should_Get_All_TimeEntryActivities()
        {
            var timeEntryActivities = redmineManager.GetObjects<TimeEntryActivity>(null);

            Assert.IsNotNull(timeEntryActivities, "Get all time entry activities returned null");
            Assert.IsTrue(timeEntryActivities.Count == NUMBER_OF_TIME_ENTRY_ACTIVITIES, "Time entry activities count != " + NUMBER_OF_TIME_ENTRY_ACTIVITIES);
            CollectionAssert.AllItemsAreNotNull(timeEntryActivities, "Time entry activities list contains null items.");
            CollectionAssert.AllItemsAreUnique(timeEntryActivities, "Time entry activities items are not unique.");
            CollectionAssert.AllItemsAreInstancesOfType(timeEntryActivities, typeof(TimeEntryActivity), "Not all items are of type TimeEntryActivity.");
            Assert.IsTrue(timeEntryActivities.Exists(tea => tea.IsDefault) == EXISTS_DEFAULT_TIME_ENTRY_ACTIVITIES, EXISTS_DEFAULT_TIME_ENTRY_ACTIVITIES ? "Default time entry activity was expected to exist." : "Default time entry antivity was not expected to exist.");
        }
    }
}
