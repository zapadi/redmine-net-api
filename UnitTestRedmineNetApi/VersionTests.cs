using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestRedmineNetApi
{
    [TestClass]
    public class VersionTests
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
        public void RedmineProjectVersions_ShouldGetSpecificProjectVersions()
        {
            int projectId = 6;

            var versions = redmineManager.GetObjectList<Redmine.Net.Api.Types.Version>(new NameValueCollection { { "project_id", projectId.ToString() } });
        
            Assert.IsTrue(versions.Count == 3);
        }

        [TestMethod]
        public void RedmineProjectVersion_ShouldCreateNewVersion()
        {
            Redmine.Net.Api.Types.Version version = new Redmine.Net.Api.Types.Version();
            version.Name = "Test version";
            version.Status = VersionStatus.locked;
            version.Sharing = VersionSharing.hierarchy;
            version.DueDate = DateTime.Now.AddDays(7);
            version.Description = "Version description";

            Redmine.Net.Api.Types.Version savedVersion = redmineManager.CreateObject<Redmine.Net.Api.Types.Version>(version, "6");

            Assert.AreEqual(version.Name, savedVersion.Name);
        }

        [TestMethod]
        public void RedmineProjectVersion_ShouldGetVersionById()
        {
            var versionId = "12";

            Redmine.Net.Api.Types.Version version = redmineManager.GetObject<Redmine.Net.Api.Types.Version>(versionId, null);

            Assert.IsNotNull(version);
        }

        [TestMethod]
        public void RedmineProjectVersion_ShouldUpdateVersion()
        {
            var versionId = "12";

            Redmine.Net.Api.Types.Version version = redmineManager.GetObject<Redmine.Net.Api.Types.Version>(versionId, null);
            version.Name = "Updated version";
            version.Status = VersionStatus.closed;
            version.Sharing = VersionSharing.system;
            version.DueDate = DateTime.Now.AddMonths(1);
            version.Description = "Updated description";

            redmineManager.UpdateObject<Redmine.Net.Api.Types.Version>(versionId, version);

            Redmine.Net.Api.Types.Version updatedVersion = redmineManager.GetObject<Redmine.Net.Api.Types.Version>(versionId, null);

            Assert.AreEqual(version.Name, updatedVersion.Name);
        }

        [TestMethod]
        public void RedmineProjectVersion_ShouldDeleteVersion()
        {
            var versionId = "12";

            try
            {
                redmineManager.DeleteObject<Redmine.Net.Api.Types.Version>(versionId, null);
            }
            catch (RedmineException)
            {
                Assert.Fail("Version could not be deleted.");
                return;
            }

            try
            {
                Redmine.Net.Api.Types.Version version = redmineManager.GetObject<Redmine.Net.Api.Types.Version>(versionId, null);
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
