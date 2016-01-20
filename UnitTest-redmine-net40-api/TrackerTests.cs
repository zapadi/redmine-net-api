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
    public class TrackerTests
    {
        private RedmineManager redmineManager;

        private const int NUMBER_OF_TRACKERS = 3;

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
        public void RedmineTrackers_ShouldGetAllTrackers()
        {
            var trackers = redmineManager.GetObjects<Tracker>(null);

            Assert.IsNotNull(trackers, "Get all trackers returned null");
            Assert.IsTrue(trackers.Count == NUMBER_OF_TRACKERS, "Trackers count != " + NUMBER_OF_TRACKERS);
            CollectionAssert.AllItemsAreNotNull(trackers, "Trackers list contains null items.");
            CollectionAssert.AllItemsAreUnique(trackers, "Trackers items are not unique.");
            CollectionAssert.AllItemsAreInstancesOfType(trackers, typeof(Tracker), "Not all items are of type Tracker.");
 
        }
    }
}
