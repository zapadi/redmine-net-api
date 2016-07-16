/*
   Copyright 2011 - 2016 Adrian Popescu.

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

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Redmine.Net.Api;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Types;
using Xunit;

namespace xUnitTestredminenet45api
{
	[Trait("Redmine-Net-Api", "WikiPages")]
	[Collection("RedmineCollection")]
    public class WikiPageTests
    {
        public WikiPageTests(RedmineFixture fixture)
        {
            this.fixture = fixture;
        }

	    private readonly RedmineFixture fixture;

	    private const string PROJECT_ID = "redmine-net-api";
        private const string WIKI_PAGE_NAME = "Wiki";

        [Fact, Order(1)]
        public void Should_Add_Or_Update_WikiPage()
        {
	        const string WIKI_PAGE_UPDATED_TEXT = "Updated again and again wiki page";
	        const string WIKI_PAGE_COMMENT = "I did it through code";

	        var page = fixture.RedmineManager.CreateOrUpdateWikiPage(PROJECT_ID, WIKI_PAGE_NAME,
                new WikiPage {Text = WIKI_PAGE_UPDATED_TEXT, Comments = WIKI_PAGE_COMMENT});

            Assert.NotNull(page);
            Assert.True(page.Title.Equals(WIKI_PAGE_NAME), "Wiki page name is invalid.");
            Assert.True(page.Text.Equals(WIKI_PAGE_UPDATED_TEXT), "Wiki page text is invalid.");
            Assert.True(page.Comments.Equals(WIKI_PAGE_COMMENT), "Wiki page comments are invalid.");
        }

        [Fact, Order(99)]
        public void Should_Delete_Wiki_Page()
        {
            fixture.RedmineManager.DeleteWikiPage(PROJECT_ID, WIKI_PAGE_NAME);
            Assert.Throws<NotFoundException>(() => fixture.RedmineManager.GetWikiPage(PROJECT_ID, null, WIKI_PAGE_NAME));
        }

        [Fact, Order(2)]
        public void Should_Get_All_Wiki_Pages_By_Project_Id()
        {
	        const int NUMBER_OF_WIKI_PAGES = 2;

	        var pages = (List<WikiPage>) fixture.RedmineManager.GetAllWikiPages(PROJECT_ID);

            Assert.NotNull(pages);
            Assert.All(pages, p => Assert.IsType<WikiPage>(p));
            Assert.True(pages.Count == NUMBER_OF_WIKI_PAGES, "Wiki pages count != " + NUMBER_OF_WIKI_PAGES);
            Assert.True(pages.Exists(p => p.Title == WIKI_PAGE_NAME),
                string.Format("Wiki page {0} does not exist", WIKI_PAGE_NAME));
        }

        [Fact, Order(3)]
        public void Should_Get_Wiki_Page_By_Title()
        {
	        const string WIKI_PAGE_TITLE = "Wiki2";

	        var page = fixture.RedmineManager.GetWikiPage(PROJECT_ID, null, WIKI_PAGE_TITLE);

            Assert.NotNull(page);
            Assert.True(page.Title.Equals(WIKI_PAGE_TITLE), "Wiki page title is invalid.");
        }

        [Fact, Order(4)]
        public void Should_Get_Wiki_Page_By_Title_With_Attachments()
        {
            var page = fixture.RedmineManager.GetWikiPage(PROJECT_ID,
                new NameValueCollection {{RedmineKeys.INCLUDE, RedmineKeys.ATTACHMENTS}}, WIKI_PAGE_NAME);

            Assert.NotNull(page);
            Assert.Equal(page.Title, WIKI_PAGE_NAME);
            Assert.NotNull(page.Attachments.ToList());
            Assert.All(page.Attachments.ToList(), a => Assert.IsType<Attachment>(a));
        }

        [Fact, Order(5)]
        public void Should_Get_Wiki_Page_By_Version()
        {
	        const int WIKI_PAGE_VERSION = 1;
	        var oldPage = fixture.RedmineManager.GetWikiPage(PROJECT_ID, null, WIKI_PAGE_NAME, WIKI_PAGE_VERSION);

            Assert.NotNull(oldPage);
            Assert.Equal(oldPage.Title, WIKI_PAGE_NAME);
            Assert.True(oldPage.Version == WIKI_PAGE_VERSION, "Wiki page version is invalid.");
        }
    }
}