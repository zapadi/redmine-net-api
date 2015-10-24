using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System.Diagnostics;

namespace UnitTestRedmineNetApi
{
    [TestClass]
    public class ProjectTests
    {
        private RedmineManager redmineManager;
        private string uri;
        private string apiKey;

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

        [TestMethod]
        public void GetProject_WithAll_AssociatedData()
        {
            var result = redmineManager.GetObject<Project>("9", new NameValueCollection()
            {
                {"include","trackers, issue_categories, enabled_modules" }
            });

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(Project));
            Assert.IsNotNull(result.Trackers, "result.Trackers != null");
            Assert.IsNotNull(result.IssueCategories, "result.IssueCategories != null");
            Assert.IsNotNull(result.EnabledModules,"result.EnabledModules != null");
        }

        [TestMethod]
        public void GetAllProjects_WithAll_AssociatedData()
        {
            IList<Project> result = redmineManager.GetTotalObjectList<Project>(new NameValueCollection()
            {
                {"include", "trackers, issue_categories, enabled_modules"}
            });

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetProject_News()
        {
            var result = redmineManager.GetObjectList<News>(new NameValueCollection()
            {
                {"project_id","9" }
            });

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void RedmineProjects_ShouldCreateProject()
        {
            Project project = new Project();
            project.Name = "Project created using API";
            project.Identifier = "redmine-net-testxyz";
            project.Description = "Test project description";
            project.HomePage = "http://RedmineTests.ro";
            project.IsPublic = true;
            project.Parent = new IdentifiableName { Id = 9 };
            project.InheritMembers = true;
            project.Trackers = new List<ProjectTracker> { new ProjectTracker { Id = 3 } };

            project.EnabledModules = new List<ProjectEnabledModule>();
            project.EnabledModules.Add(new ProjectEnabledModule { Name = "news" });
            project.EnabledModules.Add(new ProjectEnabledModule { Name = "issue_tracking" });

            Project savedProject = redmineManager.CreateObject<Project>(project);

            Assert.AreEqual(project.Name, savedProject.Name);
        }

        [TestMethod]
        public void RedmineProjects_ShouldUpdateProject()
        {
            var projectId = "redmine-net-testxyz";

            var project = redmineManager.GetObject<Project>(projectId, new NameValueCollection { { "include", "trackers,issue_categories,enabled_modules" } });
            project.Name = "Project created using API updated";
            project.Description = "Test project description updated";
            project.HomePage = "http://redmineTestsUpdated.ro";
            project.IsPublic = true;
            project.Parent = null;
            project.InheritMembers = false;
            project.Trackers = null;

            redmineManager.UpdateObject<Project>(projectId, project);

            var updatedProject = redmineManager.GetObject<Project>(projectId,  new NameValueCollection { { "include", "trackers,issue_categories,enabled_modules" } });

            Assert.AreEqual(project.Name, updatedProject.Name);
        }

        [TestMethod]
        public void RedmineProjects_ShouldDeleteProject()
        {
            var projectId = "redmine-net-testxyz";

            try
            {
                redmineManager.DeleteObject<Project>(projectId, null);
            }
            catch (RedmineException)
            {
                Assert.Fail("Project could not be deleted.");
                return;
            }

            try
            {
                Project project = redmineManager.GetObject<Project>(projectId, null);
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
