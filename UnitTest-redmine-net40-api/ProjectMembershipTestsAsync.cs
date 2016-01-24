using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redmine.Net.Api.Extensions;

namespace UnitTest_redmine_net40_api
{
    [TestClass]
    public class ProjectMembershipTestsAsync
    {
        private RedmineManager redmineManager;

        private const string PROJECT_ID = "redmine-test";
        private const int USER_ID = 5;
        private const int ROLE_ID = 36;

        [TestInitialize]
        public void Initialize()
        {
            SetMimeTypeXML();
            SetMimeTypeJSON();
        }

        [Conditional("JSON")]
        private void SetMimeTypeJSON()
        {
            redmineManager = new RedmineManager(Helper.Uri, Helper.ApiKey, MimeFormat.json);
        }

        [Conditional("XML")]
        private void SetMimeTypeXML()
        {
            redmineManager = new RedmineManager(Helper.Uri, Helper.ApiKey);
        }

        [TestMethod]
        public async Task Should_Add_Project_Membership()
        {
            ProjectMembership pm = new ProjectMembership();
            pm.User = new IdentifiableName { Id = USER_ID };
            pm.Roles = new List<MembershipRole>();
            pm.Roles.Add(new MembershipRole { Id = ROLE_ID });

            ProjectMembership updatedPM = await redmineManager.CreateObjectAsync<ProjectMembership>(pm, PROJECT_ID);

            Assert.IsNotNull(updatedPM, "Project membership is null.");
        }
    }
}
