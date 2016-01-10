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
        #region Constants
        private const int numberOfRoles = 3;

        private const string roleId = "4";
        private const string roleName = "Admins";
        #endregion Constants

        #region Properties
        private RedmineManager redmineManager;
        private string uri;
        private string apiKey;
        #endregion Properties

        #region Initialize
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
        #endregion Initialize

        #region Tests
        [TestMethod]
        public void RedmineRoles_ShouldGetAllRoles()
        {
            var roles = redmineManager.GetObjects<Role>(null);

            Assert.IsTrue(roles.Count == numberOfRoles);
        }

        [TestMethod]
        public void RedmineRoles_ShouldGetRoleById()
        {
            Role adminRole = redmineManager.GetObject<Role>(roleId, null);

            Assert.AreEqual(adminRole.Name, roleName);
        }
        #endregion Tests
    }
}
