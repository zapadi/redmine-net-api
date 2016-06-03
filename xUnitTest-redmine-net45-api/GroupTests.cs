using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using Xunit;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using Redmine.Net.Api.Exceptions;

namespace xUnitTestredminenet45api
{
	[Collection("RedmineCollection")]
	public class GroupTests
	{
		private const int NUMBER_OF_GROUPS = 3;
		private const string GROUP_ID = "57";
		private const string GROUP_NAME = "Test";
		private const int NUMBER_OF_MEMBERSHIPS = 1;
		private const int NUMBER_OF_USERS = 2;

		//group data - used for create
		private const string NEW_GROUP_NAME = "Developers1";
		private const int NEW_GROUP_USER_ID = 8;

		//data used for update
		private const string UPDATED_GROUP_ID = "58";
		private const string UPDATED_GROUP_NAME = "Best Developers";
		private const int UPDATED_GROUP_USER_ID = 2;

		private const string DELETED_GROUP_ID = "63";

		RedmineFixture fixture;
		public GroupTests (RedmineFixture fixture)
		{
			this.fixture = fixture;
		}

		[Fact]
		public void Should_Get_All_Groups()
		{
			var groups = fixture.redmineManager.GetObjects<Group>(null);

			Assert.NotNull(groups);
			Assert.All (groups, g => Assert.IsType<Group> (g));
			Assert.True(groups.Count == NUMBER_OF_GROUPS, "Number of groups != "+NUMBER_OF_GROUPS);
		}

		[Fact]
		public void Should_Get_Group_With_Memberships()
		{
			var group = fixture.redmineManager.GetObject<Group>(GROUP_ID, new NameValueCollection() { { RedmineKeys.INCLUDE, RedmineKeys.MEMBERSHIPS } });

			Assert.NotNull(group);
			Assert.All (group.Memberships, m => Assert.IsType<Membership> (m));
			Assert.True(group.Memberships.Count == NUMBER_OF_MEMBERSHIPS, "Number of memberships != " + NUMBER_OF_MEMBERSHIPS);
		}

		[Fact]
		public void Should_Get_Group_With_Users()
		{
			var group = fixture.redmineManager.GetObject<Group>(GROUP_ID, new NameValueCollection() { { RedmineKeys.INCLUDE, RedmineKeys.USERS } });

			Assert.NotNull(group);
			Assert.All (group.Users, u => Assert.IsType<GroupUser> (u));
			Assert.True(group.Users.Count == NUMBER_OF_USERS, "Number of users != " + NUMBER_OF_USERS);
		}

		[Fact]
		public void Should_Get_Group_With_All_Associated_Data()
		{
			var group = fixture.redmineManager.GetObject<Group>(GROUP_ID, new NameValueCollection() { { RedmineKeys.INCLUDE, RedmineKeys.MEMBERSHIPS+","+RedmineKeys.USERS } });

			Assert.NotNull(group);

			Assert.All (group.Memberships, m => Assert.IsType<Membership> (m));
			Assert.True(group.Memberships.Count == NUMBER_OF_MEMBERSHIPS, "Number of memberships != " + NUMBER_OF_MEMBERSHIPS);

			Assert.All (group.Users, u => Assert.IsType<GroupUser> (u));
			Assert.True(group.Users.Count == NUMBER_OF_USERS, "Number of users != " + NUMBER_OF_USERS);

			Assert.True(group.Name.Equals(GROUP_NAME), "Group name is not valid.");
		}

		[Fact]
		public void Should_Add_Group()
		{
			Group group = new Group();
			group.Name = NEW_GROUP_NAME;
			group.Users = new List<GroupUser>();
			group.Users.Add(new GroupUser { Id = NEW_GROUP_USER_ID });

			Group savedGroup = null;
			RedmineException exception = (RedmineException)Record.Exception(() => savedGroup = fixture.redmineManager.CreateObject<Group>(group));

			Assert.Null (exception);
			Assert.NotNull(savedGroup);
			Assert.True(group.Name.Equals(savedGroup.Name), "Saved group name is not valid.");
		}

		[Fact]
		public void Should_Update_Group()
		{
			Group group = fixture.redmineManager.GetObject<Group>(UPDATED_GROUP_ID, new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.USERS} });
			group.Name = UPDATED_GROUP_NAME;
			group.Users.Add(new GroupUser { Id = UPDATED_GROUP_USER_ID });

			fixture.redmineManager.UpdateObject<Group>(UPDATED_GROUP_ID, group);

			Group updatedGroup = fixture.redmineManager.GetObject<Group>(UPDATED_GROUP_ID, new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.USERS } });

			Assert.NotNull(updatedGroup);
			Assert.True(updatedGroup.Name.Equals(UPDATED_GROUP_NAME), "Group name was not updated.");

			Assert.NotNull(updatedGroup.Users);
			Assert.All (updatedGroup.Users, u => Assert.IsType<GroupUser> (u));
			Assert.True(updatedGroup.Users.Find(u => u.Id == UPDATED_GROUP_USER_ID) != null, "User was not added to group.");
		}

		[Fact]
		public void Should_Delete_Group()
		{
			RedmineException exception = (RedmineException)Record.Exception(() => fixture.redmineManager.DeleteObject<Group>(DELETED_GROUP_ID, null));
			Assert.Null (exception);
			Assert.Throws<NotFoundException>(() => fixture.redmineManager.GetObject<Group>(DELETED_GROUP_ID, null));
		}

		[Fact]
		public void Should_Compare_Groups()
		{
			var firstGroup = fixture.redmineManager.GetObject<Group>(GROUP_ID, new NameValueCollection() { { RedmineKeys.INCLUDE, RedmineKeys.MEMBERSHIPS + "," + RedmineKeys.USERS } });
			var secondGroup = fixture.redmineManager.GetObject<Group>(GROUP_ID, new NameValueCollection() { { RedmineKeys.INCLUDE, RedmineKeys.MEMBERSHIPS + "," + RedmineKeys.USERS } });

			Assert.True(firstGroup.Equals(secondGroup), "Compared groups are different.");
		}
	}
}

