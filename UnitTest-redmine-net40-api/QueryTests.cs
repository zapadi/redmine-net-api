using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest_redmine_net40_api
{
    [TestClass]
    public class QueryTests
    {
        private RedmineManager redmineManager;
        
        private const int NUMBER_OF_QUERIES = 3;
        private const bool EXISTS_PUBLIC_QUERY = true;
 
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
        public void Should_Get_All_Queries()
        {
            var queries = redmineManager.GetObjects<Query>(null);

            Assert.IsNotNull(queries, "Get all queries returned null");
            Assert.IsTrue(queries.Count == NUMBER_OF_QUERIES, "Queries count != " + NUMBER_OF_QUERIES);
            CollectionAssert.AllItemsAreNotNull(queries, "Queries list contains null items.");
            CollectionAssert.AllItemsAreUnique(queries, "Queries items are not unique.");
            CollectionAssert.AllItemsAreInstancesOfType(queries, typeof(Query), "Not all items are of type query.");

            Assert.IsTrue(queries.Exists(q => q.IsPublic) == EXISTS_PUBLIC_QUERY, EXISTS_PUBLIC_QUERY ? "Public query should exist." : "Public query should not exist.");
        }
    }
}
