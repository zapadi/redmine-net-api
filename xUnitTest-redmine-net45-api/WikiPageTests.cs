using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Redmine.Net.Api;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Types;
using Xunit;

namespace xUnitTestredminenet45api
{
    [Collection("RedmineCollection")]
    public class WikiPageTests
    {
        private const string PROJECT_ID = "redmine-net-api";
        private const string WIKI_PAGE_NAME = "Wiki";
        private const string WIKI_PAGE_UPDATED_TEXT = "Updated again and again wiki page";
        private const string WIKI_PAGE_COMMENT = "I did it through code";

        private const int NUMBER_OF_WIKI_PAGES = 2;
        private const int WIKI_PAGE_VERSION = 1;

        private const string WIKI_PAGE_TITLE = "Wiki2";

        private readonly RedmineFixture fixture;

        public WikiPageTests(RedmineFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void Should_Add_Or_Update_WikiPage()
        {
            var page = fixture.Manager.CreateOrUpdateWikiPage(PROJECT_ID, WIKI_PAGE_NAME,
                new WikiPage {Text = WIKI_PAGE_UPDATED_TEXT, Comments = WIKI_PAGE_COMMENT});

            Assert.NotNull(page);
            Assert.True(page.Title.Equals(WIKI_PAGE_NAME), "Wiki page name is invalid.");
            Assert.True(page.Text.Equals(WIKI_PAGE_UPDATED_TEXT), "Wiki page text is invalid.");
            Assert.True(page.Comments.Equals(WIKI_PAGE_COMMENT), "Wiki page comments are invalid.");
        }

        [Fact]
        public void Should_Get_All_Wiki_Pages_By_Project_Id()
        {
            var pages = (List<WikiPage>) fixture.Manager.GetAllWikiPages(PROJECT_ID);

            Assert.NotNull(pages);
            Assert.All(pages, p => Assert.IsType<WikiPage>(p));
            Assert.True(pages.Count == NUMBER_OF_WIKI_PAGES, "Wiki pages count != " + NUMBER_OF_WIKI_PAGES);
            Assert.True(pages.Exists(p => p.Title == WIKI_PAGE_NAME),
                string.Format("Wiki page {0} does not exist", WIKI_PAGE_NAME));
        }

        [Fact]
        public void Should_Get_Wiki_Page_By_Title()
        {
            var page = fixture.Manager.GetWikiPage(PROJECT_ID, null, WIKI_PAGE_TITLE);

            Assert.NotNull(page);
            Assert.True(page.Title.Equals(WIKI_PAGE_TITLE), "Wiki page name is invalid.");
        }

        [Fact]
        public void Should_Get_Wiki_Page_By_Title_With_Attachments()
        {
            var page = fixture.Manager.GetWikiPage(PROJECT_ID,
                new NameValueCollection {{RedmineKeys.INCLUDE, RedmineKeys.ATTACHMENTS}}, WIKI_PAGE_NAME);

            Assert.NotNull(page);
            Assert.Equal(page.Title, WIKI_PAGE_NAME);
            Assert.NotNull(page.Attachments.ToList());
            Assert.All(page.Attachments.ToList(), a => Assert.IsType<Attachment>(a));
        }

        [Fact]
        public void Should_Get_Wiki_Page_By_Version()
        {
            var oldPage = fixture.Manager.GetWikiPage(PROJECT_ID, null, WIKI_PAGE_NAME, WIKI_PAGE_VERSION);

            Assert.NotNull(oldPage);
            Assert.Equal(oldPage.Title, WIKI_PAGE_NAME);
            Assert.True(oldPage.Version == WIKI_PAGE_VERSION, "Wiki page version is invalid.");
        }

        [Fact]
        public void Should_Compare_Wiki_Pages()
        {
            var page = fixture.Manager.GetWikiPage(PROJECT_ID,
                new NameValueCollection {{RedmineKeys.INCLUDE, RedmineKeys.ATTACHMENTS}}, WIKI_PAGE_NAME);
            var pageToCompare = fixture.Manager.GetWikiPage(PROJECT_ID,
                new NameValueCollection {{RedmineKeys.INCLUDE, RedmineKeys.ATTACHMENTS}}, WIKI_PAGE_NAME);

            Assert.NotNull(page);
            Assert.True(page.Equals(pageToCompare), "Wiki pages are not equal.");
        }

        [Fact]
        public void Should_Delete_Wiki_Page()
        {
            fixture.Manager.DeleteWikiPage(PROJECT_ID, WIKI_PAGE_NAME);
            Assert.Throws<NotFoundException>(() => fixture.Manager.GetWikiPage(PROJECT_ID, null, WIKI_PAGE_NAME));
        }
    }
}