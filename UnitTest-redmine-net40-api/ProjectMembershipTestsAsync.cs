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
using System.Threading;

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
        public void Should_Add_Project_Membership()
        {
            ProjectMembership pm = new ProjectMembership();
            pm.User = new IdentifiableName { Id = USER_ID };
            pm.Roles = new List<MembershipRole>();
            pm.Roles.Add(new MembershipRole { Id = ROLE_ID });

            var updatedPM = redmineManager.CreateObjectAsync<ProjectMembership>(pm, PROJECT_ID);
            var delay = Delay(3000);
            int index = Task.WaitAny(updatedPM, delay);

            if (index == 0)
                Assert.IsNotNull(updatedPM.Result, "Project membership is null.");
            else
                Assert.Fail("Operation timeout.");
        }

        private Task Delay(int milliseconds)       
        {
            var tcs = new TaskCompletionSource<object>();
            new Timer(_ => tcs.SetResult(null)).Change(milliseconds, -1);
            return tcs.Task;
        }
    }
}
