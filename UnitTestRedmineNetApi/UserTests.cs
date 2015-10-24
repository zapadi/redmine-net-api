using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;


namespace UnitTestRedmineNetApi
{
    [TestClass]
    public class UserTests
    {
        private RedmineManager redmineManager;
        private string uri;
        private string apiKey;
        private string username;
        private string password;

        [TestInitialize]
        public void Initialize()
        {
            uri = ConfigurationManager.AppSettings["uri"];
            // apiKey = ConfigurationManager.AppSettings["apiKey"];

            username = ConfigurationManager.AppSettings["username"];
            password = ConfigurationManager.AppSettings["password"];

            SetMimeTypeXML();
            SetMimeTypeJSON();
        }

        [Conditional("JSON")]
        private void SetMimeTypeJSON()
        {
            redmineManager = new RedmineManager(uri, username, password, MimeFormat.json);
        }

        [Conditional("XML")]
        private void SetMimeTypeXML()
        {
            redmineManager = new RedmineManager(uri, username, password);
        }

        [TestMethod]
        public void RedmineUser_ShouldReturnCurrentUser()
        {
            User currentUser = redmineManager.GetCurrentUser();

            Assert.AreEqual(currentUser.ApiKey, ConfigurationManager.AppSettings["apiKey"]);
        }

        [TestMethod]
        public void RedmineUser_ShouldGetUserById()
        {
            var id = 8;

            User user = redmineManager.GetObject<User>(id.ToString(), new NameValueCollection { { "include", "groups" }, { "include", "memberships" } });

            Assert.AreEqual(user.Login, "alinac");
        }

        [TestMethod]
        public void RedmineUser_CreateUser_ShouldReturnInvalidEntity()
        {
            User redmineUser = new User();

            try
            {
                redmineManager.CreateObject<User>(redmineUser);
            }
            catch (RedmineException exc)
            {
                StringAssert.Contains(exc.Message, "invalid or missing attribute parameters");
                return;
            }
            Assert.Fail("No exception was thrown.");
        }

        [TestMethod]
        public void RedmineUser_ShouldCreateUser()
        {
            User redmineUser = new User();
            redmineUser.Login = "ioanap";
            redmineUser.FirstName = "Ioana";
            redmineUser.LastName = "Popa";
            redmineUser.Email = "ioanap@mail.com";
            redmineUser.Password = "123456";
            redmineUser.AuthenticationModeId = null;
            redmineUser.MustChangePassword = false;
            redmineUser.CustomFields = new List<IssueCustomField>();
            redmineUser.CustomFields.Add(new IssueCustomField { Id = 4, Values = new List<CustomFieldValue> { new CustomFieldValue { Info = "User custom field completed" } } });

            User savedRedmineUser = null;
            try
            {
                savedRedmineUser = redmineManager.CreateObject<User>(redmineUser);
            }
            catch (RedmineException)
            {
                Assert.Fail("Create user failed.");
                return;
            }

            Assert.AreEqual(redmineUser.Login, savedRedmineUser.Login);
        }

        [TestMethod]
        public void RedmineUser_ShouldUpdateUser()
        {
            var id = 14;

            User user = redmineManager.GetObject<User>(id.ToString(), null);
            user.FirstName = "Ioana M.";
            redmineManager.UpdateObject<User>(id.ToString(), user);

            User updatedUser = redmineManager.GetObject<User>(id.ToString(), null);

            Assert.AreEqual(user.FirstName, updatedUser.FirstName);
        }

        [TestMethod]
        public void RedmineUser_ShouldDeleteUser()
        {
            var id = 14;

            User user = null;
            try
            {
                user = redmineManager.GetObject<User>(id.ToString(), null);
            }
            catch (RedmineException)
            {

                Assert.Fail("User not found.");
                return;
            }

            if (user != null)
            {
                try
                {
                    redmineManager.DeleteObject<User>(id.ToString(), null);
                }
                catch (RedmineException)
                {
                    Assert.Fail("User could not be deleted.");
                    return;
                }

                try
                {
                    user = redmineManager.GetObject<User>(id.ToString(), null);
                }
                catch (RedmineException exc)
                {
                    StringAssert.Contains(exc.Message, "Not Found");
                    return;
                }
            }

            Assert.Fail("Failed");

        }

        [TestMethod]
        public void RedmineUser_ShouldAddUserToGroup()
        {
            var groupId = 9;
            var userId = 14;

            redmineManager.AddUser(groupId, userId);

            User user = redmineManager.GetObject<User>(userId.ToString(), new NameValueCollection { { "include", "groups" } });

            Assert.IsTrue(user.Groups.Find(g => g.Id == groupId) != null);
        }

        [TestMethod]
        public void RedmineUser_ShouldDeleteUserFromGroup()
        {
            var groupId = 9;
            var userId = 14;

            redmineManager.DeleteUser(groupId, userId);

            User user = redmineManager.GetObject<User>(userId.ToString(), new NameValueCollection { { "include", "groups" } });

            Assert.IsTrue(user.Groups == null || user.Groups.Find(g => g.Id == groupId) == null);
        }

        [TestMethod]
        public void RedmineUser_ShouldReturnAllUsers()
        {
            IList<User> users = redmineManager.GetObjectList<User>(new NameValueCollection { { "include", "groups" }, { "include", "memberships" } });

            Assert.IsTrue(users.Count == 3);
        }

        [TestMethod]
        public void RedmineUser_ShouldGetUserByGroup()
        {
            var users = redmineManager.GetUsers(UserStatus.STATUS_ACTIVE, null, 17);

            Assert.IsTrue(users.Count == 2);
        }
    }
}
