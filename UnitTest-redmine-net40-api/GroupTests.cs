using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System.Diagnostics;
using System.Linq;

namespace UnitTest_redmine_net40_api
{
    [TestClass]
    public class GroupTests
    {
        private RedmineManager redmineManager;

        private const int NUMBER_OF_GROUPS = 2;
        private const string GROUP_ID = "34";
        private const string GROUP_NAME = "Test";
        private const int NUMBER_OF_MEMBERSHIPS = 1;
        private const int NUMBER_OF_USERS = 2;

        //group data - used for create
        private const string NEW_GROUP_NAME = "Developers";
        private const int NEW_GROUP_USER_ID = 5;

        //data used for update
        private const string UPDATED_GROUP_ID = "36";
        private const string UPDATED_GROUP_NAME = "Best Developers";
        private const int UPDATED_GROUP_USER_ID = 1;

        private const string DELETED_GROUP_ID = "36";

        [TestInitialize]
        public void Initialize()
        {
            SetMimeTypeJSON();
            SetMimeTypeXML();
        }

        [Conditional("JSON")]
        private void SetMimeTypeJSON()
        {
            redmineManager = new RedmineManager(Helper.Uri, Helper.ApiKey, MimeFormat.json);
        }

        [Conditional("XML")]
        private void SetMimeTypeXML()
        {
            redmineManager = new RedmineManager(Helper.Uri, Helper.ApiKey, MimeFormat.xml);
        }

        [TestMethod]
        public void Should_Get_All_Groups()
        {
            var groups = redmineManager.GetObjects<Group>(null);

            Assert.IsNotNull(groups, "Get objects returned null.");
            CollectionAssert.AllItemsAreInstancesOfType(groups, typeof(Group), "Not all items are of type group.");
            CollectionAssert.AllItemsAreNotNull(groups, "Groups contains null items.");
            CollectionAssert.AllItemsAreUnique(groups, "Groups items are not unique.");
            Assert.IsTrue(groups.Count == NUMBER_OF_GROUPS, "Number of groups != "+NUMBER_OF_GROUPS);
        }

        [TestMethod]
        public void Should_Get_Group_With_Memberships()
        {
            var group = redmineManager.GetObject<Group>(GROUP_ID, new NameValueCollection() { { RedmineKeys.INCLUDE, RedmineKeys.MEMBERSHIPS } });

            Assert.IsNotNull(group, "Get group by id returned null.");
            CollectionAssert.AllItemsAreInstancesOfType(group.Memberships.ToList(), typeof(Membership), "Not all items are of type membership.");
            CollectionAssert.AllItemsAreNotNull(group.Memberships.ToList(), "Memberships contains null items.");
            CollectionAssert.AllItemsAreUnique(group.Memberships.ToList(), "Memberships items are not unique.");
            Assert.IsTrue(group.Memberships.Count == NUMBER_OF_MEMBERSHIPS, "Number of memberships != " + NUMBER_OF_MEMBERSHIPS);
        }

        [TestMethod]
        public void Should_Get_Group_With_Users()
        {
            var group = redmineManager.GetObject<Group>(GROUP_ID, new NameValueCollection() { { RedmineKeys.INCLUDE, RedmineKeys.USERS } });

            Assert.IsNotNull(group, "Get group by id returned null.");
            CollectionAssert.AllItemsAreInstancesOfType(group.Users.ToList(), typeof(GroupUser), "Not all items are of type GroupUser.");
            CollectionAssert.AllItemsAreNotNull(group.Users.ToList(), "Users contains null items.");
            CollectionAssert.AllItemsAreUnique(group.Users.ToList(), "Users items are not unique.");
            Assert.IsTrue(group.Users.Count == NUMBER_OF_USERS, "Number of users != " + NUMBER_OF_USERS);
        }

        [TestMethod]
        public void Should_Get_Group_With_All_Associated_Data()
        {
            var group = redmineManager.GetObject<Group>(GROUP_ID, new NameValueCollection() { { RedmineKeys.INCLUDE, RedmineKeys.MEMBERSHIPS+","+RedmineKeys.USERS } });

            Assert.IsNotNull(group, "Get group by id returned null.");

            CollectionAssert.AllItemsAreInstancesOfType(group.Memberships.ToList(), typeof(Membership), "Not all items are of type membership.");
            CollectionAssert.AllItemsAreNotNull(group.Memberships.ToList(), "Memberships contains null items.");
            CollectionAssert.AllItemsAreUnique(group.Memberships.ToList(), "Memberships items are not unique.");
            Assert.IsTrue(group.Memberships.Count == NUMBER_OF_MEMBERSHIPS, "Number of memberships != " + NUMBER_OF_MEMBERSHIPS);

            CollectionAssert.AllItemsAreInstancesOfType(group.Users.ToList(), typeof(GroupUser), "Not all items are of type GroupUser.");
            CollectionAssert.AllItemsAreNotNull(group.Users.ToList(), "Users contains null items.");
            CollectionAssert.AllItemsAreUnique(group.Users.ToList(), "Users items are not unique.");
            Assert.IsTrue(group.Users.Count == NUMBER_OF_USERS, "Number of users != " + NUMBER_OF_USERS);

            Assert.AreEqual(group.Name, GROUP_NAME, "Group name is not valid.");
        }

        [TestMethod]
        public void Should_Add_Group()
        {
            Group group = new Group();
            group.Name = NEW_GROUP_NAME;
            group.Users = new List<GroupUser>();
            group.Users.Add(new GroupUser { Id = NEW_GROUP_USER_ID });

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

            Assert.IsNotNull(savedGroup, "Create group returned null.");
            Assert.AreEqual(group.Name, savedGroup.Name, "Saved group name is not valid.");
        }

        [TestMethod]
        public void Should_Update_Group()
        {
            Group group = redmineManager.GetObject<Group>(UPDATED_GROUP_ID, new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.USERS} });
            group.Name = UPDATED_GROUP_NAME;
            group.Users.Add(new GroupUser { Id = UPDATED_GROUP_USER_ID });

            redmineManager.UpdateObject<Group>(UPDATED_GROUP_ID, group);

            Group updatedGroup = redmineManager.GetObject<Group>(UPDATED_GROUP_ID, new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.USERS } });

            Assert.IsNotNull(updatedGroup, "Get group returned null.");
            Assert.AreEqual(updatedGroup.Name, UPDATED_GROUP_NAME, "Group name was not updated.");

            Assert.IsNotNull(updatedGroup.Users, "Group users list is null.");
            CollectionAssert.AllItemsAreInstancesOfType(updatedGroup.Users.ToList(), typeof(GroupUser), "Not all items are of type GroupUser.");
            CollectionAssert.AllItemsAreNotNull(updatedGroup.Users.ToList(), "Users contains null items.");
            CollectionAssert.AllItemsAreUnique(updatedGroup.Users.ToList(), "Users items are not unique.");
            Assert.IsTrue(updatedGroup.Users.Find(u => u.Id == UPDATED_GROUP_USER_ID) != null, "User was not added to group.");
        }

        [TestMethod]
        public void Should_Delete_Group()
        {
            try
            {
                redmineManager.DeleteObject<Group>(DELETED_GROUP_ID, null);
            }
            catch (RedmineException)
            {
                Assert.Fail("Group could not be deleted.");
                return;
            }

            try
            {
                Group group = redmineManager.GetObject<Group>(DELETED_GROUP_ID, null);
            }
            catch (RedmineException exc)
            {
                StringAssert.Contains(exc.Message, "Not Found");
                return;
            }
            Assert.Fail("Test failed");
        }

        [TestMethod]
        public void Should_Compare_Groups()
        {
            var firstGroup = redmineManager.GetObject<Group>(GROUP_ID, new NameValueCollection() { { RedmineKeys.INCLUDE, RedmineKeys.MEMBERSHIPS + "," + RedmineKeys.USERS } });
            var secondGroup = redmineManager.GetObject<Group>(GROUP_ID, new NameValueCollection() { { RedmineKeys.INCLUDE, RedmineKeys.MEMBERSHIPS + "," + RedmineKeys.USERS } });

            Assert.IsTrue(firstGroup.Equals(secondGroup), "Compared groups are different.");
        }
    }
}