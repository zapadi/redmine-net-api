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
        #region Constants
        private const string userId = "8";
        private const int groupId = 17;
        private const string userLogin = "alinac";

        //user data - used for create
        private const string login = "ioanaM";
        private const string firstName = "Ioana";
        private const string lastName = "Manea";
        private const string email = "ioanam@mail.com";
        private const string userPassword = "123456";
        private const int customFieldId = 4;
        private const string customFieldValue = "User custom field completed";

        private const string userIdToModify = "23";
        private const string userFirstNameUpdate = "Ioana G.";

        #endregion Constants

        #region Properties
        private RedmineManager redmineManager;
        private string uri;
        private string apiKey;
        private string username;
        private string password;
        #endregion Properties

        #region Initialize
        [TestInitialize]
        public void Initialize()
        {
            uri = ConfigurationManager.AppSettings["uri"];
            apiKey = ConfigurationManager.AppSettings["apiKey"];

            username = ConfigurationManager.AppSettings["username"];
            password = ConfigurationManager.AppSettings["password"];

            SetMimeTypeXML();
            SetMimeTypeJSON();
        }

        [Conditional("JSON")]
        private void SetMimeTypeJSON()
        {
            redmineManager = new RedmineManager(uri, apiKey, MimeFormat.json);
        }

        [Conditional("XML")]
        private void SetMimeTypeXML()
        {
            redmineManager = new RedmineManager(uri, apiKey);
        }
        #endregion Initialize

        #region Tests
        [TestMethod]
        public void RedmineUser_ShouldReturnCurrentUser()
        {
            User currentUser = redmineManager.GetCurrentUser();

            Assert.AreEqual(currentUser.ApiKey, ConfigurationManager.AppSettings["apiKey"]);
        }

        [TestMethod]
        public void RedmineUser_ShouldGetUserById()
        {
            User user = redmineManager.GetObject<User>(userId, new NameValueCollection { { "include", "groups,memberships" }});

            Assert.AreEqual(user.Login, userLogin);
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
            redmineUser.Login = login;
            redmineUser.FirstName = firstName;
            redmineUser.LastName = lastName;
            redmineUser.Email =email;
            redmineUser.Password = userPassword;
            redmineUser.AuthenticationModeId = null;
            redmineUser.MustChangePassword = false;
            redmineUser.CustomFields = new List<IssueCustomField>();
            redmineUser.CustomFields.Add(new IssueCustomField { Id = customFieldId, Values = new List<CustomFieldValue> { new CustomFieldValue { Info = customFieldValue} } });

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
            User user = redmineManager.GetObject<User>(userIdToModify, null);
            user.FirstName = userFirstNameUpdate;
            redmineManager.UpdateObject<User>(userIdToModify, user);

            User updatedUser = redmineManager.GetObject<User>(userIdToModify, null);

            Assert.AreEqual(user.FirstName, updatedUser.FirstName);
        }

        [TestMethod]
        public void RedmineUser_ShouldDeleteUser()
        {

            User user = null;
            try
            {
                user = redmineManager.GetObject<User>(userIdToModify, null);
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
                    redmineManager.DeleteObject<User>(userIdToModify, null);
                }
                catch (RedmineException)
                {
                    Assert.Fail("User could not be deleted.");
                    return;
                }

                try
                {
                    user = redmineManager.GetObject<User>(userIdToModify, null);
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
            redmineManager.AddUser(groupId, int.Parse(userId));

            User user = redmineManager.GetObject<User>(userId.ToString(), new NameValueCollection { { "include", "groups" } });

            Assert.IsTrue(user.Groups.Find(g => g.Id == groupId) != null);
        }

        [TestMethod]
        public void RedmineUser_ShouldDeleteUserFromGroup()
        {
            redmineManager.DeleteUser(groupId, int.Parse(userId));

            User user = redmineManager.GetObject<User>(userId.ToString(), new NameValueCollection { { "include", "groups" } });

            Assert.IsTrue(user.Groups == null || user.Groups.Find(g => g.Id == groupId) == null);
        }

        [TestMethod]
        public void RedmineUser_ShouldReturnAllUsers()
        {
            IList<User> users = redmineManager.GetObjectList<User>(new NameValueCollection { { "include", "groups,memberships" } });

            Assert.IsNotNull(users);
        }

        [TestMethod]
        public void RedmineUser_ShouldGetUserByGroup()
        {
            var users = redmineManager.GetUsers(UserStatus.STATUS_ACTIVE, null, groupId);

            Assert.IsNotNull(users);
        }

        [TestMethod]
        public void RedmineUser_ShouldCompareUsers()
        {
            RedmineManager redmineSecondManager;

            #if JSON
                redmineSecondManager = new RedmineManager(uri, apiKey, MimeFormat.xml);
            #else
                redmineSecondManager = new RedmineManager(uri, apiKey, MimeFormat.json);
            #endif

            User user = redmineManager.GetObject<User>(userId, new NameValueCollection { { "include", "groups,memberships" } });
            User secondUser = redmineSecondManager.GetObject<User>(userId, new NameValueCollection { { "include", "groups,memberships" } });

            Assert.IsTrue(user.Equals(secondUser));
        }
        #endregion Tests
    }
}
