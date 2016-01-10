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
    public class ProjectMembershipTests
    {
        #region Constants
        private const string projectId = "9";

        //PM data - used for create
        private const int newPMUserId = 2;
        private const int newPMRoleId = 4;

        private const string projectMembershipId = "118";

        //PM data - used for update
        private const string updatedPMId = "111";
        private const int updatedPMRoleId = 5;

        private const string deletedPMId = "111";
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
        public void RedmineProjectMembership_ShouldGetAllByProject()
        {
            var projectMemberships = redmineManager.GetObjects<ProjectMembership>(new NameValueCollection { { "project_id", projectId } });

            Assert.IsNotNull(projectMemberships);
        }

        [TestMethod]
        public void RedmineProjectMembership_ShouldAdd()
        {
            ProjectMembership pm = new ProjectMembership();
            pm.User = new IdentifiableName { Id = newPMUserId };
            pm.Roles = new List<MembershipRole>();
            pm.Roles.Add(new MembershipRole { Id = newPMRoleId });

            ProjectMembership updatedPM = redmineManager.CreateObject<ProjectMembership>(pm, projectId);

            Assert.IsNotNull(updatedPM);
        }

        [TestMethod]
        public void RedmineProjectMembership_ShouldGetById()
        {
            var projectMembership = redmineManager.GetObject<ProjectMembership>(projectMembershipId, null);

            Assert.IsNotNull(projectMembership);
        }

        [TestMethod]
        public void RedmineProjectMembership_ShouldUpdate()
        {
            var pm = redmineManager.GetObject<ProjectMembership>(updatedPMId, null);
            pm.Roles.Add(new MembershipRole { Id = updatedPMRoleId });

            redmineManager.UpdateObject<ProjectMembership>(updatedPMId, pm);

            var updatedPM = redmineManager.GetObject<ProjectMembership>(updatedPMId, null);

            Assert.IsTrue(updatedPM.Roles.Find(r => r.Id == updatedPMRoleId) != null);
        }

        [TestMethod]
        public void RedmineProjectMembership_ShouldDelete()
        {
            try
            {
                redmineManager.DeleteObject<ProjectMembership>(deletedPMId, null);
            }
            catch (RedmineException)
            {
                Assert.Fail("Project membership could not be deleted.");
                return;
            }

            try
            {
                ProjectMembership projectMembership = redmineManager.GetObject<ProjectMembership>(deletedPMId, null);
            }
            catch (RedmineException exc)
            {
                StringAssert.Contains(exc.Message, "Not Found");
                return;
            }
            Assert.Fail("Test failed");
        }

        [TestMethod]
        public void RedmineProjectMembership_ShouldCompare()
        {
            var projectMembership = redmineManager.GetObject<ProjectMembership>(projectMembershipId, null);
            var projectMembershipToCompare = redmineManager.GetObject<ProjectMembership>(projectMembershipId, null);

            Assert.IsTrue(projectMembership.Equals(projectMembershipToCompare));
        }
        #endregion Tests
    }
}
