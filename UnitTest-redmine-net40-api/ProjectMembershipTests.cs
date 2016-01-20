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
    public class ProjectMembershipTests
    {
        private RedmineManager redmineManager;

        private const string PROJECT_IDENTIFIER = "redmine-test";
        private const int NUMBER_OF_PROJECT_MEMBERSHIPS = 3;

        //PM data - used for create
        private const int NEW_PROJECT_MEMBERSHIP_USER_ID = 5;
        private const int NEW_PROJECT_MEMBERSHIP_ROLE_ID = 34;

        private const string PROJECT_MEMBERSHIP_ID = "17";

        //PM data - used for update
        private const string UPDATED_PROJECT_MEMBERSHIP_ID = "17";
        private const int UPDATED_PROJECT_MEMBERSHIP_ROLE_ID = 35;

        private const string DELETED_PROJECT_MEMBERSHIP_ID = "17";

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
        public void Should_Get_Memberships_By_Project_Identifier()
        {
            var projectMemberships = redmineManager.GetObjects<ProjectMembership>(new NameValueCollection { { RedmineKeys.PROJECT_ID, PROJECT_IDENTIFIER } });

            Assert.IsNotNull(projectMemberships, "Get project memberships by project id returned null.");
            Assert.AreEqual(projectMemberships.Count, NUMBER_OF_PROJECT_MEMBERSHIPS, "Project memberships count != " + NUMBER_OF_PROJECT_MEMBERSHIPS);
            CollectionAssert.AllItemsAreNotNull(projectMemberships, "Project memberships list contains null items.");
            CollectionAssert.AllItemsAreUnique(projectMemberships, "Project memberships items are not unique.");
            CollectionAssert.AllItemsAreInstancesOfType(projectMemberships, typeof(ProjectMembership), "Not all items are of type ProjectMembership.");
        }

        [TestMethod]
        public void Should_Add_Project_Membership()
        {
            ProjectMembership pm = new ProjectMembership();
            pm.User = new IdentifiableName { Id = NEW_PROJECT_MEMBERSHIP_USER_ID };
            pm.Roles = new List<MembershipRole>();
            pm.Roles.Add(new MembershipRole { Id = NEW_PROJECT_MEMBERSHIP_ROLE_ID });

            ProjectMembership createdPM = redmineManager.CreateObject<ProjectMembership>(pm, PROJECT_IDENTIFIER);

            Assert.IsNotNull(createdPM, "Project membership is null.");
            Assert.AreEqual(createdPM.User.Id, NEW_PROJECT_MEMBERSHIP_USER_ID, "User is invalid.");
            Assert.IsNotNull(createdPM.Roles, "Project membership roles list is null.");
            Assert.IsTrue(createdPM.Roles.Exists(r => r.Id == NEW_PROJECT_MEMBERSHIP_ROLE_ID), string.Format("Role id {0} does not exist.", NEW_PROJECT_MEMBERSHIP_ROLE_ID));
        }

        [TestMethod]
        public void Should_Get_Project_Membership_By_Id()
        {
            var projectMembership = redmineManager.GetObject<ProjectMembership>(PROJECT_MEMBERSHIP_ID, null);

            Assert.IsNotNull(projectMembership, "Get project membership returned null.");
            Assert.IsNotNull(projectMembership.Project, "Project is null.");
            Assert.IsTrue(projectMembership.User != null || projectMembership.Group != null, "User and group are both null.");
            Assert.IsNotNull(projectMembership.Roles, "Project membership roles list is null.");
            CollectionAssert.AllItemsAreNotNull(projectMembership.Roles, "Project membership roles list contains null items.");
            CollectionAssert.AllItemsAreUnique(projectMembership.Roles, "Project membership roles items are not unique.");
            CollectionAssert.AllItemsAreInstancesOfType(projectMembership.Roles, typeof(MembershipRole), "Not all items are of type MembershipRole.");
        }

        [TestMethod]
        public void Should_Compare_Project_Memberships()
        {
            var projectMembership = redmineManager.GetObject<ProjectMembership>(PROJECT_MEMBERSHIP_ID, null);
            var projectMembershipToCompare = redmineManager.GetObject<ProjectMembership>(PROJECT_MEMBERSHIP_ID, null);

            Assert.IsNotNull(projectMembership, "Project membership is null.");
            Assert.IsTrue(projectMembership.Equals(projectMembershipToCompare), "Project memberships are not equal.");
        }

        [TestMethod]
        public void Should_Update_Project_Membership()
        {
            var pm = redmineManager.GetObject<ProjectMembership>(UPDATED_PROJECT_MEMBERSHIP_ID, null);
            pm.Roles.Add(new MembershipRole { Id = UPDATED_PROJECT_MEMBERSHIP_ROLE_ID });

            redmineManager.UpdateObject<ProjectMembership>(UPDATED_PROJECT_MEMBERSHIP_ID, pm);

            var updatedPM = redmineManager.GetObject<ProjectMembership>(UPDATED_PROJECT_MEMBERSHIP_ID, null);

            Assert.IsNotNull(updatedPM, "Get updated project membership returned null.");
            Assert.IsNotNull(updatedPM.Roles, "Project membership roles list is null.");
            CollectionAssert.AllItemsAreNotNull(updatedPM.Roles, "Project membership roles list contains null items.");
            CollectionAssert.AllItemsAreUnique(updatedPM.Roles, "Project membership roles items are not unique.");
            CollectionAssert.AllItemsAreInstancesOfType(updatedPM.Roles, typeof(MembershipRole), "Not all items are of type MembershipRole.");
       
            Assert.IsTrue(updatedPM.Roles.Find(r => r.Id == UPDATED_PROJECT_MEMBERSHIP_ROLE_ID) != null, string.Format("Role with id {0} was not found in roles list.", UPDATED_PROJECT_MEMBERSHIP_ROLE_ID));
        }

        [TestMethod]
        public void Should_Delete_Project_Membership()
        {
            try
            {
                redmineManager.DeleteObject<ProjectMembership>(DELETED_PROJECT_MEMBERSHIP_ID, null);
            }
            catch (RedmineException)
            {
                Assert.Fail("Project membership could not be deleted.");
                return;
            }

            try
            {
                ProjectMembership projectMembership = redmineManager.GetObject<ProjectMembership>(DELETED_PROJECT_MEMBERSHIP_ID, null);
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
