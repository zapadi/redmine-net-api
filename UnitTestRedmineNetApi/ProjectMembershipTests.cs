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
        public void RedmineProjectMembership_ShouldGetAllByProject()
        {
            string projectId = "9";

            var projectMemberships = redmineManager.GetObjectList<ProjectMembership>(new NameValueCollection { { "project_id", projectId } });

            Assert.IsNotNull(projectMemberships);
        }

        [TestMethod]
        public void RedmineProjectMembership_ShouldAdd()
        {
            ProjectMembership pm = new ProjectMembership();
            pm.User = new IdentifiableName { Id = 17 };
            pm.Roles = new List<MembershipRole>();
            pm.Roles.Add(new MembershipRole { Id = 4 });

            ProjectMembership updatedPM = redmineManager.CreateObject<ProjectMembership>(pm, "9");

            Assert.IsNotNull(updatedPM);
        }

        [TestMethod]
        public void RedmineProjectMembership_ShouldGetById()
        {
            string pmId = "104";

            var projectMembership = redmineManager.GetObject<ProjectMembership>(pmId, null);

            Assert.IsNotNull(projectMembership);
        }

        [TestMethod]
        public void RedmineProjectMembership_ShouldUpdate()
        {
            var pmId = "104";

            var pm = redmineManager.GetObject<ProjectMembership>(pmId, null);
            pm.Roles.Add(new MembershipRole { Id = 4 });

            redmineManager.UpdateObject<ProjectMembership>(pmId, pm);

            var updatedPM = redmineManager.GetObject<ProjectMembership>(pmId, null);

            Assert.IsTrue(updatedPM.Roles.Find(r => r.Id == 4) != null);
        }

        [TestMethod]
        public void RedmineProjectMembership_ShouldDelete()
        {
            var pmId = "104";

            try
            {
                redmineManager.DeleteObject<ProjectMembership>(pmId, null);
            }
            catch (RedmineException)
            {
                Assert.Fail("Project membership could not be deleted.");
                return;
            }

            try
            {
                ProjectMembership projectMembership = redmineManager.GetObject<ProjectMembership>(pmId, null);
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
