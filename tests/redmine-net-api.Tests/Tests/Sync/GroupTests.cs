using System.Collections.Generic;
using System.Collections.Specialized;
using Padi.RedmineApi.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineApi.Tests.Tests.Sync
{
    [Trait("Redmine-Net-Api", "Groups")]
#if !(NET20 || NET40)
    [Collection("RedmineCollection")]
#endif
    public class GroupTests
    {
        public GroupTests(RedmineFixture fixture)
        {
            this.fixture = fixture;
        }

        private readonly RedmineFixture fixture;

        private const string GROUP_ID = "57";
        private const int NUMBER_OF_MEMBERSHIPS = 1;
        private const int NUMBER_OF_USERS = 2;

        [Fact, Order(1)]
        public void Should_Add_Group()
        {
            const string NEW_GROUP_NAME = "Developers1";
            const int NEW_GROUP_USER_ID = 8;

            var group = new Group();
            group.Name = NEW_GROUP_NAME;
            group.Users = new List<GroupUser> { (GroupUser)IdentifiableName.Create<GroupUser>(NEW_GROUP_USER_ID )};

            Group savedGroup = null;
            var exception =
                (RedmineException)Record.Exception(() => savedGroup = fixture.RedmineManager.CreateObject(group));

            Assert.Null(exception);
            Assert.NotNull(savedGroup);
            Assert.True(group.Name.Equals(savedGroup.Name), "Group name is not valid.");
        }

        [Fact, Order(2)]
        public void Should_Update_Group()
        {
            const string UPDATED_GROUP_ID = "58";
            const string UPDATED_GROUP_NAME = "Best Developers";
            const int UPDATED_GROUP_USER_ID = 2;

            var group = fixture.RedmineManager.GetObject<Group>(UPDATED_GROUP_ID,
                new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.USERS } });
            group.Name = UPDATED_GROUP_NAME;
            group.Users.Add((GroupUser)IdentifiableName.Create<GroupUser>(UPDATED_GROUP_USER_ID));

            fixture.RedmineManager.UpdateObject(UPDATED_GROUP_ID, group);

            var updatedGroup = fixture.RedmineManager.GetObject<Group>(UPDATED_GROUP_ID,
                new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.USERS } });

            Assert.NotNull(updatedGroup);
            Assert.True(updatedGroup.Name.Equals(UPDATED_GROUP_NAME), "Group name was not updated.");
            Assert.NotNull(updatedGroup.Users);
            //     Assert.True(updatedGroup.Users.Find(u => u.Id == UPDATED_GROUP_USER_ID) != null,
            //"User was not added to group.");
        }

        [Fact, Order(3)]
        public void Should_Get_All_Groups()
        {
            const int NUMBER_OF_GROUPS = 3;

            var groups = fixture.RedmineManager.GetObjects<Group>();

            Assert.NotNull(groups);
            Assert.True(groups.Count == NUMBER_OF_GROUPS, "Number of groups ( " + groups.Count + " ) != " + NUMBER_OF_GROUPS);
        }

        [Fact, Order(4)]
        public void Should_Get_Group_With_All_Associated_Data()
        {
            var group = fixture.RedmineManager.GetObject<Group>(GROUP_ID,
                new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.MEMBERSHIPS + "," + RedmineKeys.USERS } });

            Assert.NotNull(group);

            Assert.True(group.Memberships.Count == NUMBER_OF_MEMBERSHIPS,
                "Number of memberships != " + NUMBER_OF_MEMBERSHIPS);

            Assert.True(group.Users.Count == NUMBER_OF_USERS, "Number of users ( " + group.Users.Count + " ) != " + NUMBER_OF_USERS);
            Assert.True(group.Name.Equals("Test"), "Group name is not valid.");
        }

        [Fact, Order(5)]
        public void Should_Get_Group_With_Memberships()
        {
            var group = fixture.RedmineManager.GetObject<Group>(GROUP_ID,
                new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.MEMBERSHIPS } });

            Assert.NotNull(group);
            Assert.True(group.Memberships.Count == NUMBER_OF_MEMBERSHIPS,
                "Number of memberships ( " + group.Memberships.Count + " ) != " + NUMBER_OF_MEMBERSHIPS);
        }

        [Fact, Order(6)]
        public void Should_Get_Group_With_Users()
        {
            var group = fixture.RedmineManager.GetObject<Group>(GROUP_ID,
                new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.USERS } });

            Assert.NotNull(group);
            Assert.True(group.Users.Count == NUMBER_OF_USERS, "Number of users ( " + group.Users.Count + " ) != " + NUMBER_OF_USERS);
        }

        [Fact, Order(99)]
        public void Should_Delete_Group()
        {
            const string DELETED_GROUP_ID = "63";

            var exception =
                (RedmineException)
                Record.Exception(() => fixture.RedmineManager.DeleteObject<Group>(DELETED_GROUP_ID));
            Assert.Null(exception);
            Assert.Throws<NotFoundException>(() => fixture.RedmineManager.GetObject<Group>(DELETED_GROUP_ID, null));
        }
    }
}