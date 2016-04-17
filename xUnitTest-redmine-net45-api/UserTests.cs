using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Globalization;
using Xunit;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;

namespace xUnitTestredminenet45api
{
	[Collection("RedmineCollection")]
	[Trait("Category", "User")]
	public class UserTests
	{

		private const string USER_ID = "8";
		private const int NUMBER_OF_GROUPS = 1;
		private const int NUMBER_OF_MEMBERSHIPS = 3;
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

		private const string USER_ID_TO_UPDATE = "34";
		private const string USER_FIRST_NAME_UPDATED = "Updated first name";

		private const string USER_ID_TO_DELETE = "35";

		private const string USER_ID_FOR_GROUP = "2";

		private const int GROUP_ID = 9;

		RedmineFixture fixture;
		public UserTests(RedmineFixture fixture)
		{
			this.fixture = fixture;
		}

		[Fact]
		public void Should_Get_Current_User()
		{
			User currentUser = fixture.redmineManager.GetCurrentUser();

			Assert.NotNull(currentUser);
			Assert.Equal(currentUser.ApiKey, Helper.ApiKey);


		}

		[Fact]
		public void Should_Get_User_By_Id()
		{
			User user = fixture.redmineManager.GetObject<User>(USER_ID, null);

			Assert.NotNull(user);
		}

		[Fact]
		public void Should_Get_User_By_Id_Including_Groups_And_Memberships()
		{
			var user = fixture.redmineManager.GetObject<User>(USER_ID, new NameValueCollection() { { RedmineKeys.INCLUDE, RedmineKeys.GROUPS+","+RedmineKeys.MEMBERSHIPS } });

			Assert.NotNull(user);

			Assert.NotNull (user.Groups);
			Assert.True(user.Groups.Count == NUMBER_OF_GROUPS, "Group count != "+NUMBER_OF_GROUPS);
			Assert.All (user.Groups, g => Assert.IsType<UserGroup> (g));

			Assert.NotNull (user.Memberships);
			Assert.True(user.Memberships.Count == NUMBER_OF_MEMBERSHIPS, "Membership count != "+NUMBER_OF_MEMBERSHIPS);
			Assert.All (user.Memberships, m => Assert.IsType<Membership> (m));
		}

		[Fact]
		public void Should_Get_X_Users_From_Offset_Y()
		{
			var result = fixture.redmineManager.GetPaginatedObjects<User>(new NameValueCollection() {
				{ RedmineKeys.INCLUDE, RedmineKeys.GROUPS+","+RedmineKeys.MEMBERSHIPS },
				{RedmineKeys.LIMIT,LIMIT },
				{RedmineKeys.OFFSET,OFFSET }
			});

			Assert.NotNull(result);
			Assert.All (result.Objects, u => Assert.IsType<User> (u));
		}

		[Fact]
		public void Should_Get_Users_By_State()
		{
			var users = fixture.redmineManager.GetObjects<User>(new NameValueCollection()
				{
					{ RedmineKeys.STATUS, ((int)USER_STATE).ToString(CultureInfo.InvariantCulture) }
				});

			Assert.NotNull(users);
			Assert.All (users, u => Assert.IsType<User> (u));
		}

		[Fact]
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
				savedRedmineUser = fixture.redmineManager.CreateObject<User>(user);
			}
			catch (RedmineException)
			{
				Assert.True(false, "Create user failed.");
				return;
			}

			Assert.NotNull(savedRedmineUser);
			Assert.Equal(user.Login, savedRedmineUser.Login);
			Assert.Equal(user.Email, savedRedmineUser.Email);
		}

		[Fact]
		public void Should_Update_User()
		{
			User user = fixture.redmineManager.GetObject<User>(USER_ID_TO_UPDATE, null);
			user.FirstName = USER_FIRST_NAME_UPDATED;
			fixture.redmineManager.UpdateObject<User>(USER_ID_TO_UPDATE, user);

			User updatedUser = fixture.redmineManager.GetObject<User>(USER_ID_TO_UPDATE, null);

			Assert.NotNull(updatedUser);
			Assert.Equal (user.FirstName, updatedUser.FirstName);
		}

		[Fact]
		public void Should_Delete_User()
		{
			User user = null;
			try
			{
				user = fixture.redmineManager.GetObject<User>(USER_ID_TO_DELETE, null);
			}
			catch (RedmineException)
			{

				Assert.True(false, "User not found.");
				return;
			}

			if (user != null)
			{
				try
				{
					fixture.redmineManager.DeleteObject<User>(USER_ID_TO_DELETE, null);
				}
				catch (RedmineException)
				{
					Assert.True(false, "User could not be deleted.");
					return;
				}

				try
				{
					user = fixture.redmineManager.GetObject<User>(USER_ID_TO_DELETE, null);
				}
				catch (RedmineException exc)
				{
					Assert.Contains(exc.Message, "Not Found");
					return;
				}
			}

			Assert.True(false, "Failed");

		}

		[Fact]
		public void Should_Add_User_To_Group()
		{
			fixture.redmineManager.AddUserToGroup(GROUP_ID, int.Parse(USER_ID_FOR_GROUP));

			User user = fixture.redmineManager.GetObject<User>(USER_ID_FOR_GROUP.ToString(), new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.GROUPS} });

			Assert.NotNull(user);
			Assert.NotNull(user.Groups);
			Assert.True(user.Groups.Find(g => g.Id == GROUP_ID) != null, "User was not added to group.");
		}

		[Fact]
		public void Should_Get_User_By_Group()
		{
			var users = fixture.redmineManager.GetObjects<User>(new NameValueCollection()
				{
					{RedmineKeys.GROUP_ID,GROUP_ID.ToString(CultureInfo.InvariantCulture)}
				});

			Assert.NotNull(users);
			Assert.All (users, u => Assert.IsType<User> (u));
		}

		[Fact]
		public void Should_Delete_User_From_Group()
		{
			fixture.redmineManager.RemoveUserFromGroup(GROUP_ID, int.Parse(USER_ID_FOR_GROUP));

			User user = fixture.redmineManager.GetObject<User>(USER_ID_FOR_GROUP, new NameValueCollection { {RedmineKeys.INCLUDE, RedmineKeys.GROUPS } });

			Assert.NotNull(user);
			Assert.True(user.Groups == null || user.Groups.Find(g => g.Id == GROUP_ID) == null, "User was not removed from group.");
		}

		[Fact]
		public void Should_Get_All_Users_With_Metadata()
		{
			IList<User> users = fixture.redmineManager.GetObjects<User>(new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.GROUPS+","+RedmineKeys.MEMBERSHIPS } });

			Assert.NotNull(users);
			Assert.All (users, u => Assert.IsType<User> (u));
		}

		[Fact]
		public void Should_Compare_Users()
		{
			var user = fixture.redmineManager.GetObject<User>(USER_ID, new NameValueCollection() { { RedmineKeys.INCLUDE, RedmineKeys.GROUPS + "," + RedmineKeys.MEMBERSHIPS } });
			var userToCompare = fixture.redmineManager.GetObject<User>(USER_ID, new NameValueCollection() { { RedmineKeys.INCLUDE, RedmineKeys.GROUPS + "," + RedmineKeys.MEMBERSHIPS } });

			Assert.NotNull(user);
			Assert.True(user.Equals(userToCompare), "Users are not equal.");
		}

	}
}

