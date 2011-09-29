using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Redmine.Net.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Redmine.Net.Api.Types;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace Test
{
    
    
    /// <summary>
    ///This is a test class for RedmineManagerTest and is intended
    ///to contain all RedmineManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RedmineManagerTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for RedmineManager Constructor
        ///</summary>
        [TestMethod()]
        public void RedmineManagerConstructorTest()
        {
            string host = string.Empty; // TODO: Initialize to an appropriate value
            string apiKey = string.Empty; // TODO: Initialize to an appropriate value
            var target = new RedmineManager(host, apiKey);
         //   Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for RedmineManager Constructor
        ///</summary>
        [TestMethod()]
        public void RedmineManagerConstructorTest1()
        {
            string host = string.Empty; // TODO: Initialize to an appropriate value
            string login = string.Empty; // TODO: Initialize to an appropriate value
            string password = string.Empty; // TODO: Initialize to an appropriate value
            var target = new RedmineManager(host, login, password);
          //  Assert.Inconclusive("TODO: Implement code to verify target");
        }

        [TestMethod()]
        [DeploymentItem("Redmine.Net.Api.dll")]
        public void DesirializeTest()
        {
            var sr = new XmlSerializer(typeof (Issue));
            using (var str = new XmlTextReader("../../../TestFiles/issue.xml"))
            {
                var issue = sr.Deserialize(str) as Issue;
            }
        }

        /// <summary>
        ///A test for GetObjectList
        ///</summary>
        public void GetObjectListTestHelper<T>()
            where T : class
        {
            string host = "";
            string apiKey = "";
            RedmineManager target = new RedmineManager(host, apiKey);
            NameValueCollection filters = new NameValueCollection { { "status_Id", "*" } };
            target.GetObjectList<T>(filters);
        }

        [TestMethod()]
        public void GetObjectListTest()
        {
            GetObjectListTestHelper<GenericParameterHelper>();
        }
    }
}
