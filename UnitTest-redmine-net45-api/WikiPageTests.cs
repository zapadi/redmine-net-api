using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redmine.Net.Api.Async;
using Redmine.Net.Api.Types;
using Redmine.Net.Api;
using System.Diagnostics;
using Redmine.Net.Api.Extensions;
using System.Collections.Specialized;

namespace UnitTest_redmine_net45_api
{
    [TestClass]
    public class WikiPageTests
    {
        private RedmineManager redmineManager;

        private const string projectId = "redmine-net-metro";
        private const string wikiPageName = "Wiki";
        private const int noOfWikiPages = 1;
        private const int wikiPageVersion = 1;

        private const string wikiPageUpdatedText = "Updated again and again wiki page";
        private const string wikiPageComment = "Comment added through code";


        [TestInitialize]
        public void Initialize()
        {
            SetMimeTypeXML();
            SetMimeTypeJSON();
            redmineManager.UseTraceLog();
        }

        [Conditional("JSON")]
        private void SetMimeTypeJSON()
        {
            redmineManager = new RedmineManager(Helper.Uri, Helper.ApiKey, MimeFormat.json);
        }

        [Conditional("XML")]
        private void SetMimeTypeXML()
        {
            redmineManager = new RedmineManager(Helper.Uri, Helper.ApiKey);
        }

        [TestMethod]
        public  async Task Should_Add_Or_Update_Page()
        {
            WikiPage page = await redmineManager.CreateOrUpdateWikiPageAsync(projectId, wikiPageName, new WikiPage { Text = wikiPageUpdatedText, Comments = wikiPageComment });

            Assert.IsNotNull(page, "Wiki page returned is null.");
            Assert.IsTrue(page.Title == wikiPageName, "Wiki page " + wikiPageName + " does not exist.");
        }

        [TestMethod]
        public async Task Should_Get_All_Pages()
        {
            List<WikiPage> pages = (List<WikiPage>)await redmineManager.GetAllWikiPagesAsync(null, projectId);

            Assert.IsNotNull(pages, "Get pages returned null.");
            CollectionAssert.AllItemsAreNotNull(pages, "Pages contains null elements.");
            CollectionAssert.AllItemsAreUnique(pages, "Wiki pages are not unique.");

            Assert.IsTrue(pages.Count == noOfWikiPages, "Number of pages != "+noOfWikiPages);
            Assert.IsTrue(pages.Exists(p => p.Title == wikiPageName), "Wiki page "+wikiPageName+" does not exist." );
        }

        [TestMethod]
        public async Task Should_Get_Page_By_Name()
        {
            WikiPage page = await redmineManager.GetWikiPageAsync(projectId, new NameValueCollection { { "include", "attachments" } }, wikiPageName);

            Assert.IsNotNull(page, "Wiki page returned is null.");
            Assert.IsTrue(page.Title == wikiPageName, "Wiki page " + wikiPageName + " does not exist.");
        }

        [TestMethod]
        public async Task Should_Get_Wiki_Page_Old_Version()
        {
            WikiPage oldPage = await redmineManager.GetWikiPageAsync(projectId, new NameValueCollection { { "include", "attachments" } }, wikiPageName, wikiPageVersion);

            Assert.IsTrue(oldPage.Title == wikiPageName,  "Wiki page " + wikiPageName + " does not exist.");
            Assert.IsTrue(oldPage.Version == wikiPageVersion, "Wiki page version " + wikiPageVersion + " does not exist.");
        }

        [TestMethod]
        public async Task Should_Delete_WikiPage()
        {
            try
            {
                await redmineManager.DeleteWikiPageAsync(projectId, wikiPageName);

                WikiPage page = await redmineManager.GetWikiPageAsync(projectId, null, wikiPageName);
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
