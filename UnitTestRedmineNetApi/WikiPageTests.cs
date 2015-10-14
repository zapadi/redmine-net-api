using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestRedmineNetApi
{
    [TestClass]
    public class WikiPageTests
    {
        private RedmineManager redmineManager;

        [TestInitialize]
        public void Initialize()
        {
            var uri = ConfigurationManager.AppSettings["uri"];
            var apiKey = ConfigurationManager.AppSettings["apiKey"];
            redmineManager = new RedmineManager(uri, apiKey);
        }

        [TestMethod]
        public void RedmineWikiPages_ShouldGetAllPages()
        {
            List<WikiPage> pages = (List<WikiPage>)redmineManager.GetAllWikiPages("9");

            Assert.IsTrue(pages.Count == 1 && pages[0].Title.Equals("Wiki"));
        }

        [TestMethod]
        public void RedmineWikiPages_ShouldReturnWikiPage()
        {
            WikiPage page = redmineManager.GetWikiPage("9", new NameValueCollection { { "include", "attachments" } }, "Wiki");

            Assert.IsTrue(page.Title == "Wiki");
        }

        [TestMethod]
        public void RedmineWikiPages_ShouldReturnOldVersion()
        {
            WikiPage oldPage = redmineManager.GetWikiPage("9", new NameValueCollection { { "include", "attachments" } }, "Wiki", 1);

            Assert.IsTrue(oldPage.Title == "Wiki");
            Assert.IsTrue(oldPage.Version == 1);
        }

        [TestMethod]
        public void RedmineWikiPages_ShouldAddOrUpdatePage()
        {
            WikiPage page = redmineManager.CreateOrUpdateWikiPage("9", "Wiki", new WikiPage { Text = "Updated again and again wiki page", Comments = "I did it through code" });

            Assert.IsTrue(page.Title == "Wiki");
        }

        [TestMethod]
        public void RedmineWikiPages_ShouldDelete()
        {
            redmineManager.DeleteWikiPage("9", "Wiki");

            try
            {
                WikiPage page = redmineManager.GetWikiPage("9", null, "Wiki");
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
