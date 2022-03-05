/*
   Copyright 2011 - 2022 Adrian Popescu

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System.Collections.Specialized;
using Padi.RedmineApi.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineApi.Tests.Tests.Sync
{
	[Trait("Redmine-Net-Api", "IssueRelations")]
#if !(NET20 || NET40)
    [Collection("RedmineCollection")]
#endif
    public class IssueRelationTests
    {
        public IssueRelationTests(RedmineFixture fixture)
        {
            this.fixture = fixture;
        }

	    private readonly RedmineFixture fixture;

	    private const string ISSUE_ID = "96";
        private const int RELATED_ISSUE_ID = 94;
        private const int RELATION_DELAY = 2;

        private const IssueRelationType OPPOSED_RELATION_TYPE = IssueRelationType.Precedes;

        [Fact, Order(1)]
        public void Should_Add_Issue_Relation()
        {
	        const IssueRelationType RELATION_TYPE = IssueRelationType.Follows;
	        var relation = new IssueRelation
            {
                IssueToId = RELATED_ISSUE_ID,
                Type = RELATION_TYPE,
                Delay = RELATION_DELAY
            };

            var savedRelation = fixture.RedmineManager.CreateObject(relation, ISSUE_ID);

            Assert.NotNull(savedRelation);
            Assert.True(savedRelation.IssueId == RELATED_ISSUE_ID, "Related issue id is not valid.");
            Assert.True(savedRelation.IssueToId.ToString().Equals(ISSUE_ID), "Issue id is not valid.");
            Assert.True(savedRelation.Delay == RELATION_DELAY, "Delay is not valid.");
            Assert.True(savedRelation.Type == OPPOSED_RELATION_TYPE, "Relation type is not valid.");
        }

        [Fact, Order(4)]
        public void Should_Delete_Issue_Relation()
        {
	        const string RELATION_ID_TO_DELETE = "23";
	        var exception =
                (RedmineException)
                    Record.Exception(
                        () => fixture.RedmineManager.DeleteObject<IssueRelation>(RELATION_ID_TO_DELETE));
            Assert.Null(exception);
            Assert.Throws<NotFoundException>(
                () => fixture.RedmineManager.GetObject<IssueRelation>(RELATION_ID_TO_DELETE, null));
        }

        [Fact, Order(2)]
        public void Should_Get_IssueRelation_By_Id()
        {
	        const string RELATION_ID_TO_GET = "27";
	        var relation = fixture.RedmineManager.GetObject<IssueRelation>(RELATION_ID_TO_GET, null);

            Assert.NotNull(relation);
            Assert.True(relation.IssueId == RELATED_ISSUE_ID, "Related issue id is not valid.");
            Assert.True(relation.IssueToId.ToString().Equals(ISSUE_ID), "Issue id is not valid.");
            Assert.True(relation.Delay == RELATION_DELAY, "Delay is not valid.");
            Assert.True(relation.Type == OPPOSED_RELATION_TYPE, "Relation type is not valid.");
        }

        [Fact, Order(3)]
        public void Should_Get_IssueRelations_By_Issue_Id()
        {
	        const int NUMBER_OF_RELATIONS = 1;
	        var relations =
                fixture.RedmineManager.GetObjects<IssueRelation>(new NameValueCollection
                {
                    {RedmineKeys.ISSUE_ID, ISSUE_ID}
                });

            Assert.NotNull(relations);
            Assert.True(relations.Count == NUMBER_OF_RELATIONS, "Number of issue relations ( "+relations.Count+" ) != " + NUMBER_OF_RELATIONS);
        }
    }
}