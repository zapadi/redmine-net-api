using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using Redmine.Net.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Redmine.Net.Api.Types;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Web;

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
        ///A test for CreateObject
        ///</summary>
        public void CreateObjectTestHelper<T>()
            where T : class
        {
            RedmineManager target = new RedmineManager(host, apiKey);
            var obj = new Project { Identifier = "test project", Name = "testproject" };

            target.CreateObject<Project>(obj);
        }

        [TestMethod()]
        public void CreateObjectTest()
        {
            CreateObjectTestHelper<GenericParameterHelper>();
        }

        /// <summary>
        ///A test for DeleteObject
        ///</summary>
        public void DeleteObjectTestHelper<T>()
            where T : class
        {

            RedmineManager target = new RedmineManager(host, apiKey); // TODO: Initialize to an appropriate value
            string id = "27"; // TODO: Initialize to an appropriate value
            NameValueCollection parameters = null; // TODO: Initialize to an appropriate value
            target.DeleteObject<Project>(id, parameters);
        }

        [TestMethod()]
        public void DeleteObjectTest()
        {
            DeleteObjectTestHelper<GenericParameterHelper>();
        }

        /// <summary>
        ///A test for GetObject
        ///</summary>
        public void GetObjectTestHelper<T>()
            where T : class
        {

            RedmineManager target = new RedmineManager(host, apiKey); // TODO: Initialize to an appropriate value
            string id = "27"; // TODO: Initialize to an appropriate value
            NameValueCollection parameters = null; // TODO: Initialize to an appropriate value
            T expected = null; // TODO: Initialize to an appropriate value
            Project actual;
            actual = target.GetObject<Project>(id, parameters);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [TestMethod()]
        public void GetObjectTest()
        {
            var xml = File.ReadAllText("issues.xml");

            using (var text = new StringReader(xml))
            {
                using (var xmlReader = new XmlTextReader(text))
                {
                    var diverse = RedmineManager.Deserialize<Issue>(xml);
                }
            }

            GetObjectTestHelper<GenericParameterHelper>();
        }

        /// <summary>
        ///A test for GetObjectList
        ///</summary>
        public void GetObjectListTestHelper<T>()
            where T : class
        {
            RedmineManager target = new RedmineManager(host, apiKey); // TODO: Initialize to an appropriate value
            NameValueCollection parameters = null; // TODO: Initialize to an appropriate value

            var actual = target.GetObjectList<Issue>(parameters);
        }

        [TestMethod()]
        public void GetObjectListTest()
        {
            GetObjectListTestHelper<CustomField>();
        }

        /// <summary>
        ///A test for UpdateObject
        ///</summary>
        public void UpdateObjectTestHelper<T>()
            where T : class
        {

            RedmineManager target = new RedmineManager(host, apiKey);
            string id = "90";
            var obj = new Issue();
            obj.Subject = "test subject issue";
            obj.Project = new IdentifiableName { Id = 25 };
            target.UpdateObject<Issue>(id, obj);
        }

        [TestMethod()]
        public void UpdateObjectTest()
        {
            UpdateObjectTestHelper<GenericParameterHelper>();
        }

        /// <summary>
        ///A test for RedmineManager Constructor
        ///</summary>
        [TestMethod()]
        public void RedmineManagerConstructorTest()
        {
            string host = string.Empty; // TODO: Initialize to an appropriate value
            string apiKey = string.Empty; // TODO: Initialize to an appropriate value
            RedmineManager target = new RedmineManager(host, apiKey);
            Assert.Inconclusive("TODO: Implement code to verify target");
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
            RedmineManager target = new RedmineManager(host, login, password);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for RedmineManager Constructor
        ///</summary>
        [TestMethod()]
        public void RedmineManagerConstructorTest2()
        {
            string host = string.Empty; // TODO: Initialize to an appropriate value
            RedmineManager target = new RedmineManager(host);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}