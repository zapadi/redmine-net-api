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
        #region Constants
        private const string projectId = "6";
        private const int numberOfVersions = 3;

        //version data - used for create
        private const string newVersionName = "Test version";
        private const VersionStatus newVersionStatus = VersionStatus.locked;
        private const VersionSharing newVersionSharing = VersionSharing.hierarchy;
        private DateTime newVersionDueDate = DateTime.Now.AddDays(7);
        private const string newVersionDescription = "Version description";

        private const string versionId = "15";

        //version data - used for update 
        private const string updatedVersionId = "14";
        private const string updatedVersionName = "Updated version";
        private const VersionStatus updatedVersionStatus = VersionStatus.closed;
        private const VersionSharing updatedVersionSharing = VersionSharing.system;
        private DateTime updatedVersionDueDate = DateTime.Now.AddMonths(1);
        private const string updatedVersionDescription = "Updated description";

        private const string deletedVersionId = "14";
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
        public void RedmineProjectVersions_ShouldGetSpecificProjectVersions()
        {
            var versions = redmineManager.GetObjectList<Redmine.Net.Api.Types.Version>(new NameValueCollection { { "project_id", projectId } });
        
            Assert.IsTrue(versions.Count == numberOfVersions);
        }

        [TestMethod]
        public void RedmineProjectVersion_ShouldCreateNewVersion()
        {
            Redmine.Net.Api.Types.Version version = new Redmine.Net.Api.Types.Version();
            version.Name = newVersionName;
            version.Status = newVersionStatus;
            version.Sharing = newVersionSharing;
            version.DueDate = newVersionDueDate;
            version.Description = newVersionDescription;

            Redmine.Net.Api.Types.Version savedVersion = redmineManager.CreateObject<Redmine.Net.Api.Types.Version>(version, projectId);

            Assert.AreEqual(version.Name, savedVersion.Name);
        }

        [TestMethod]
        public void RedmineProjectVersion_ShouldGetVersionById()
        {
            Redmine.Net.Api.Types.Version version = redmineManager.GetObject<Redmine.Net.Api.Types.Version>(versionId, null);

            Assert.IsNotNull(version);
        }

        [TestMethod]
        public void RedmineProjectVersion_ShouldUpdateVersion()
        {
            Redmine.Net.Api.Types.Version version = redmineManager.GetObject<Redmine.Net.Api.Types.Version>(updatedVersionId, null);
            version.Name = updatedVersionName;
            version.Status = updatedVersionStatus;
            version.Sharing = updatedVersionSharing;
            version.DueDate = updatedVersionDueDate;
            version.Description = updatedVersionDescription;

            redmineManager.UpdateObject<Redmine.Net.Api.Types.Version>(updatedVersionId, version);

            Redmine.Net.Api.Types.Version updatedVersion = redmineManager.GetObject<Redmine.Net.Api.Types.Version>(updatedVersionId, null);

            Assert.AreEqual(version.Name, updatedVersion.Name);
        }

        [TestMethod]
        public void RedmineProjectVersion_ShouldDeleteVersion()
        {
            try
            {
                redmineManager.DeleteObject<Redmine.Net.Api.Types.Version>(deletedVersionId, null);
            }
            catch (RedmineException)
            {
                Assert.Fail("Version could not be deleted.");
                return;
            }

            try
            {
                Redmine.Net.Api.Types.Version version = redmineManager.GetObject<Redmine.Net.Api.Types.Version>(deletedVersionId, null);
            }
            catch (RedmineException exc)
            {
                StringAssert.Contains(exc.Message, "Not Found");
                return;
            }
            Assert.Fail("Test failed");

        }

        [TestMethod]
        public void RedmineProjectVersion_ShouldCompare()
        {
            Redmine.Net.Api.Types.Version version = redmineManager.GetObject<Redmine.Net.Api.Types.Version>(versionId, null);
            Redmine.Net.Api.Types.Version versionToCompare = redmineManager.GetObject<Redmine.Net.Api.Types.Version>(versionId, null);

            Assert.IsTrue(version.Equals(versionToCompare));
        }
        #endregion Tests
    }
}
