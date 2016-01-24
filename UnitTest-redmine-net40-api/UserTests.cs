using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;


namespace UnitTest_redmine_net40_api
{
    [TestClass]
    public class UserTests
    {
        private RedmineManager redmineManager;

        private const string USER_ID = "5";
        private const int NUMBER_OF_GROUPS = 1;
        private const int NUMBER_OF_MEMBERSHIPS = 2;
        private const string LIMIT = "2";
        private const string OFFSET = "1";
        private const UserStatus USER_STATE = UserStatus.STATUS_ACTIVE;
       
        //user data - used for create
        private const string USER_LOGIN = "user1";
        private const string USER_FIRST_NAME = "User";
        private const string USER_LAST_NAME = "One";
        private const string USER_EMAIL = "user1@mail.com";
        private const string USER_PASSWORD = "12345678";
        private const int USER_CUSTOM_FIELD_ID = 9;
        private const string USER_CUSTOM_FIELD_VALUE = "User custom field completed";

        private const string USER_ID_TO_UPDATE = "37";
        private const string USER_FIRST_NAME_UPDATED = "Updated first name";

        private const string USER_ID_TO_DELETE = "37";

        private const int GROUP_ID = 35;
    
        [TestInitialize]
        public void Initialize()
        {
            SetMimeTypeXML();
            SetMimeTypeJSON();
        }

        [Conditional("JSON")]
        private void SetMimeTypeJSON()
        {
            redmineManager = new RedmineManager(Helper.Uri, Helper.ApiKey, MimeFormat.json);
        }

        [Conditional("XML")]
        private void SetMimeTypeXML()
        {
            redmineManager = new RedmineManager(Helper.Uri, Helper.ApiKey);
        }
      
        [TestMethod]
        public void Should_Get_Current_User()
        {
            User currentUser = redmineManager.GetCurrentUser();

            Assert.IsNotNull(currentUser, "Current user is null.");
            Assert.AreEqual(currentUser.ApiKey, Helper.ApiKey, "Current user api key is invalid.");
        }

        [TestMethod]
        public void Should_Get_User_By_Id()
        {
            User user = redmineManager.GetObject<User>(USER_ID, null);

            Assert.IsNotNull(user, "Get user by id returned null!");
        }

        [TestMethod]
        public void Should_Get_User_By_Id_Including_Groups_And_Memberships()
        {
            var user = redmineManager.GetObject<User>(USER_ID, new NameValueCollection() { { RedmineKeys.INCLUDE, RedmineKeys.GROUPS+","+RedmineKeys.MEMBERSHIPS } });

            Assert.IsNotNull(user, "Get user by id returned null!");

            CollectionAssert.AllItemsAreNotNull(user.Groups, "Groups contains null items.");
            CollectionAssert.AllItemsAreUnique(user.Groups, "Groups items are not unique.");
            Assert.IsTrue(user.Groups.Count == NUMBER_OF_GROUPS, "Group count != "+NUMBER_OF_GROUPS);

            CollectionAssert.AllItemsAreNotNull(user.Memberships, "Memberships contains null items.");
            CollectionAssert.AllItemsAreUnique(user.Memberships, "Memberships items are not unique.");
            Assert.IsTrue(user.Memberships.Count == NUMBER_OF_MEMBERSHIPS, "Membership count != "+NUMBER_OF_MEMBERSHIPS);
        }

        [TestMethod]
        public void Should_Get_X_Users_From_Offset_Y()
        {
            var result = redmineManager.GetPaginatedObjects<User>(new NameValueCollection() {
                { RedmineKeys.INCLUDE, RedmineKeys.GROUPS+","+RedmineKeys.MEMBERSHIPS },
                {RedmineKeys.LIMIT,LIMIT },
                {RedmineKeys.OFFSET,OFFSET }
            });

            Assert.IsNotNull(result, "Users list is null.");
            CollectionAssert.AllItemsAreNotNull(result.Objects, "List contains null user!");
            CollectionAssert.AllItemsAreUnique(result.Objects, "Users not unique!");
        }

        [TestMethod]
        public void Should_Get_Users_By_State()
        {
            var users = redmineManager.GetObjects<User>(new NameValueCollection()
            {
                { RedmineKeys.STATUS, ((int)USER_STATE).ToString(CultureInfo.InvariantCulture) }
            });

            Assert.IsNotNull(users, "Users list is null.");
            CollectionAssert.AllItemsAreNotNull(users, "Users contains null items.");
            CollectionAssert.AllItemsAreUnique(users, "Users items are not unique.");
            CollectionAssert.AllItemsAreInstancesOfType(users, typeof(User), "Not all items are of type User.");
        }

        [TestMethod]
        public void Should_Add_User()
        {
            User user = new User();
            user.Login = USER_LOGIN;
            user.FirstName = USER_FIRST_NAME;
            user.LastName = USER_LAST_NAME;
            user.Email = USER_EMAIL;
            user.Password = USER_PASSWORD;
            user.AuthenticationModeId = null;
            user.MustChangePassword = false;
            user.CustomFields = new List<IssueCustomField>();
            user.CustomFields.Add(new IssueCustomField { Id = USER_CUSTOM_FIELD_ID, Values = new List<CustomFieldValue> { new CustomFieldValue { Info = USER_CUSTOM_FIELD_VALUE } } });

            User savedRedmineUser = null;
            try
            {
                savedRedmineUser = redmineManager.CreateObject<User>(user);
            }
            catch (RedmineException)
            {
                Assert.Fail("Create user failed.");
                return;
            }

            Assert.IsNotNull(savedRedmineUser, "Created user is null.");
            Assert.AreEqual(user.Login, savedRedmineUser.Login, "User login is invalid.");
            Assert.AreEqual(user.Email, savedRedmineUser.Email, "User email is invalid.");
        }

        [TestMethod]
        public void Should_Update_User()
        {
            User user = redmineManager.GetObject<User>(USER_ID_TO_UPDATE, null);
            user.FirstName = USER_FIRST_NAME_UPDATED;
            redmineManager.UpdateObject<User>(USER_ID_TO_UPDATE, user);

            User updatedUser = redmineManager.GetObject<User>(USER_ID_TO_UPDATE, null);

            Assert.IsNotNull(updatedUser, "Updated user is null.");
            Assert.AreEqual(user.FirstName, updatedUser.FirstName, "User first name was not updated.");
        }

        [TestMethod]
        public void Should_Delete_User()
        {
            User user = null;
            try
            {
                user = redmineManager.GetObject<User>(USER_ID_TO_DELETE, null);
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
                    redmineManager.DeleteObject<User>(USER_ID_TO_DELETE, null);
                }
                catch (RedmineException)
                {
                    Assert.Fail("User could not be deleted.");
                    return;
                }

                try
                {
                    user = redmineManager.GetObject<User>(USER_ID_TO_DELETE, null);
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
        public void Should_Add_User_To_Group()
        {
            redmineManager.AddUserToGroup(GROUP_ID, int.Parse(USER_ID));

            User user = redmineManager.GetObject<User>(USER_ID.ToString(), new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.GROUPS} });

            Assert.IsNotNull(user, "User is null.");
            Assert.IsNotNull(user.Groups, "Groups list is null.");
            Assert.IsTrue(user.Groups.Find(g => g.Id == GROUP_ID) != null, "User was not added to group.");
        }

        [TestMethod]
        public void Should_Get_User_By_Group()
        {
            var users = redmineManager.GetObjects<User>(new NameValueCollection()
            {
                {RedmineKeys.GROUP_ID,GROUP_ID.ToString(CultureInfo.InvariantCulture)}
            });

            Assert.IsNotNull(users, "Users list is null.");
            CollectionAssert.AllItemsAreNotNull(users, "List contains null user!");
            CollectionAssert.AllItemsAreUnique(users, "Users not unique!");
        }

        [TestMethod]
        public void Should_Delete_User_From_Group()
        {
            redmineManager.RemoveUserFromGroup(GROUP_ID, int.Parse(USER_ID));

            User user = redmineManager.GetObject<User>(USER_ID.ToString(), new NameValueCollection { {RedmineKeys.INCLUDE, RedmineKeys.GROUPS } });

            Assert.IsNotNull(user, "User object is null.");
            Assert.IsTrue(user.Groups == null || user.Groups.Find(g => g.Id == GROUP_ID) == null, "User was not removed from group.");
        }

        [TestMethod]
        public void Should_Get_All_Users_With_Metadata()
        {
            IList<User> users = redmineManager.GetObjects<User>(new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.GROUPS+","+RedmineKeys.MEMBERSHIPS } });

            Assert.IsNotNull(users, "Users list is null.");
            CollectionAssert.AllItemsAreInstancesOfType(users.ToList(), typeof(User), "Not all items are of type User.");
            CollectionAssert.AllItemsAreNotNull(users.ToList(), "Users list contains null items.");
            CollectionAssert.AllItemsAreUnique(users.ToList(), "Users are not unique.");
        }

        [TestMethod]
        public void Should_Compare_Users()
        {
            var user = redmineManager.GetObject<User>(USER_ID, new NameValueCollection() { { RedmineKeys.INCLUDE, RedmineKeys.GROUPS + "," + RedmineKeys.MEMBERSHIPS } });
            var userToCompare = redmineManager.GetObject<User>(USER_ID, new NameValueCollection() { { RedmineKeys.INCLUDE, RedmineKeys.GROUPS + "," + RedmineKeys.MEMBERSHIPS } });

            Assert.IsNotNull(user, "User is null.");
            Assert.IsTrue(user.Equals(userToCompare), "Users are not equal.");
        }
      
    }
}
