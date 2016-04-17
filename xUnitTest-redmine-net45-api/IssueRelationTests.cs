using System;
using System.Collections.Specialized;
using Xunit;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;

namespace xUnitTestredminenet45api
{
	[Collection("RedmineCollection")]
	public class IssueRelationTests
	{
		private const string ISSUE_ID = "96";
		private const int RELATED_ISSUE_ID = 94;
		private const IssueRelationType RELATION_TYPE = IssueRelationType.follows;
		private const IssueRelationType OPPOSED_RELATION_TYPE = IssueRelationType.precedes;
		private const int RELATION_DELAY = 2;
		private const int NUMBER_OF_RELATIONS = 1;

		private const string RELATION_ID_TO_GET = "27";

		private const string RELATION_ID_TO_DELETE = "23";

		private const string RELATION_ID_TO_COMPARE = "26";

		RedmineFixture fixture;
		public IssueRelationTests (RedmineFixture fixture)
		{
			this.fixture = fixture;
		}

		[Fact]
		public void Should_Add_Issue_Relation()
		{
			IssueRelation relation = new IssueRelation();
			relation.IssueToId = RELATED_ISSUE_ID;
			relation.Type = RELATION_TYPE;
			relation.Delay = RELATION_DELAY;

			IssueRelation savedRelation = fixture.redmineManager.CreateObject<IssueRelation>(relation, ISSUE_ID);

			Assert.NotNull(savedRelation);
			Assert.True(savedRelation.IssueId == RELATED_ISSUE_ID, "Related issue id is not valid.");
			Assert.True(savedRelation.IssueToId.ToString().Equals(ISSUE_ID), "Issue id is not valid.");
			Assert.True(savedRelation.Delay == RELATION_DELAY, "Delay is not valid.");
			Assert.True(savedRelation.Type == OPPOSED_RELATION_TYPE, "Relation type is not valid.");
		}

		[Fact]
		public void Should_Get_IssueRelations_By_Issue_Id()
		{
			var relations = fixture.redmineManager.GetObjects<IssueRelation>(new NameValueCollection { { RedmineKeys.ISSUE_ID, ISSUE_ID } });

			Assert.NotNull(relations);
			Assert.True(relations.Count == NUMBER_OF_RELATIONS, "Number of issue relations != " + NUMBER_OF_RELATIONS);
		}

		[Fact]
		public void Should_Get_IssueRelation_By_Id()
		{
			var relation = fixture.redmineManager.GetObject<IssueRelation>(RELATION_ID_TO_GET, null);

			Assert.NotNull(relation);
			Assert.True(relation.IssueId == RELATED_ISSUE_ID, "Related issue id is not valid.");
			Assert.True(relation.IssueToId.ToString().Equals(ISSUE_ID), "Issue id is not valid.");
			Assert.True(relation.Delay == RELATION_DELAY, "Delay is not valid.");
			Assert.True(relation.Type == OPPOSED_RELATION_TYPE, "Relation type is not valid.");
		}

		[Fact]
		public void Should_Compare_IssueRelations()
		{
			var relation = fixture.redmineManager.GetObject<IssueRelation>(RELATION_ID_TO_COMPARE, null);
			var relationToCompare = fixture.redmineManager.GetObject<IssueRelation>(RELATION_ID_TO_COMPARE, null);

			Assert.True(relation.Equals(relationToCompare), "Issue relations are not equal.");
		}

		[Fact]
		public void Should_Delete_Issue_Relation()
		{
			try
			{
				fixture.redmineManager.DeleteObject<IssueRelation>(RELATION_ID_TO_DELETE, null);
			}
			catch (RedmineException)
			{
				Assert.True(false, "Relation could not be deleted.");
				return;
			}

			try
			{
				fixture.redmineManager.GetObject<IssueRelation>(RELATION_ID_TO_DELETE, null);
			}
			catch (RedmineException exc)
			{
				Assert.Contains(exc.Message, "Not Found");
				return;
			}
			Assert.True(false, "Test failed");
		}
	}
}

