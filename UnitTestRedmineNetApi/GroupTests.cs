using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;

namespace UnitTestRedmineNetApi
{
    [TestClass]
    public class GroupTests
    {
        private RedmineManager redmineManager;

        [TestInitialize]
        public void Initialize()
        {
            var uri = ConfigurationManager.AppSettings["uri"];
            var apiKey = ConfigurationManager.AppSettings["apiKey"];
            redmineManager = new RedmineManager(uri, apiKey);
        }

        [TestMethod]
        public void GetAllGroups()
        {
            var result = redmineManager.GetObjectList<Group>(null);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetGroup_With_Memberships()
        {
            var result = redmineManager.GetObject<Group>("9", new NameValueCollection() { { "include", "memberships" } });

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Should_Return_Group_With_Users()
        {
            var result = redmineManager.GetObject<Group>("9", new NameValueCollection() { { "include", "users" } });

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetGroup_WithAll_AssociatedData()
        {
            var result = redmineManager.GetObject<Group>("9", new NameValueCollection() { { "include", "memberships, users" } });

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Add_New_Membership_To_Group()
        {
            var group = redmineManager.GetObject<Group>("9", new NameValueCollection() { { "include", "memberships" } });

            var mbs = new Membership();
            mbs.Roles = new List<MembershipRole>();
            mbs.Roles.Add(new MembershipRole()
            {
                Inherited = true,
                Name = "role de test"
            });

            group.Memberships.Add(mbs);

            redmineManager.UpdateObject("9", group);

            var updatedGroup = redmineManager.GetObject<Group>("9", new NameValueCollection() { { "include", "memberships" } });
        }

    }
}