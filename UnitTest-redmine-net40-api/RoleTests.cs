using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest_redmine_net40_api
{
    [TestClass]
    public class RoleTests
    {
        private RedmineManager redmineManager;

        private const int NUMBER_OF_ROLES = 3;
        private const string ROLE_ID = "35";
        private const string ROLE_NAME = "Developer";
        private const int NUMBER_OF_ROLE_PERMISSIONS = 29;

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
        public void Should_Get_All_Roles()
        {
            var roles = redmineManager.GetObjects<Role>(null);

            Assert.IsNotNull(roles, "Get all roles returned null");
            Assert.IsTrue(roles.Count == NUMBER_OF_ROLES, "Roles count != " + NUMBER_OF_ROLES);
            CollectionAssert.AllItemsAreNotNull(roles, "Roles list contains null items.");
            CollectionAssert.AllItemsAreUnique(roles, "Roles items are not unique.");
            CollectionAssert.AllItemsAreInstancesOfType(roles, typeof(Role), "Not all items are of type role.");
        }

        [TestMethod]
        public void Should_Get_Role_By_Id()
        {
            Role role = redmineManager.GetObject<Role>(ROLE_ID, null);

            Assert.IsNotNull(role, "Get role returned null.");
            Assert.AreEqual(role.Name, ROLE_NAME, "Role name is invalid."); ;
            Assert.IsNotNull(role.Permissions, "Role permisions list is null.");
            CollectionAssert.AllItemsAreNotNull(role.Permissions.ToList(), "Permissions contains null items.");
            CollectionAssert.AllItemsAreUnique(role.Permissions.ToList(), "Permissions are not unique.");
            Assert.IsTrue(role.Permissions.Count == NUMBER_OF_ROLE_PERMISSIONS, "Permissions count != "+NUMBER_OF_ROLE_PERMISSIONS);
        }
    }
}
