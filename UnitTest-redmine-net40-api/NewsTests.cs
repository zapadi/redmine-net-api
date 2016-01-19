using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System.Collections.Specialized;
using System.Diagnostics;

namespace UnitTest_redmine_net40_api
{
    [TestClass]
    public class NewsTests
    {
        private RedmineManager redmineManager;

        private const int NUMBER_OF_NEWS = 5;
        private const string PROJECT_ID = "redmine-test";
        private const int NUMBER_OF_NEWS_BY_PROJECT_ID = 3;
 
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
        public void Should_Get_All_News()
        {
            var news = redmineManager.GetObjects<News>(null);

            Assert.IsNotNull(news, "Get all news returned null");
            Assert.IsTrue(news.Count == NUMBER_OF_NEWS, "News count != " + NUMBER_OF_NEWS);
            CollectionAssert.AllItemsAreNotNull(news, "News list contains null items.");
            CollectionAssert.AllItemsAreUnique(news, "News items are not unique.");
            CollectionAssert.AllItemsAreInstancesOfType(news, typeof(News), "Not all items are of type news.");
        }

        [TestMethod]
        public void Should_Get_News_By_Project_Id()
        {
            var news = redmineManager.GetObjects<News>(new NameValueCollection { { RedmineKeys.PROJECT_ID, PROJECT_ID } });

            Assert.IsNotNull(news, "Get all news returned null");
            Assert.IsTrue(news.Count == NUMBER_OF_NEWS_BY_PROJECT_ID, "News count != " + NUMBER_OF_NEWS_BY_PROJECT_ID);
            CollectionAssert.AllItemsAreNotNull(news, "News list contains null items.");
            CollectionAssert.AllItemsAreUnique(news, "News items are not unique.");
            CollectionAssert.AllItemsAreInstancesOfType(news, typeof(News), "Not all items are of type news.");
        }

        [TestMethod]
        public void Should_Compare_News()
        {
            var firstNews = redmineManager.GetPaginatedObjects<News>(new NameValueCollection() {{RedmineKeys.LIMIT, "1" },{RedmineKeys.OFFSET, "0" }});
            var secondNews = redmineManager.GetPaginatedObjects<News>(new NameValueCollection() { { RedmineKeys.LIMIT, "1" }, { RedmineKeys.OFFSET, "0" } });

            Assert.IsNotNull(firstNews, "Get first news returned null.");
            Assert.IsNotNull(firstNews.Objects, "Get first news returned null objects list.");
            Assert.IsTrue(firstNews.Objects.Count == 1, "First news objects list count != 1");

            Assert.IsNotNull(secondNews, "Get second news returned null.");
            Assert.IsNotNull(secondNews.Objects, "Get second news returned null objects list.");
            Assert.IsTrue(secondNews.Objects.Count == 1, "Second news objects list count != 1");

            Assert.IsTrue(firstNews.Objects[0].Equals(secondNews.Objects[0]), "Compared news are not equal.");
        }
    }
}
