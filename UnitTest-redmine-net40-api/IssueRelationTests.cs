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
        private RedmineManager redmineManager;

        private const string ISSUE_ID = "1";
        private const int RELATED_ISSUE_ID = 2;
        private const IssueRelationType RELATION_TYPE = IssueRelationType.follows;
        private const IssueRelationType OPPOSED_RELATION_TYPE = IssueRelationType.precedes;
        private const int RELATION_DELAY = 2;
        private const int NUMBER_OF_RELATIONS = 2;

        private const string RELATION_ID_TO_GET = "3";

        private const string RELATION_ID_TO_DELETE = "3";

        private const string RELATION_ID_TO_COMPARE = "3";
  
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
        public void Should_Add_Issue_Relation()
        {
            IssueRelation relation = new IssueRelation();
            relation.IssueToId = RELATED_ISSUE_ID;
            relation.Type = RELATION_TYPE;
            relation.Delay = RELATION_DELAY;

            IssueRelation savedRelation = redmineManager.CreateObject<IssueRelation>(relation, ISSUE_ID);

            Assert.IsNotNull(savedRelation, "Create issue relation returned null.");
            Assert.AreEqual(savedRelation.IssueId, RELATED_ISSUE_ID, "Related issue id is not valid.");
            Assert.AreEqual(savedRelation.IssueToId.ToString(), ISSUE_ID, "Issue id is not valid.");
            Assert.AreEqual(savedRelation.Delay, RELATION_DELAY, "Delay is not valid.");
            Assert.AreEqual(savedRelation.Type, OPPOSED_RELATION_TYPE, "Relation type is not valid.");
        }

        [TestMethod]
        public void Should_Get_IssueRelations_By_Issue_Id()
        {
            var relations = redmineManager.GetObjects<IssueRelation>(new NameValueCollection { { RedmineKeys.ISSUE_ID, ISSUE_ID } });

            Assert.IsNotNull(relations, "Get issue relations by issue id returned null.");
            CollectionAssert.AllItemsAreNotNull(relations, "Relations contains null items.");
            CollectionAssert.AllItemsAreUnique(relations, "Relations items are not unique.");
            Assert.IsTrue(relations.Count == NUMBER_OF_RELATIONS, "Number of issue relations != " + NUMBER_OF_RELATIONS);
        }

        [TestMethod]
        public void Should_Get_IssueRelation_By_Id()
        {
            var relation = redmineManager.GetObject<IssueRelation>(RELATION_ID_TO_GET, null);

            Assert.IsNotNull(relation, "Get issue relation returned null.");
            Assert.AreEqual(relation.IssueId, RELATED_ISSUE_ID, "Related issue id is not valid.");
            Assert.AreEqual(relation.IssueToId.ToString(), ISSUE_ID, "Issue id is not valid.");
            Assert.AreEqual(relation.Delay, RELATION_DELAY, "Delay is not valid.");
            Assert.AreEqual(relation.Type, OPPOSED_RELATION_TYPE, "Relation type is not valid.");
        }

        [TestMethod]
        public void Should_Compare_IssueRelations()
        {
            var relation = redmineManager.GetObject<IssueRelation>(RELATION_ID_TO_COMPARE, null);
            var relationToCompare = redmineManager.GetObject<IssueRelation>(RELATION_ID_TO_COMPARE, null);

            Assert.IsTrue(relation.Equals(relationToCompare), "Issue relations are not equal.");
        }

        [TestMethod]
        public void Should_Delete_Issue_Relation()
        {
           try
            {
                redmineManager.DeleteObject<IssueRelation>(RELATION_ID_TO_DELETE, null);
            }
            catch (RedmineException)
            {
                Assert.Fail("Relation could not be deleted.");
                return;
            }

            try
            {
                IssueRelation issueRelation = redmineManager.GetObject<IssueRelation>(RELATION_ID_TO_DELETE, null);
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
