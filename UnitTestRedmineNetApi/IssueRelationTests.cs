using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestRedmineNetApi
{
    [TestClass]
    public class IssueRelationTests
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
        public void RedmineIssueRelation_ShouldReturnRelationsByIssueId()
        {
            var relations = redmineManager.GetObjectList<IssueRelation>(new NameValueCollection { { "issue_id", "19" } });

            Assert.IsNotNull(relations);
        }

        [TestMethod]
        public void RedmineIssueRelation_ShouldAddRelation()
        {
            IssueRelation relation = new IssueRelation();
            relation.IssueToId = 18;
            relation.Type = IssueRelationType.follows;
            relation.Delay = 2;

            IssueRelation savedRelation = redmineManager.CreateObject<IssueRelation>(relation, "19");

            Assert.Inconclusive();
        }

        [TestMethod]
        public void RedmineIssueRelation_ShouldGetRelationById()
        {
            string relationId = "20";

            var relation = redmineManager.GetObject<IssueRelation>(relationId, null);

            Assert.IsNotNull(relation);
        }

        [TestMethod]
        public void RedmineIssueRelation_ShouldDeleteRelation()
        {
            var relationId = "20";

            try
            {
                redmineManager.DeleteObject<IssueRelation>(relationId, null);
            }
            catch (RedmineException)
            {
                Assert.Fail("Relation could not be deleted.");
                return;
            }

            try
            {
                IssueRelation issueRelation = redmineManager.GetObject<IssueRelation>(relationId, null);
            }
            catch (RedmineException exc)
            {
                StringAssert.Contains(exc.Message, "Not Found");
                return;
            }
            Assert.Fail("Test failed");
        }
    }
}
