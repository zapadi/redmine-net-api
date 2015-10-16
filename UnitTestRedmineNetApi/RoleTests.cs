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

namespace UnitTestRedmineNetApi
{
    [TestClass]
    public class RoleTests
    {
        private RedmineManager redmineManager;
        private string uri;
        private string apiKey;

        [TestInitialize]
        public void Initialize()
        {
            uri = ConfigurationManager.AppSettings["uri"];
            apiKey = ConfigurationManager.AppSettings["apiKey"];

            SetMimeTypeJSON();
            SetMimeTypeXML();
        }

        [Conditional("JSON")]
        private void SetMimeTypeJSON()
        {
            redmineManager = new RedmineManager(uri, apiKey, MimeFormat.json);
        }

        [Conditional("XML")]
        private void SetMimeTypeXML()
        {
            redmineManager = new RedmineManager(uri, apiKey, MimeFormat.xml);
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
