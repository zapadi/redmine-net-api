using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;

namespace Test
{
    /// <summary>
    ///This is a test class for RedmineManagerTest and is intended
    ///to contain all RedmineManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RedmineManagerTest
    {
        string host = "";
        string apiKey = "";


        /// <summary>
        ///A test for GetObject
        ///</summary>
        public void GetObjectTestHelper<T>()
            where T : class, new()
        {

            RedmineManager target = new RedmineManager(host, apiKey); // TODO: Initialize to an appropriate value
            string id = "27"; // TODO: Initialize to an appropriate value
            NameValueCollection parameters = null; // TODO: Initialize to an appropriate value
            T expected = null; // TODO: Initialize to an appropriate value

            T actual;
            actual = target.GetObject<T>(id, parameters);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [TestMethod()]
        public void GetObjectTest()
        {
                 
           GetObjectTestHelper<Issue>();
        }

        /// <summary>
        ///A test for GetObjectList
        ///</summary>
        public void GetObjectListTestHelper<T>()
            where T : class, new()
        {
            RedmineManager target = new RedmineManager(host, apiKey); // TODO: Initialize to an appropriate value
            NameValueCollection parameters = null; // TODO: Initialize to an appropriate value

            var actual = target.GetObjectList<T>(parameters);
        }

        [TestMethod()]
        public void GetObjectListTest()
        {
            GetObjectListTestHelper<Issue>();
        }
    }
}