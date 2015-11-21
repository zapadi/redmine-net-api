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
        #region Constants
        private const string projectId = "9";

        //project data - used for create
        private const string newProjectName = "Project created using API";
        private const string newProjectIdentifier = "redmine-net-testxyz";
        private const string newProjectDescription = "Test project description";
        private const string newProjectHomePage = "http://RedmineTests.ro";
        private const bool newProjectIsPublic = true;
        private const int newProjectParentId = 9;
        private const bool newProjectInheritMembers = true;
        private const int newProjectTrackerId = 3;
        private const string newProjectEnableModuleName = "news";

        //project data - used for update
        private const string updatedProjectIdentifier = "redmine-net-testxyz";
        private const string updatedProjectName = "Project created using API updated";
        private const string updatedProjectDescription = "Test project description updated";
        private const string updatedProjectHomePage = "http://redmineTestsUpdated.ro";
        private const bool updatedProjectIsPublic = true;
        private const IdentifiableName updatedProjectParent = null;
        private const bool updatedProjectInheritMembers = false;
        private const List<ProjectTracker> updatedProjectTrackers = null;

        private const string deletedProjectIdentifier = "redmine-net-testxyz";
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
        #endregion Initializes

        #region Tests
        [TestMethod]
        public void GetProject_WithAll_AssociatedData()
        {
            var result = redmineManager.GetObject<Project>(projectId, new NameValueCollection()
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
                {"project_id",projectId }
            });

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void RedmineProjects_ShouldCreateProject()
        {
            Project project = new Project();
            project.Name = newProjectName;
            project.Identifier = newProjectIdentifier;
            project.Description = newProjectDescription;
            project.HomePage = newProjectHomePage;
            project.IsPublic = newProjectIsPublic;
            project.Parent = new IdentifiableName { Id = newProjectParentId };
            project.InheritMembers = newProjectInheritMembers;
            project.Trackers = new List<ProjectTracker> { new ProjectTracker { Id = newProjectTrackerId } };

            project.EnabledModules = new List<ProjectEnabledModule>();
            project.EnabledModules.Add(new ProjectEnabledModule { Name = newProjectEnableModuleName });

            Project savedProject = redmineManager.CreateObject<Project>(project);

            Assert.AreEqual(project.Name, savedProject.Name);
        }

        [TestMethod]
        public void RedmineProjects_ShouldUpdateProject()
        {
            var project = redmineManager.GetObject<Project>(updatedProjectIdentifier, new NameValueCollection { { "include", "trackers,issue_categories,enabled_modules" } });
            project.Name = updatedProjectName;
            project.Description = updatedProjectDescription;
            project.HomePage = updatedProjectHomePage;
            project.IsPublic = updatedProjectIsPublic;
            project.Parent = updatedProjectParent;
            project.InheritMembers = updatedProjectInheritMembers;
            project.Trackers = updatedProjectTrackers;

            redmineManager.UpdateObject<Project>(updatedProjectIdentifier, project);

            var updatedProject = redmineManager.GetObject<Project>(updatedProjectIdentifier, new NameValueCollection { { "include", "trackers,issue_categories,enabled_modules" } });

            Assert.AreEqual(project.Name, updatedProject.Name);
        }

        [TestMethod]
        public void RedmineProjects_ShouldDeleteProject()
        {
            try
            {
                redmineManager.DeleteObject<Project>(deletedProjectIdentifier, null);
            }
            catch (RedmineException)
            {
                Assert.Fail("Project could not be deleted.");
                return;
            }

            try
            {
                Project project = redmineManager.GetObject<Project>(deletedProjectIdentifier, null);
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
