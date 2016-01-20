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

namespace UnitTest_redmine_net40_api
{
    [TestClass]
    public class VersionTests
    {
        private RedmineManager redmineManager;

        private const string PROJECT_ID = "redmine-test";
        private const int NUMBER_OF_VERSIONS = 1;

        //version data - used for create
        private const string NEW_VERSION_NAME = "Test version";
        private const VersionStatus NEW_VERSION_STATUS = VersionStatus.locked;
        private const VersionSharing NEW_VERSION_SHARING = VersionSharing.hierarchy;
        private DateTime NEW_VERSION_DUE_DATE = DateTime.Now.AddDays(7);
        private const string NEW_VERSION_DESCRIPTION = "Version description";

        private const string VERSION_ID = "1";

        //version data - used for update 
        private const string UPDATED_VERSION_ID = "2";
        private const string UPDATED_VERSION_NAME = "Updated version";
        private const VersionStatus UPDATED_VERSION_STATUS = VersionStatus.closed;
        private const VersionSharing UPDATED_VERSION_SHARING = VersionSharing.system;
        private DateTime UPDATED_VERSION_DUE_DATE = DateTime.Now.AddMonths(1);
        private const string UPDATED_VERSION_DESCRIPTION = "Updated description";

        private const string DELETED_VERSION_ID = "2";
   
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
        public void Should_Get_Versions_By_Project_Id()
        {
            var versions = redmineManager.GetObjects<Redmine.Net.Api.Types.Version>(new NameValueCollection { { RedmineKeys.PROJECT_ID, PROJECT_ID } });

            Assert.IsNotNull(versions, "Get versions returned null");
            Assert.IsTrue(versions.Count == NUMBER_OF_VERSIONS, "Versions count != " + NUMBER_OF_VERSIONS);
            CollectionAssert.AllItemsAreNotNull(versions, "Versions list contains null items.");
            CollectionAssert.AllItemsAreUnique(versions, "Versions items are not unique.");
            CollectionAssert.AllItemsAreInstancesOfType(versions, typeof(Redmine.Net.Api.Types.Version), "Not all items are of type Version.");
        }

        [TestMethod]
        public void Should_Create_Version()
        {
            Redmine.Net.Api.Types.Version version = new Redmine.Net.Api.Types.Version();
            version.Name = NEW_VERSION_NAME;
            version.Status = NEW_VERSION_STATUS;
            version.Sharing = NEW_VERSION_SHARING;
            version.DueDate = NEW_VERSION_DUE_DATE;
            version.Description = NEW_VERSION_DESCRIPTION;

            Redmine.Net.Api.Types.Version savedVersion = redmineManager.CreateObject<Redmine.Net.Api.Types.Version>(version, PROJECT_ID);

            Assert.IsNotNull(savedVersion, "Create version returned null.");
            Assert.IsNotNull(savedVersion.Project, "Project is null.");
            Assert.AreEqual(savedVersion.Name, NEW_VERSION_NAME, "Version name is invalid.");
            Assert.AreEqual(savedVersion.Status, NEW_VERSION_STATUS, "Version status is invalid.");
            Assert.AreEqual(savedVersion.Sharing, NEW_VERSION_SHARING, "Version sharing is invalid.");
            Assert.IsNotNull(savedVersion.DueDate, "Due date is null.");
            Assert.AreEqual(savedVersion.DueDate.Value.Date, NEW_VERSION_DUE_DATE.Date, "Version due date is invalid.");
            Assert.AreEqual(savedVersion.Description, NEW_VERSION_DESCRIPTION, "Version description is invalid.");
        }

        [TestMethod]
        public void Should_Get_Version_By_Id()
        {
            Redmine.Net.Api.Types.Version version = redmineManager.GetObject<Redmine.Net.Api.Types.Version>(VERSION_ID, null);

            Assert.IsNotNull(version, "Get version returned null.");
        }

        [TestMethod]
        public void Should_Compare_Versions()
        {
            Redmine.Net.Api.Types.Version version = redmineManager.GetObject<Redmine.Net.Api.Types.Version>(VERSION_ID, null);
            Redmine.Net.Api.Types.Version versionToCompare = redmineManager.GetObject<Redmine.Net.Api.Types.Version>(VERSION_ID, null);

            Assert.IsNotNull(version, "Version is null.");
            Assert.IsTrue(version.Equals(versionToCompare), "Versions are not equal.");

        }

        [TestMethod]
        public void Should_Update_Version()
        {
            Redmine.Net.Api.Types.Version version = redmineManager.GetObject<Redmine.Net.Api.Types.Version>(UPDATED_VERSION_ID, null);
            version.Name = UPDATED_VERSION_NAME;
            version.Status = UPDATED_VERSION_STATUS;
            version.Sharing = UPDATED_VERSION_SHARING;
            version.DueDate = UPDATED_VERSION_DUE_DATE;
            version.Description = UPDATED_VERSION_DESCRIPTION;

            redmineManager.UpdateObject<Redmine.Net.Api.Types.Version>(UPDATED_VERSION_ID, version);

            Redmine.Net.Api.Types.Version updatedVersion = redmineManager.GetObject<Redmine.Net.Api.Types.Version>(UPDATED_VERSION_ID, null);

            Assert.IsNotNull(updatedVersion, "Updated version is null.");
            Assert.AreEqual<Redmine.Net.Api.Types.Version>(version, updatedVersion, "Version was not properly updated.");
        }

        [TestMethod]
        public void Should_Delete_Version()
        {
            try
            {
                redmineManager.DeleteObject<Redmine.Net.Api.Types.Version>(DELETED_VERSION_ID, null);
            }
            catch (RedmineException)
            {
                Assert.Fail("Version could not be deleted.");
                return;
            }

            try
            {
                Redmine.Net.Api.Types.Version version = redmineManager.GetObject<Redmine.Net.Api.Types.Version>(DELETED_VERSION_ID, null);
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
