using System;
using System.Collections.Specialized;
using Xunit;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;

namespace xUnitTestredminenet45api
{
	[Collection("RedmineCollection")]
	public class NewsTests
	{
		private const int NUMBER_OF_NEWS = 2;
		private const string PROJECT_ID = "redmine-net-testq";
		private const int NUMBER_OF_NEWS_BY_PROJECT_ID = 1;

		RedmineFixture fixture;
		public NewsTests (RedmineFixture fixture)
		{
			this.fixture = fixture;
		}

		[Fact]
		public void Should_Get_All_News()
		{
			var news = fixture.RedmineManager.GetObjects<News>(null);

			Assert.NotNull(news);
			Assert.True(news.Count == NUMBER_OF_NEWS, "News count != " + NUMBER_OF_NEWS);
			Assert.All (news, n => Assert.IsType<News> (n));
		}

		[Fact]
		public void Should_Get_News_By_Project_Id()
		{
			var news = fixture.RedmineManager.GetObjects<News>(new NameValueCollection { { RedmineKeys.PROJECT_ID, PROJECT_ID } });

			Assert.NotNull(news);
			Assert.True(news.Count == NUMBER_OF_NEWS_BY_PROJECT_ID, "News count != " + NUMBER_OF_NEWS_BY_PROJECT_ID);
			Assert.All(news, n => Assert.IsType<News> (n));
		}

		[Fact]
		public void Should_Compare_News()
		{
			var firstNews = fixture.RedmineManager.GetPaginatedObjects<News>(new NameValueCollection() {{RedmineKeys.LIMIT, "1" },{RedmineKeys.OFFSET, "0" }});
			var secondNews = fixture.RedmineManager.GetPaginatedObjects<News>(new NameValueCollection() { { RedmineKeys.LIMIT, "1" }, { RedmineKeys.OFFSET, "0" } });

			Assert.NotNull(firstNews);
			Assert.NotNull(firstNews.Objects);
			Assert.True(firstNews.Objects.Count == 1, "First news objects list count != 1");

			Assert.NotNull(secondNews);
			Assert.NotNull(secondNews.Objects);
			Assert.True(secondNews.Objects.Count == 1, "Second news objects list count != 1");

			Assert.True(firstNews.Objects[0].Equals(secondNews.Objects[0]), "Compared news are not equal.");
		}
	}
}

