using System;
using Xunit;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using Redmine.Net.Api.Async;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Redmine.Net.Api.Exceptions;

namespace xUnitTestredminenet45api
{
	[Collection("RedmineCollection")]
	public class WikiPageAsyncTests
	{
		private const string projectId = "redmine-net-testq";
		private const string wikiPageName = "Wiki";
		private const int noOfWikiPages = 1;
		private const int wikiPageVersion = 1;

		private const string wikiPageUpdatedText = "Updated again and again wiki page";
		private const string wikiPageComment = "Comment added through code";

		RedmineFixture fixture;
		public WikiPageAsyncTests(RedmineFixture fixture)
		{
			this.fixture = fixture;
		}

		[Fact]
		public  async Task Should_Add_Or_Update_Page()
		{
			WikiPage page = await fixture.RedmineManager.CreateOrUpdateWikiPageAsync(projectId, wikiPageName, new WikiPage { Text = wikiPageUpdatedText, Comments = wikiPageComment });

			Assert.NotNull(page);
			Assert.True(page.Title == wikiPageName, "Wiki page " + wikiPageName + " does not exist.");
		}

		[Fact]
		public async Task Should_Get_All_Pages()
		{
			List<WikiPage> pages = (List<WikiPage>)await fixture.RedmineManager.GetAllWikiPagesAsync(null, projectId);

			Assert.NotNull(pages);

			Assert.True(pages.Count == noOfWikiPages, "Number of pages != "+noOfWikiPages);
			Assert.True(pages.Exists(p => p.Title == wikiPageName), "Wiki page "+wikiPageName+" does not exist." );
		}

		[Fact]
		public async Task Should_Get_Page_By_Name()
		{
			WikiPage page = await fixture.RedmineManager.GetWikiPageAsync(projectId, new NameValueCollection { { "include", "attachments" } }, wikiPageName);

			Assert.NotNull(page);
			Assert.True(page.Title == wikiPageName, "Wiki page " + wikiPageName + " does not exist.");
		}

		[Fact]
		public async Task Should_Get_Wiki_Page_Old_Version()
		{
			WikiPage oldPage = await fixture.RedmineManager.GetWikiPageAsync(projectId, new NameValueCollection { { "include", "attachments" } }, wikiPageName, wikiPageVersion);

			Assert.True(oldPage.Title == wikiPageName,  "Wiki page " + wikiPageName + " does not exist.");
			Assert.True(oldPage.Version == wikiPageVersion, "Wiki page version " + wikiPageVersion + " does not exist.");
		}

		[Fact]
		public async Task Should_Delete_WikiPage()
		{
			await fixture.RedmineManager.DeleteWikiPageAsync(projectId, wikiPageName);
			await Assert.ThrowsAsync<NotFoundException>(async () => await fixture.RedmineManager.GetWikiPageAsync(projectId, null, wikiPageName));
		}

	}
}

