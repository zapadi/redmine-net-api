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
    public class TimeEntryActivityTests
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
        public void RedmineTimeEntryActivities_ShouldGetAllTimeEntryActivities()
        {
            var timeEntryActivities = redmineManager.GetObjectList<TimeEntryActivity>(null);

            Assert.IsTrue(timeEntryActivities.Count == 3);
        }
    }
}
