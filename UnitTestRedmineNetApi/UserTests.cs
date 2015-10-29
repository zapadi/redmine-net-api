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

            SetMimeTypeXml();
            SetMimeTypeJson();
        }

        [Conditional("JSON")]
        private void SetMimeTypeJson()
        {
            redmineManager = new RedmineManager(uri, username, password, MimeFormat.json);
        }

        [Conditional("XML")]
        private void SetMimeTypeXml()
        {
            redmineManager = new RedmineManager(uri, username, password);
        }

        [TestMethod]
        public void RedmineUser_ShouldReturnCurrentUser()
        {
            User currentUser = redmineManager.GetCurrentUser();

            Assert.AreEqual(currentUser.ApiKey, apiKey);
        }

        [TestMethod]
        public void RedmineUser_ShouldGetUserById()
        {
            const string id = "8";

            User user = redmineManager.GetObject<User>(id, new NameValueCollection { { "include", "groups, memberships" } });

            Assert.AreEqual(user.Login, "alinac");
        }

        [TestMethod]
        public void RedmineUser_CreateUser_ShouldReturnInvalidEntity()
        {
            User redmineUser = new User();

            try
            {
                redmineManager.CreateObject(redmineUser);
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
            User redmineUser = new User
            {
                Login = "ioanap2",
                FirstName = "Ioana",
                LastName = "Popa",
                Email = "ioanap@mail.com",
                Password = "123456",
                AuthenticationModeId = 1,
                MustChangePassword = false,
                CustomFields = new List<IssueCustomField>
                {
                    new IssueCustomField
                    {
                        Id = 4,
                        Values = new List<CustomFieldValue>
                        {
                            new CustomFieldValue {Info = "User custom field completed"}
                        }
                    }
                }
            };

            User savedRedmineUser = null;
            try
            {
                savedRedmineUser = redmineManager.CreateObject(redmineUser);
            }
            catch (RedmineException)
            {
                Assert.Fail("Create user failed.");
            }

            Assert.AreEqual(redmineUser.Login, savedRedmineUser.Login);
        }

        [TestMethod]
        public void RedmineUser_ShouldUpdateUser()
        {
            const string id = "14";

            var user = redmineManager.GetObject<User>(id, null);
            user.FirstName = "Ioana M.";
            redmineManager.UpdateObject(id, user);

            var updatedUser = redmineManager.GetObject<User>(id, null);

            Assert.AreEqual(user.FirstName, updatedUser.FirstName);
        }

        [TestMethod]
        public void RedmineUser_ShouldDeleteUser()
        {
            const string id = "14";

            User user = null;
            try
            {
                user = redmineManager.GetObject<User>(id, null);
            }
            catch (RedmineException)
            {

                Assert.Fail("User not found.");
            }

            if (user != null)
            {
                try
                {
                    redmineManager.DeleteObject<User>(id, null);
                }
                catch (RedmineException)
                {
                    Assert.Fail("User could not be deleted.");
                }

                try
                {
                     redmineManager.GetObject<User>(id, null);
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
            const int groupId = 9;
            const int userId = 14;

            redmineManager.AddUser(groupId, userId);

            User user = redmineManager.GetObject<User>(userId.ToString(), new NameValueCollection { { "include", "groups" } });

            Assert.IsTrue(user.Groups.Find(g => g.Id == groupId) != null);
        }

        [TestMethod]
        public void RedmineUser_ShouldDeleteUserFromGroup()
        {
            const int groupId = 9;
            const int userId = 14;

            redmineManager.DeleteUser(groupId, userId);

            User user = redmineManager.GetObject<User>(userId.ToString(), new NameValueCollection { { "include", "groups" } });

            Assert.IsTrue(user.Groups == null || user.Groups.Find(g => g.Id == groupId) == null);
        }

        [TestMethod]
        public void RedmineUser_ShouldReturnAllUsers()
        {
            IList<User> users = redmineManager.GetObjectList<User>(new NameValueCollection { { "include", "groups, memberships" } });

            Assert.IsTrue(users.Count == 3);
        }

        [TestMethod]
        public void RedmineUser_ShouldGetUserByGroup()
        {
            const int groupId = 17;
            var users = redmineManager.GetUsers(UserStatus.STATUS_ACTIVE, null, groupId);

            Assert.IsTrue(users.Count == 2);
        }
    }
}
