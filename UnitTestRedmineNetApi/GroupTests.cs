using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System.Diagnostics;

namespace UnitTestRedmineNetApi
{
    [TestClass]
    public class GroupTests
    {
        #region Constants
        private const string groupId = "9";
        private const string groupName = "Administrators";

        //group data - used for create
        private const string newGroupName = "Developers";
        private const int newGroupUserId = 8;

        //data used for update
        private const string updatedGroupId = "31";
        private const string updatedGroupName = "Best Developers";
        private const int updatedGroupUserId = 2;

        private const string groupIdToDelete = "31";
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
        public void GetAllGroups()
        {
            var result = redmineManager.GetObjectList<Group>(null);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetGroup_With_Memberships()
        {
            var result = redmineManager.GetObject<Group>(groupId, new NameValueCollection() { { "include", "memberships" } });

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Should_Return_Group_With_Users()
        {
            var result = redmineManager.GetObject<Group>(groupId, new NameValueCollection() { { "include", "users" } });

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetGroup_WithAll_AssociatedData()
        {
            var result = redmineManager.GetObject<Group>(groupId, new NameValueCollection() { { "include", "memberships, users" } });

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void RedmineGroups_ShouldGetGroupById()
        {
            Group adminGroup = redmineManager.GetObject<Group>(groupId, new NameValueCollection { { "include", "users,memberships" } });
        
            Assert.AreEqual(adminGroup.Name, groupName);
        }

        [TestMethod]
        public void RedmineGroups_ShouldAddGroup()
        {
            Group group = new Group();
            group.Name = newGroupName;
            group.Users = new List<GroupUser>();
            group.Users.Add(new GroupUser { Id = newGroupUserId });

            Group savedGroup = null;
            try
            {
                savedGroup = redmineManager.CreateObject<Group>(group);
            }
            catch (RedmineException)
            {
                Assert.Fail("Create group failed.");
                return;
            }

            Assert.IsNotNull(savedGroup);
            Assert.AreEqual(group.Name, savedGroup.Name);
        }

        [TestMethod]
        public void RedmineGroups_ShouldUpdateGroup()
        {
            Group group = redmineManager.GetObject<Group>(updatedGroupId, new NameValueCollection { { "include", "users" } });
            group.Name = updatedGroupName;
            group.Users.Add(new GroupUser { Id = updatedGroupUserId });
            redmineManager.UpdateObject<Group>(updatedGroupId, group);

            Group updatedGroup = redmineManager.GetObject<Group>(updatedGroupId, new NameValueCollection { { "include", "users" } });

            Assert.IsTrue(updatedGroup.Users.Find(u => u.Id == 2) != null);
            Assert.AreEqual(group.Name, updatedGroup.Name);
        }

        [TestMethod]
        public void RedmineGroups_ShouldDeleteGroup()
        {
            try
            {
                redmineManager.DeleteObject<Group>(groupIdToDelete, null);
            }
            catch (RedmineException)
            {
                Assert.Fail("Group could not be deleted.");
                return;
            }

            try
            {
                Group group = redmineManager.GetObject<Group>(groupIdToDelete, null);
            }
            catch (RedmineException exc)
            {
                StringAssert.Contains(exc.Message, "Not Found");
                return;
            }
            Assert.Fail("Test failed");
        }

        //TODO: equals
        #endregion Tests
    }
}