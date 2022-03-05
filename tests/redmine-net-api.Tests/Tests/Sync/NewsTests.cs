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
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineApi.Tests.Tests.Sync
{
	[Trait("Redmine-Net-Api", "News")]
#if !(NET20 || NET40)
    [Collection("RedmineCollection")]
#endif
    public class NewsTests
    {
        public NewsTests(RedmineFixture fixture)
        {
            this.fixture = fixture;
        }

        private readonly RedmineFixture fixture;

        [Fact, Order(1)]
        public void Should_Get_All_News()
        {
            const int NUMBER_OF_NEWS = 2;
            var news = fixture.RedmineManager.GetObjects<News>();

            Assert.NotNull(news);
            Assert.True(news.Count == NUMBER_OF_NEWS, "News count(" + news.Count + ") != " + NUMBER_OF_NEWS);
        }

        [Fact, Order(2)]
        public void Should_Get_News_By_Project_Id()
        {
	        const string PROJECT_ID = "redmine-net-testq";
	        const int NUMBER_OF_NEWS_BY_PROJECT_ID = 1;
            var news =
                fixture.RedmineManager.GetObjects<News>(new NameValueCollection {{RedmineKeys.PROJECT_ID, PROJECT_ID}});

            Assert.NotNull(news);
            Assert.True(news.Count == NUMBER_OF_NEWS_BY_PROJECT_ID,
                "News count(" + news.Count + ") != " + NUMBER_OF_NEWS_BY_PROJECT_ID);
        }
    }
}