using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestRedmineNetApi
{
    [TestClass]
    public class RoleTests
    {
        private RedmineManager redmineManager;

        [TestInitialize]
        public void Initialize()
        {
            var uri = ConfigurationManager.AppSettings["uri"];
            var apiKey = ConfigurationManager.AppSettings["apiKey"];
            var mimeFormat = (ConfigurationManager.AppSettings["mimeFormat"].Equals("xml")) ? MimeFormat.xml : MimeFormat.json;
            redmineManager = new RedmineManager(uri, apiKey, mimeFormat);
        }

        [TestMethod]
        public void RedmineRoles_ShouldGetAllRoles()
        {
            var roles = redmineManager.GetObjectList<Role>(null);

            Assert.IsTrue(roles.Count == 2);
        }

        [TestMethod]
        public void RedmineRoles_ShouldGetRoleById()
        {
            var roleId = 4;

            Role adminRole = redmineManager.GetObject<Role>(roleId.ToString(), null);

            Assert.AreEqual(adminRole.Name, "Admins");
        }
    }
}
