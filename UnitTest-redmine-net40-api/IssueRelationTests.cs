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

namespace UnitTest_redmine_net40_api
{
    [TestClass]
    public class IssueRelationTests
    {
        #region Constants
        private const string issueId = "19";

        private const int relatedIssueId = 18;
        private const IssueRelationType relationType = IssueRelationType.follows;
        private const int relationDelay = 2;

        private const string relationIdToGet = "23";

        private const string relationIdToDelete = "23";

        private const string relationIdToCompare = "12";
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
        public void RedmineIssueRelation_ShouldReturnRelationsByIssueId()
        {
            var relations = redmineManager.GetObjects<IssueRelation>(new NameValueCollection { { "issue_id", issueId } });

            Assert.IsNotNull(relations);
        }

        [TestMethod]
        public void RedmineIssueRelation_ShouldAddRelation()
        {
            IssueRelation relation = new IssueRelation();
            relation.IssueToId = relatedIssueId;
            relation.Type = relationType;
            relation.Delay = relationDelay;

            IssueRelation savedRelation = redmineManager.CreateObject<IssueRelation>(relation, issueId);

            Assert.Inconclusive();
        }

        [TestMethod]
        public void RedmineIssueRelation_ShouldGetRelationById()
        {
            var relation = redmineManager.GetObject<IssueRelation>(relationIdToGet, null);

            Assert.IsNotNull(relation);
        }

        [TestMethod]
        public void RedmineIssueRelation_ShouldDeleteRelation()
        {
           try
            {
                redmineManager.DeleteObject<IssueRelation>(relationIdToDelete, null);
            }
            catch (RedmineException)
            {
                Assert.Fail("Relation could not be deleted.");
                return;
            }

            try
            {
                IssueRelation issueRelation = redmineManager.GetObject<IssueRelation>(relationIdToDelete, null);
            }
            catch (RedmineException exc)
            {
                StringAssert.Contains(exc.Message, "Not Found");
                return;
            }
            Assert.Fail("Test failed");
        }

        [TestMethod]
        public void RedmineIssueRelation_ShouldCompare()
        {
            var relation = redmineManager.GetObject<IssueRelation>(relationIdToCompare, null);
            var relationToCompare = redmineManager.GetObject<IssueRelation>(relationIdToCompare, null);

            Assert.IsTrue(relation.Equals(relationToCompare));
        }
        #endregion Tests
    }
}
