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
        public void GetAllGroups()
        {
            var result = redmineManager.GetObjectList<Group>(null);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetGroup_With_Memberships()
        {
            var result = redmineManager.GetObject<Group>("9", new NameValueCollection() { { "include", "memberships" } });

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Should_Return_Group_With_Users()
        {
            var result = redmineManager.GetObject<Group>("9", new NameValueCollection() { { "include", "users" } });

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetGroup_WithAll_AssociatedData()
        {
            var result = redmineManager.GetObject<Group>("9", new NameValueCollection() { { "include", "memberships, users" } });

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Add_New_Membership_To_Group()
        {
            var group = redmineManager.GetObject<Group>("9", new NameValueCollection() { { "include", "memberships" } });

            var mbs = new Membership();
            mbs.Roles = new List<MembershipRole>();
            mbs.Roles.Add(new MembershipRole()
            {
                Inherited = true,
                Name = "role de test"
            });

            group.Memberships.Add(mbs);

            redmineManager.UpdateObject("9", group);

            var updatedGroup = redmineManager.GetObject<Group>("9", new NameValueCollection() { { "include", "memberships" } });
        }

        [TestMethod]
        public void RedmineGroups_ShouldGetGroupById()
        {
            var groupId = 9;

            Group adminGroupXml = redmineManager.GetObject<Group>(groupId.ToString(), new NameValueCollection { { "include", "users" }, { "include", "memberships" } });
        
            Assert.AreEqual(adminGroupXml.Name, "Administrators");
        }

        [TestMethod]
        public void RedmineGroups_ShouldAddGroup()
        {
            Group group = new Group();
            group.Name = "Developers";
            group.Users = new List<GroupUser>();
            group.Users.Add(new GroupUser { Id = 8 });

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
            var id = 17;

            Group group = redmineManager.GetObject<Group>(id.ToString(), new NameValueCollection { { "include", "users" } });
            group.Name = "Best Developers";
            group.Users.Add(new GroupUser { Id = 2 });
            redmineManager.UpdateObject<Group>(id.ToString(), group);

            Group updatedGroup = redmineManager.GetObject<Group>(id.ToString(), new NameValueCollection { { "include", "users" } });

            Assert.IsTrue(updatedGroup.Users.Find(u => u.Id == 2) != null);
            Assert.AreEqual(group.Name, updatedGroup.Name);
        }

        [TestMethod]
        public void RedmineGroups_ShouldDeleteGroup()
        {
            var groupId = 16;
            try
            {
                redmineManager.DeleteObject<Group>(groupId.ToString(), null);
            }
            catch (RedmineException)
            {
                Assert.Fail("Group could not be deleted.");
                return;
            }

            try
            {
                Group group = redmineManager.GetObject<Group>(groupId.ToString(), null);
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