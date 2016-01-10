using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;
using Redmine.Net.Api.Async;
using Redmine.Net.Api.Types;

namespace UnitTest_redmine_net20_api
{
    [TestClass]
    public class UnitTest1
    {
        private RedmineManager redmineManager;

        [TestInitialize]
        public void Initialize()
        {
            redmineManager = new RedmineManager(Helper.Uri, Helper.ApiKey);
        }

        [TestMethod]
        public void ShouldReturnCurrentUser()
        {
            User currentUser = redmineManager.GetCurrentUserAsync()();
            Assert.AreEqual(currentUser.ApiKey, Helper.ApiKey);
        }
    }
}