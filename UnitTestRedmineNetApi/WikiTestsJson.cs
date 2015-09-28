using System.Collections.Specialized;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;

namespace UnitTestRedmineNetApi
{
    [TestClass]
    public class WikiTestsJson
    {
        private RedmineManager redmineManager;

        [TestInitialize]
        public void Initialize()
        {
            var uri = ConfigurationManager.AppSettings["uri"];
            var apiKey = ConfigurationManager.AppSettings["apiKey"];
            redmineManager = new RedmineManager(uri, apiKey, MimeFormat.json);
        }

        [TestMethod]
        public void Should_Add_Wiki()
        {
            var result = redmineManager.CreateOrUpdateWikiPage("", "", new WikiPage());
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Should_Remove_Wiki()
        {
            redmineManager.DeleteWikiPage("", "");
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Should_Update_Wiki()
        {
            var wiki = GetWiki("9", null, "Wiki", 2);

            Assert.IsNotNull(wiki,"wiki != null");

            wiki.Text = "text updated";
            wiki.Comments = "comments updated";
            wiki.Title = "am schimbat titlul";
           

            redmineManager.CreateOrUpdateWikiPage("9", "Wiki", wiki);



            Assert.Inconclusive();
        }

        [TestMethod]
        public void Should_Get_Wiki()
        {
            var result = GetWiki("9", null, "Wiki", 0);
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Should_Get_All_Wikis()
        {
            var result = redmineManager.GetAllWikiPages("9");
            Assert.Inconclusive();
        }

        private WikiPage GetWiki(string projectId, NameValueCollection parameters, string pageName, uint version)
        {
            return redmineManager.GetWikiPage(projectId, parameters, pageName, version);
        }
    }
}