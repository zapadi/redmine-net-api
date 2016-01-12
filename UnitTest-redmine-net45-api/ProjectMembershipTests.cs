using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redmine.Net.Api.Extensions;
using System.Diagnostics;
using Redmine.Net.Api.Types;
using Redmine.Net.Api.Async;

namespace UnitTest_redmine_net45_api
{
    [TestClass]
    public class ProjectMembershipTests
    {
        private RedmineManager redmineManager;

        private const string projectId = "redmine-net-metro";
        private const int newPMUserId = 2;
        private const int newPMRoleId = 4;

        [TestInitialize]
        public void Initialize()
        {
            SetMimeTypeXML();
            SetMimeTypeJSON();
            redmineManager.UseTraceLog();
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
            pm.User = new IdentifiableName { Id = newPMUserId };
            pm.Roles = new List<MembershipRole>();
            pm.Roles.Add(new MembershipRole { Id = newPMRoleId });

            ProjectMembership updatedPM = await redmineManager.CreateObjectAsync<ProjectMembership>(pm, projectId);

            Assert.IsNotNull(updatedPM);
        }
    }
}
