using System.IO;
using System.Xml;
using Redmine.Net.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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

        /// <summary>
        ///A test for Desirialize
        ///</summary>
        public void DesirializeTestHelper<T>()
            where T : class
        {
            PrivateObject param0 = new PrivateObject(new RedmineManager(null, null)); // TODO: Initialize to an appropriate value
            RedmineManager_Accessor target = new RedmineManager_Accessor(param0); // TODO: Initialize to an appropriate value

            var actual = target.Desirialize<Issue>(File.ReadAllText(@"d:\Work\Redmine-NET-API\issues.xml"));
        }

        [TestMethod()]
        [DeploymentItem("Redmine.Net.Api.dll")]
        public void DesirializeTest()
        {
            DesirializeTestHelper<GenericParameterHelper>();
        }
    }
}
