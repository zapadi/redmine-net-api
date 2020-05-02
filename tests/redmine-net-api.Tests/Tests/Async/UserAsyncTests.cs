#if !(NET20 || NET40)
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Redmine.Net.Api;
using Redmine.Net.Api.Async;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineApi.Tests.Tests.Async
{
    [Collection("RedmineCollection")]
    public class UserAsyncTests
    {
        private const string USER_ID = "8";
        private const string LIMIT = "2";
        private const string OFFSET = "1";
        private const int GROUP_ID = 9;

        private readonly RedmineFixture fixture;
        public UserAsyncTests(RedmineFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async Task Should_Get_CurrentUser()
        {
            var currentUser = await fixture.RedmineManager.GetCurrentUserAsync();
            Assert.NotNull(currentUser);
        }

        [Fact]
        public async Task Should_Get_User_By_Id()
        {
            var user = await fixture.RedmineManager.GetObjectAsync<User>(USER_ID, null);
            Assert.NotNull(user);
        }

        [Fact]
        public async Task Should_Get_User_By_Id_Including_Groups_And_Memberships()
        {
            var user = await fixture.RedmineManager.GetObjectAsync<User>(USER_ID, new NameValueCollection() { { RedmineKeys.INCLUDE, "groups,memberships" } });

            Assert.NotNull(user);

            Assert.NotNull(user.Groups);
            Assert.True(user.Groups.Count == 1, "Group count != 1");

            Assert.NotNull(user.Memberships);
            Assert.True(user.Memberships.Count == 3, "Membership count != 3");
        }

        [Fact]
        public async Task Should_Get_X_Users_From_Offset_Y()
        {
            var result = await fixture.RedmineManager.GetPaginatedObjectsAsync<User>(new NameValueCollection() {
                { RedmineKeys.INCLUDE, "groups, memberships" },
                {RedmineKeys.LIMIT,LIMIT },
                {RedmineKeys.OFFSET,OFFSET }
            });

            Assert.NotNull(result);
            Assert.All(result.Items, u => Assert.IsType<User>(u));
        }

        [Fact]
        public async Task Should_Get_All_Users_With_Groups_And_Memberships()
        {
            var users = await fixture.RedmineManager.GetObjectsAsync<User>(new NameValueCollection { { RedmineKeys.INCLUDE, "groups, memberships" } });

            Assert.NotNull(users);
            Assert.All(users, u => Assert.IsType<User>(u));
        }

        [Fact]
        public async Task Should_Get_Active_Users()
        {
            var users = await fixture.RedmineManager.GetObjectsAsync<User>(new NameValueCollection()
                {
                    { RedmineKeys.STATUS, ((int)UserStatus.StatusActive).ToString(CultureInfo.InvariantCulture) }
                });

            Assert.NotNull(users);
            Assert.True(users.Count == 6);
            Assert.All(users, u => Assert.IsType<User>(u));
        }

        [Fact]
        public async Task Should_Get_Anonymous_Users()
        {
            var users = await fixture.RedmineManager.GetObjectsAsync<User>(new NameValueCollection()
                {
                    { RedmineKeys.STATUS, ((int)UserStatus.StatusAnonymous).ToString(CultureInfo.InvariantCulture) }
                });

            Assert.NotNull(users);
            Assert.True(users.Count == 0);
            Assert.All(users, u => Assert.IsType<User>(u));
        }

        [Fact]
        public async Task Should_Get_Locked_Users()
        {
            var users = await fixture.RedmineManager.GetObjectsAsync<User>(new NameValueCollection()
                {
                    { RedmineKeys.STATUS, ((int)UserStatus.StatusLocked).ToString(CultureInfo.InvariantCulture) }
                });

            Assert.NotNull(users);
            Assert.True(users.Count == 1);
            Assert.All(users, u => Assert.IsType<User>(u));
        }

        [Fact]
        public async Task Should_Get_Registered_Users()
        {
            var users = await fixture.RedmineManager.GetObjectsAsync<User>(new NameValueCollection()
                {
                    { RedmineKeys.STATUS, ((int)UserStatus.StatusRegistered).ToString(CultureInfo.InvariantCulture) }
                });

            Assert.NotNull(users);
            Assert.True(users.Count == 1);
            Assert.All(users, u => Assert.IsType<User>(u));
        }

        [Fact]
        public async Task Should_Get_Users_By_Group()
        {
            var users = await fixture.RedmineManager.GetObjectsAsync<User>(new NameValueCollection()
                {
                    {RedmineKeys.GROUP_ID, GROUP_ID.ToString(CultureInfo.InvariantCulture)}
                });

            Assert.NotNull(users);
            Assert.True(users.Count == 3);
            Assert.All(users, u => Assert.IsType<User>(u));
        }

        [Fact]
        public async Task Should_Add_User_To_Group()
        {
            await fixture.RedmineManager.AddUserToGroupAsync(GROUP_ID, int.Parse(USER_ID));

            var user = fixture.RedmineManager.GetObject<User>(USER_ID.ToString(CultureInfo.InvariantCulture), new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.GROUPS } });

            Assert.NotNull(user.Groups);
            Assert.True(user.Groups.FirstOrDefault(g => g.Id == GROUP_ID) != null);
        }

        [Fact]
        public async Task Should_Remove_User_From_Group()
        {
            await fixture.RedmineManager.RemoveUserFromGroupAsync(GROUP_ID, int.Parse(USER_ID));

            var user = await fixture.RedmineManager.GetObjectAsync<User>(USER_ID.ToString(CultureInfo.InvariantCulture), new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.GROUPS } });

            Assert.True(user.Groups == null || user.Groups.FirstOrDefault(g => g.Id == GROUP_ID) == null);
        }

        [Fact]
        public async Task Should_Create_User()
        {
            var user = new User
            {
                Login = "userTestLogin4",
                FirstName = "userTestFirstName",
                LastName = "userTestLastName",
                Email = "testTest4@redmineapi.com",
                Password = "123456",
                AuthenticationModeId = 1,
                MustChangePassword = false
            };


            var icf = (IssueCustomField)IdentifiableName.Create<IssueCustomField>(4);
            icf.Values = new List<CustomFieldValue> { new CustomFieldValue { Info = "userTestCustomField:" + DateTime.UtcNow } };

            user.CustomFields = new List<IssueCustomField>();
            user.CustomFields.Add(icf);

            var createdUser = await fixture.RedmineManager.CreateObjectAsync(user);

            Assert.Equal(user.Login, createdUser.Login);
            Assert.Equal(user.Email, createdUser.Email);
        }

        [Fact]
        public async Task Should_Update_User()
        {
            var userId = 59.ToString();
            var user = fixture.RedmineManager.GetObject<User>(userId, null);
            user.FirstName = "modified first name";
            await fixture.RedmineManager.UpdateObjectAsync(userId, user);

            var updatedUser = await fixture.RedmineManager.GetObjectAsync<User>(userId, null);

            Assert.Equal(user.FirstName, updatedUser.FirstName);
        }

        [Fact]
        public async Task Should_Delete_User()
        {
            var userId = 62.ToString();
            await fixture.RedmineManager.DeleteObjectAsync<User>(userId);
            await Assert.ThrowsAsync<NotFoundException>(async () => await fixture.RedmineManager.GetObjectAsync<User>(userId, null));

        }
    }
}
#endif