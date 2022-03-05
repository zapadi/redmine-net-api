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

using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineApi.Tests.Tests.Sync
{
	[Trait("Redmine-Net-Api", "IssuePriorities")]
#if !(NET20 || NET40)
    [Collection("RedmineCollection")]
#endif
    public class IssuePriorityTests
    {
        public IssuePriorityTests(RedmineFixture fixture)
        {
            this.fixture = fixture;
        }

        private readonly RedmineFixture fixture;

        [Fact]
        public void Should_Get_All_Issue_Priority()
        {
            const int NUMBER_OF_ISSUE_PRIORITIES = 4;
            var issuePriorities = fixture.RedmineManager.GetObjects<IssuePriority>();

            Assert.NotNull(issuePriorities);
            Assert.True(issuePriorities.Count == NUMBER_OF_ISSUE_PRIORITIES,
                "Issue priorities count(" + issuePriorities.Count + ") != " + NUMBER_OF_ISSUE_PRIORITIES);
        }
    }
}