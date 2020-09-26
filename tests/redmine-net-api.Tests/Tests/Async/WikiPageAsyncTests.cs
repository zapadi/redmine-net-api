#if !(NET20 || NET40)
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Redmine.Net.Api.Async;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineApi.Tests.Tests.Async
{
	[Collection("RedmineCollection")]
	public class WikiPageAsyncTests
	{
		private const string PROJECT_ID = "redmine-net-testq";
		private const string WIKI_PAGE_NAME = "Wiki";
		private const int NO_OF_WIKI_PAGES = 1;
		private const int WIKI_PAGE_VERSION = 1;

		private const string WIKI_PAGE_UPDATED_TEXT = "Updated again and again wiki page";
		private const string WIKI_PAGE_COMMENT = "Comment added through code";

	    private readonly RedmineFixture fixture;
		public WikiPageAsyncTests(RedmineFixture fixture)
		{
			this.fixture = fixture;
		}

		[Fact]
		public  async Task Should_Add_Wiki_Page()
		{
			var page = await fixture.RedmineManager.CreateWikiPageAsync(PROJECT_ID, WIKI_PAGE_NAME, new WikiPage { Text = WIKI_PAGE_UPDATED_TEXT, Comments = WIKI_PAGE_COMMENT });

			Assert.NotNull(page);
			Assert.True(page.Title == WIKI_PAGE_NAME, "Wiki page " + WIKI_PAGE_NAME + " does not exist.");
		}
        
        [Fact]
        public  async Task Should_Update_Wiki_Page()
        {
             await fixture.RedmineManager.UpdateWikiPageAsync(PROJECT_ID, WIKI_PAGE_NAME, new WikiPage { Text = WIKI_PAGE_UPDATED_TEXT, Comments = WIKI_PAGE_COMMENT });
        }

		[Fact]
		public async Task Should_Get_All_Pages()
		{
			var pages = await fixture.RedmineManager.GetAllWikiPagesAsync(null, PROJECT_ID);

			Assert.NotNull(pages);

			Assert.True(pages.Count == NO_OF_WIKI_PAGES, "Number of pages != "+NO_OF_WIKI_PAGES);
			Assert.True(pages.Exists(p => p.Title == WIKI_PAGE_NAME), "Wiki page "+WIKI_PAGE_NAME+" does not exist." );
		}

		[Fact]
		public async Task Should_Get_Page_By_Name()
		{
			var page = await fixture.RedmineManager.GetWikiPageAsync(PROJECT_ID, new NameValueCollection { { "include", "attachments" } }, WIKI_PAGE_NAME);

			Assert.NotNull(page);
			Assert.True(page.Title == WIKI_PAGE_NAME, "Wiki page " + WIKI_PAGE_NAME + " does not exist.");
		}

		[Fact]
		public async Task Should_Get_Wiki_Page_Old_Version()
		{
			var oldPage = await fixture.RedmineManager.GetWikiPageAsync(PROJECT_ID, new NameValueCollection { { "include", "attachments" } }, WIKI_PAGE_NAME, WIKI_PAGE_VERSION);

			Assert.True(oldPage.Title == WIKI_PAGE_NAME,  "Wiki page " + WIKI_PAGE_NAME + " does not exist.");
			Assert.True(oldPage.Version == WIKI_PAGE_VERSION, "Wiki page version " + WIKI_PAGE_VERSION + " does not exist.");
		}

		[Fact]
		public async Task Should_Delete_WikiPage()
		{
			await fixture.RedmineManager.DeleteWikiPageAsync(PROJECT_ID, WIKI_PAGE_NAME);
			await Assert.ThrowsAsync<NotFoundException>(async () => await fixture.RedmineManager.GetWikiPageAsync(PROJECT_ID, null, WIKI_PAGE_NAME));
		}
        
        [Fact]
        public async Task Should_Get_Wiki_Page_With_Special_Chars()
        {
            var wikiPageName = "some-page-with-umlauts-and-other-special-chars-äöüÄÖÜß"; 
            
            var wikiPage = await fixture.RedmineManager.CreateWikiPageAsync(PROJECT_ID, wikiPageName,
                new WikiPage { Text = "WIKI_PAGE_TEXT", Comments = "WIKI_PAGE_COMMENT" });
            
            WikiPage page = await fixture.RedmineManager.GetWikiPageAsync
            (
                PROJECT_ID, 
                null,
                wikiPageName
            );

            Assert.NotNull(page);
            Assert.True(string.Equals(page.Title,wikiPageName, StringComparison.OrdinalIgnoreCase),$"Wiki page {wikiPageName} does not exist.");
        }

	}
}
#endif