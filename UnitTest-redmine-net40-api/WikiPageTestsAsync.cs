using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redmine.Net.Api.Extensions;

namespace UnitTest_redmine_net40_api
{
    [TestClass]
    public class WikiPageTestsAsync
    {
        private RedmineManager redmineManager;

        private const string PROJECT_ID = "redmine-test";
        private const string WIKI_PAGE_NAME = "Wiki";
        private const string WIKI_PAGE_UPDATED_TEXT = "Updated again and again wiki page";
        private const string WIKI_PAGE_COMMENT = "I did it through code";

        private const int NUMBER_OF_WIKI_PAGES = 1;
        private const int WIKI_PAGE_VERSION = 1;

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
        public async Task Should_Add_Or_Update_WikiPage()
        {
            WikiPage page = await redmineManager.CreateOrUpdateWikiPageAsync(PROJECT_ID, WIKI_PAGE_NAME, new WikiPage { Text = WIKI_PAGE_UPDATED_TEXT, Comments = WIKI_PAGE_COMMENT });

            Assert.IsNotNull(page, "Wiki page is null.");
            Assert.AreEqual(page.Title, WIKI_PAGE_NAME, "Wiki page name is invalid.");
            Assert.AreEqual(page.Text, WIKI_PAGE_UPDATED_TEXT, "Wiki page text is invalid.");
            Assert.AreEqual(page.Comments, WIKI_PAGE_COMMENT, "Wiki page comments are invalid.");
        }

        [TestMethod]
        public async Task Should_Get_All_Wiki_Pages_By_Project_Id()
        {
            PaginatedObjects<WikiPage> pages = (PaginatedObjects<WikiPage>) await redmineManager.GetAllWikiPagesAsync(null, PROJECT_ID);

            Assert.IsNotNull(pages, "Object list is null.");
            Assert.IsNotNull(pages.Objects, "Wiki pages list is null.");
            CollectionAssert.AllItemsAreNotNull(pages.Objects, "Wiki pages list contains null elements.");
            CollectionAssert.AllItemsAreUnique(pages.Objects, "Wiki pages are not unique.");
            CollectionAssert.AllItemsAreInstancesOfType(pages.Objects, typeof(WikiPage), "Not all pages are of type WikiPage.");
            Assert.IsTrue(pages.Objects.Count == NUMBER_OF_WIKI_PAGES, "Wiki pages count != " + NUMBER_OF_WIKI_PAGES);
            Assert.IsTrue(pages.Objects.Exists(p => p.Title == WIKI_PAGE_NAME), string.Format("Wiki page {0} does not exist", WIKI_PAGE_NAME));
        }

        [TestMethod]
        public async Task Should_Get_Wiki_Page_By_Title()
        {
            WikiPage page = await redmineManager.GetWikiPageAsync(PROJECT_ID, null, WIKI_PAGE_NAME);

            Assert.IsNotNull(page, "Wiki page is null.");
            Assert.AreEqual(page.Title, WIKI_PAGE_NAME, "Wiki page name is invalid.");
        }

        [TestMethod]
        public async Task Should_Get_Wiki_Page_By_Title_With_Attachments()
        {
            WikiPage page = await redmineManager.GetWikiPageAsync(PROJECT_ID, new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.ATTACHMENTS } }, WIKI_PAGE_NAME);

            Assert.IsNotNull(page, "Wiki page is null.");
            Assert.AreEqual(page.Title, WIKI_PAGE_NAME, "Wiki page name is invalid.");
            Assert.IsNotNull(page.Attachments.ToList(), "Attachments list is null.");
            CollectionAssert.AllItemsAreNotNull(page.Attachments.ToList(), "Wiki page attachments list contains null elements.");
            CollectionAssert.AllItemsAreUnique(page.Attachments.ToList(), "Wiki page attachments are not unique.");
            CollectionAssert.AllItemsAreInstancesOfType(page.Attachments.ToList(), typeof(Attachment), "Not all attachments are of type Attachment.");
        }

        [TestMethod]
        public async Task Should_Get_Wiki_Page_By_Version()
        {
            WikiPage oldPage = await redmineManager.GetWikiPageAsync(PROJECT_ID, null, WIKI_PAGE_NAME, WIKI_PAGE_VERSION);

            Assert.IsNotNull(oldPage, "Wiki page is null.");
            Assert.AreEqual(oldPage.Title, WIKI_PAGE_NAME, "Wiki page name is invalid.");
            Assert.IsTrue(oldPage.Version == WIKI_PAGE_VERSION, "Wiki page version is invalid.");
        }

        [TestMethod]
        public async Task Should_Delete_Wiki_Page()
        {
           await redmineManager.DeleteWikiPageAsync(PROJECT_ID, WIKI_PAGE_NAME);

            try
            {
                WikiPage page = await redmineManager.GetWikiPageAsync(PROJECT_ID, null, WIKI_PAGE_NAME);
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
