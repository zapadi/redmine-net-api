using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;
using System.Diagnostics;
using System.Threading.Tasks;
using Redmine.Net.Api.Async;
using Redmine.Net.Api.Types;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Globalization;
using Redmine.Net.Api.Extensions;
//using Redmine.Net.Api.Logging;

namespace UnitTest_redmine_net45_api
{
    [TestClass]
    public class UserTests
    {
        private RedmineManager redmineManager;

        private const string userId = "8";
        private const string limit = "2";
        private const string offset = "1";
        private const int groupId = 9;


        [TestInitialize]
        public void Initialize()
        {
            SetMimeTypeXML();
            SetMimeTypeJSON();
            redmineManager.UseTraceLog();
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
        public async Task Should_Get_CurrentUser()
        {
            var currentUser = await redmineManager.GetCurrentUserAsync();
            Assert.IsNotNull(currentUser, "Get current user returned null!");
        }

        [TestMethod]
        //  [ExpectedException(typeof(RedmineException))]
        public async Task Should_Get_User_By_Id()
        {
            var user = await redmineManager.GetObjectAsync<User>(userId, null);
            Assert.IsNotNull(user, "Get user by id returned null!");
        }

        [TestMethod]
        public async Task Should_Get_User_By_Id_Including_Groups_And_Memberships()
        {
            var user = await redmineManager.GetObjectAsync<User>(userId, new NameValueCollection() { { RedmineKeys.INCLUDE, "groups,memberships" } });

            Assert.IsNotNull(user, "Get user by id returned null!");

            CollectionAssert.AllItemsAreNotNull(user.Groups, "Groups contains null items.");
            CollectionAssert.AllItemsAreUnique(user.Groups, "Groups items are not unique.");
            Assert.IsTrue(user.Groups.Count == 1, "Group count != 1");

            CollectionAssert.AllItemsAreNotNull(user.Memberships, "Memberships contains null items.");
            CollectionAssert.AllItemsAreUnique(user.Memberships, "Memberships items are not unique.");
            Assert.IsTrue(user.Memberships.Count == 3, "Membership count != 3");
        }

        [TestMethod]
        public async Task Should_Get_X_Users_From_Offset_Y()
        {
            var result = await redmineManager.GetPaginatedObjectsAsync<User>(new NameValueCollection() {
                { RedmineKeys.INCLUDE, "groups, memberships" },
                {RedmineKeys.LIMIT,limit },
                {RedmineKeys.OFFSET,offset }
            });

            Assert.IsNotNull(result);
            CollectionAssert.AllItemsAreNotNull(result.Objects, "contains null user!");
            CollectionAssert.AllItemsAreUnique(result.Objects, "users not unique!");
        }

        [TestMethod]
        public async Task Should_Get_All_Users_With_Groups_And_Memberships()
        {
            List<User> users = await redmineManager.GetObjectsAsync<User>(new NameValueCollection { { RedmineKeys.INCLUDE, "groups, memberships" } });

            CollectionAssert.AllItemsAreNotNull(users, "contains null user!");
            CollectionAssert.AllItemsAreUnique(users, "users not unique!");
            CollectionAssert.AllItemsAreInstancesOfType(users, typeof(User));
        }

        [TestMethod]
        public async Task Should_Get_Active_Users()
        {
            var users = await redmineManager.GetObjectsAsync<User>(new NameValueCollection()
            {
                { RedmineKeys.STATUS, ((int)UserStatus.STATUS_ACTIVE).ToString(CultureInfo.InvariantCulture) }
            });

            Assert.IsNotNull(users);
            Assert.IsTrue(users.Count == 6);
            CollectionAssert.AllItemsAreNotNull(users);
            CollectionAssert.AllItemsAreUnique(users);
            CollectionAssert.AllItemsAreInstancesOfType(users, typeof(User));
        }

        [TestMethod]
        public async Task Should_Get_Anonymous_Users()
        {
            var users = await redmineManager.GetObjectsAsync<User>(new NameValueCollection()
            {
                { RedmineKeys.STATUS, ((int)UserStatus.STATUS_ANONYMOUS).ToString(CultureInfo.InvariantCulture) }
            });

            Assert.IsNotNull(users);
            Assert.IsTrue(users.Count == 0);
            CollectionAssert.AllItemsAreNotNull(users);
            CollectionAssert.AllItemsAreUnique(users);
            CollectionAssert.AllItemsAreInstancesOfType(users, typeof(User));
        }

        [TestMethod]
        public async Task Should_Get_Locked_Users()
        {
            var users = await redmineManager.GetObjectsAsync<User>(new NameValueCollection()
            {
                { RedmineKeys.STATUS, ((int)UserStatus.STATUS_LOCKED).ToString(CultureInfo.InvariantCulture) }
            });

            Assert.IsNotNull(users);
            Assert.IsTrue(users.Count == 1);
            CollectionAssert.AllItemsAreNotNull(users);
            CollectionAssert.AllItemsAreUnique(users);
            CollectionAssert.AllItemsAreInstancesOfType(users, typeof(User));
        }

        [TestMethod]
        public async Task Should_Get_Registered_Users()
        {
            var users = await redmineManager.GetObjectsAsync<User>(new NameValueCollection()
            {
                { RedmineKeys.STATUS, ((int)UserStatus.STATUS_REGISTERED).ToString(CultureInfo.InvariantCulture) }
            });

            Assert.IsNotNull(users);
            Assert.IsTrue(users.Count == 1);
            CollectionAssert.AllItemsAreNotNull(users);
            CollectionAssert.AllItemsAreUnique(users);
            CollectionAssert.AllItemsAreInstancesOfType(users, typeof(User));
        }

        [TestMethod]
        public async Task Should_Get_Users_By_Group()
        {
            var users = await redmineManager.GetObjectsAsync<User>(new NameValueCollection()
            {
                {RedmineKeys.GROUP_ID, groupId.ToString(CultureInfo.InvariantCulture)}
            });

            Assert.IsNotNull(users);
            Assert.IsTrue(users.Count == 3);
            CollectionAssert.AllItemsAreNotNull(users);
            CollectionAssert.AllItemsAreUnique(users);
            CollectionAssert.AllItemsAreInstancesOfType(users, typeof(User));
        }

        [TestMethod]
        public async Task Should_Add_User_To_Group()
        {
            await redmineManager.AddUserToGroupAsync(groupId, int.Parse(userId));

            User user = redmineManager.GetObject<User>(userId.ToString(CultureInfo.InvariantCulture), new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.GROUPS } });

            CollectionAssert.AllItemsAreNotNull(user.Groups);
            CollectionAssert.AllItemsAreUnique(user.Groups);

            Assert.IsTrue(user.Groups.FirstOrDefault(g => g.Id == groupId) != null);
        }

        [TestMethod]
        public async Task Should_Remove_User_From_Group()
        {
            await redmineManager.DeleteUserFromGroupAsync(groupId, int.Parse(userId));

            User user = await redmineManager.GetObjectAsync<User>(userId.ToString(CultureInfo.InvariantCulture), new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.GROUPS } });

            Assert.IsTrue(user.Groups == null || user.Groups.FirstOrDefault(g => g.Id == groupId) == null);
        }

        [TestMethod]
        public async Task Should_Create_User()
        {
            User user = new User();
            user.Login = "userTestLogin4";
            user.FirstName = "userTestFirstName";
            user.LastName = "userTestLastName";
            user.Email = "testTest4@redmineapi.com";
            user.Password = "123456";
            user.AuthenticationModeId = 1;
            user.MustChangePassword = false;
            user.CustomFields = new List<IssueCustomField>();
            user.CustomFields.Add(new IssueCustomField { Id = 4, Values = new List<CustomFieldValue> { new CustomFieldValue { Info = "userTestCustomField:" + DateTime.UtcNow } } });

            var createdUser = await redmineManager.CreateObjectAsync(user);

            Assert.AreEqual(user.Login, createdUser.Login);
            Assert.AreEqual(user.Email, createdUser.Email);
        }

        [TestMethod]
        public async Task Should_Update_User()
        {
            var userId = 44.ToString();
            User user = redmineManager.GetObject<User>(userId, null);
            user.FirstName = "modified first name";
            await redmineManager.UpdateObjectAsync(userId, user);

            User updatedUser = await redmineManager.GetObjectAsync<User>(userId, null);

            Assert.AreEqual(user.FirstName, updatedUser.FirstName);
        }

        [TestMethod]
        public async Task Should_Delete_User()
        {
            var userId = 44.ToString();
            try
            {
                await redmineManager.DeleteObjectAsync<User>(userId, null);

                var user = await redmineManager.GetObjectAsync<User>(userId, null);
            }
            catch (RedmineException exc)
            {
                StringAssert.Contains(exc.Message, "Not Found");
            }
        }
    }
}