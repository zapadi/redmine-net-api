using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestRedmineNetApi
{
    [TestClass]
    public class WikiPageTests
    {
        #region Constants
        private const string projectId = "9";
        private const string wikiPageName = "Wiki";
        private const int noOfWikiPages = 1;
        private const int wikiPageVersion = 1;

        private const string wikiPageUpdatedText = "Updated again and again wiki page";
        private const string wikiPageComment = "I did it through code";
        #endregion Constants

        #region Properties
        private RedmineManager redmineManager;
        private string uri;
        private string apiKey;
        #endregion Properties

        #region Initialize
        [TestInitialize]
        public void Initialize()
        {
            uri = ConfigurationManager.AppSettings["uri"];
            apiKey = ConfigurationManager.AppSettings["apiKey"];

            SetMimeTypeJSON();
            SetMimeTypeXML();
        }

        [Conditional("JSON")]
        private void SetMimeTypeJSON()
        {
            redmineManager = new RedmineManager(uri, apiKey, MimeFormat.json);
        }

        [Conditional("XML")]
        private void SetMimeTypeXML()
        {
            redmineManager = new RedmineManager(uri, apiKey, MimeFormat.xml);
        }
        #endregion Initialize

        #region Tests
        [TestMethod]
        public void RedmineWikiPages_ShouldAddOrUpdatePage()
        {
            WikiPage page = redmineManager.CreateOrUpdateWikiPage(projectId, wikiPageName, new WikiPage { Text = wikiPageUpdatedText, Comments = wikiPageComment });

            Assert.IsTrue(page.Title == wikiPageName);
        }

        [TestMethod]
        public void RedmineWikiPages_ShouldGetAllPages()
        {
            List<WikiPage> pages = (List<WikiPage>)redmineManager.GetAllWikiPages(projectId);

            Assert.IsTrue(pages.Count == noOfWikiPages && pages[0].Title.Equals(wikiPageName));
        }

        [TestMethod]
        public void RedmineWikiPages_ShouldReturnWikiPage()
        {
            WikiPage page = redmineManager.GetWikiPage(projectId, new NameValueCollection { { "include", "attachments" } }, wikiPageName);

            Assert.IsTrue(page.Title == wikiPageName);
        }

        [TestMethod]
        public void RedmineWikiPages_ShouldReturnOldVersion()
        {
            WikiPage oldPage = redmineManager.GetWikiPage(projectId, new NameValueCollection { { "include", "attachments" } }, wikiPageName, wikiPageVersion);

            Assert.IsTrue(oldPage.Title == wikiPageName);
            Assert.IsTrue(oldPage.Version == wikiPageVersion);
        }

        [TestMethod]
        public void RedmineWikiPages_ShouldDelete()
        {
            redmineManager.DeleteWikiPage(projectId, wikiPageName);

            try
            {
                WikiPage page = redmineManager.GetWikiPage(projectId, null, wikiPageName);
            }
            catch (RedmineException exc)
            {
                StringAssert.Contains(exc.Message, "Not Found");
                return;
            }
            Assert.Fail("Test failed");

        }
        #endregion Tests
    }
}
