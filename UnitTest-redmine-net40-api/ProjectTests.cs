using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System.Diagnostics;
using System.Linq;

namespace UnitTest_redmine_net40_api
{
    [TestClass]
    public class ProjectTests
    {
        private RedmineManager redmineManager;

        private const string PROJECT_IDENTIFIER = "redmine-test";
        private const int NUMBER_OF_TRACKERS = 3;
        private const int NUMBER_OF_ISSUE_CATEGORIES = 2;
        private const int NUMBER_OF_ENABLED_MODULES = 9;
        private const int NUMBER_OF_NEWS_BY_PROJECT_ID = 3;
        private const int NUMBER_OF_PROJECTS = 2;

        //project data - used for create
        private const string NEW_PROJECT_NAME = "Project created using API";
        private const string NEW_PROJECT_IDENTIFIER = "redmine-net-testxyz";
        private const string NEW_PROJECT_DESCRIPTION = "Test project description";
        private const string NEW_PROJECT_HOMEPAGE = "http://RedmineTests.ro";
        private const bool NEW_PROJECT_ISPUBLIC = true;
        private const int NEW_PROJECT_PARENT_ID = 1;
        private const bool NEW_PROJECT_INHERIT_MEMBERS = true;
        private const int NEW_PROJECT_TRACKER_ID = 3;
        private const string NEW_PROJECT_ENABLED_MODULE_NAME = "news";

        //project data - used for update
        private const string UPDATED_PROJECT_IDENTIFIER = "redmine-net-testxyz";
        private const string UPDATED_PROJECT_NAME = "Project created using API updated";
        private const string UPDATED_PROJECT_DESCRIPTION = "Test project description updated";
        private const string UPDATED_PROJECT_HOMEPAGE = "http://redmineTestsUpdated.ro";
        private const bool UPDATED_PROJECT_ISPUBLIC = true;
        private const IdentifiableName UPDATED_PROJECT_PARENT = null;
        private const bool UPDATED_PROJECT_INHERIT_MEMBERS = false;
        private const List<ProjectTracker> UPDATED_PROJECT_TRACKERS = null;

        private const string DELETED_PROJECT_IDENTIFIER = "redmine-net-testxyz";

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
        public void Should_Get_Project()
        {
            var project = redmineManager.GetObject<Project>(PROJECT_IDENTIFIER, null);

            Assert.IsNotNull(project, "Get project returned null.");
            Assert.IsInstanceOfType(project, typeof(Project), "Entity is not of type project.");
        }

        [TestMethod]
        public void Should_Get_Project_With_Trackers()
        {
            var project = redmineManager.GetObject<Project>(PROJECT_IDENTIFIER, new NameValueCollection()
            {
                {RedmineKeys.INCLUDE, RedmineKeys.TRACKERS}
            });

            Assert.IsNotNull(project, "Get project returned null.");
            Assert.IsInstanceOfType(project, typeof(Project), "Entity is not of type project.");

            Assert.IsNotNull(project.Trackers, "Trackers list is null.");
            Assert.IsTrue(project.Trackers.Count == NUMBER_OF_TRACKERS, "Trackers count != " + NUMBER_OF_TRACKERS);
            CollectionAssert.AllItemsAreNotNull(project.Trackers.ToList(), "Trackers list contains null items.");
            CollectionAssert.AllItemsAreUnique(project.Trackers.ToList(), "Trackers items are not unique.");
            CollectionAssert.AllItemsAreInstancesOfType(project.Trackers.ToList(), typeof(ProjectTracker), "Not all items are of type ProjectTracker.");
         }

        [TestMethod]
        public void Should_Get_Project_With_IssueCategories()
        {
            var project = redmineManager.GetObject<Project>(PROJECT_IDENTIFIER, new NameValueCollection()
            {
                {RedmineKeys.INCLUDE, RedmineKeys.ISSUE_CATEGORIES}
            });

            Assert.IsNotNull(project, "Get project returned null.");
            Assert.IsInstanceOfType(project, typeof(Project), "Entity is not of type project.");

            Assert.IsNotNull(project.IssueCategories, "Issue categories list is null.");
            Assert.IsTrue(project.IssueCategories.Count == NUMBER_OF_ISSUE_CATEGORIES, "Issue categories count != " + NUMBER_OF_ISSUE_CATEGORIES);
            CollectionAssert.AllItemsAreNotNull(project.IssueCategories.ToList(), "Issue categories list contains null items.");
            CollectionAssert.AllItemsAreUnique(project.IssueCategories.ToList(), "Issue categories items are not unique.");
            CollectionAssert.AllItemsAreInstancesOfType(project.IssueCategories.ToList(), typeof(ProjectIssueCategory), "Not all items are of type ProjectIssueCategory.");
        }

        [TestMethod]
        public void Should_Get_Project_With_EnabledModules()
        {
            var project = redmineManager.GetObject<Project>(PROJECT_IDENTIFIER, new NameValueCollection()
            {
                {RedmineKeys.INCLUDE, RedmineKeys.ENABLED_MODULES}
            });

            Assert.IsNotNull(project, "Get project returned null.");
            Assert.IsInstanceOfType(project, typeof(Project), "Entity is not of type project.");

            Assert.IsNotNull(project.EnabledModules, "Enabled modules list is null.");
            Assert.IsTrue(project.EnabledModules.Count == NUMBER_OF_ENABLED_MODULES, "Enabled modules count != " + NUMBER_OF_ENABLED_MODULES);
            CollectionAssert.AllItemsAreNotNull(project.EnabledModules.ToList(), "Enabled modules list contains null items.");
            CollectionAssert.AllItemsAreUnique(project.EnabledModules.ToList(), "Enabled modules items are not unique.");
            CollectionAssert.AllItemsAreInstancesOfType(project.EnabledModules.ToList(), typeof(ProjectEnabledModule), "Not all items are of type ProjectEnabledModule.");
        }

        [TestMethod]
        public void Should_Get_Project_With_All_Data()
        {
            var project = redmineManager.GetObject<Project>(PROJECT_IDENTIFIER, new NameValueCollection()
            {
                {RedmineKeys.INCLUDE, RedmineKeys.TRACKERS+","+RedmineKeys.ISSUE_CATEGORIES+","+RedmineKeys.ENABLED_MODULES}
            });

            Assert.IsNotNull(project, "Get project returned null.");
            Assert.IsInstanceOfType(project, typeof(Project), "Entity is not of type project.");

            Assert.IsNotNull(project.Trackers, "Trackers list is null.");
            Assert.IsTrue(project.Trackers.Count == NUMBER_OF_TRACKERS, "Trackers count != " + NUMBER_OF_TRACKERS);
            CollectionAssert.AllItemsAreNotNull(project.Trackers.ToList(), "Trackers list contains null items.");
            CollectionAssert.AllItemsAreUnique(project.Trackers.ToList(), "Trackers items are not unique.");
            CollectionAssert.AllItemsAreInstancesOfType(project.Trackers.ToList(), typeof(ProjectTracker), "Not all items are of type ProjectTracker.");

            Assert.IsNotNull(project.IssueCategories, "Issue categories list is null.");
            Assert.IsTrue(project.IssueCategories.Count == NUMBER_OF_ISSUE_CATEGORIES, "Issue categories count != " + NUMBER_OF_ISSUE_CATEGORIES);
            CollectionAssert.AllItemsAreNotNull(project.IssueCategories.ToList(), "Issue categories list contains null items.");
            CollectionAssert.AllItemsAreUnique(project.IssueCategories.ToList(), "Issue categories items are not unique.");
            CollectionAssert.AllItemsAreInstancesOfType(project.IssueCategories.ToList(), typeof(ProjectIssueCategory), "Not all items are of type ProjectIssueCategory.");

            Assert.IsNotNull(project.EnabledModules, "Enabled modules list is null.");
            Assert.IsTrue(project.EnabledModules.Count == NUMBER_OF_ENABLED_MODULES, "Enabled modules count != " + NUMBER_OF_ENABLED_MODULES);
            CollectionAssert.AllItemsAreNotNull(project.EnabledModules.ToList(), "Enabled modules list contains null items.");
            CollectionAssert.AllItemsAreUnique(project.EnabledModules.ToList(), "Enabled modules items are not unique.");
            CollectionAssert.AllItemsAreInstancesOfType(project.EnabledModules.ToList(), typeof(ProjectEnabledModule), "Not all items are of type ProjectEnabledModule.");
        }

        [TestMethod]
        public void Should_Get_All_Projects_With_All_Data()
        {
            IList<Project> projects = redmineManager.GetObjects<Project>(new NameValueCollection()
            {
                {RedmineKeys.INCLUDE, RedmineKeys.TRACKERS+","+RedmineKeys.ISSUE_CATEGORIES+","+RedmineKeys.ENABLED_MODULES}
            });

            Assert.IsNotNull(projects, "Get projects returned null.");
            Assert.IsTrue(projects.Count == NUMBER_OF_PROJECTS, "Projects count != " + NUMBER_OF_PROJECTS);
            CollectionAssert.AllItemsAreNotNull(projects.ToList(), "Projects list contains null items.");
            CollectionAssert.AllItemsAreUnique(projects.ToList(), "Projects items are not unique.");
            CollectionAssert.AllItemsAreInstancesOfType(projects.ToList(), typeof(Project), "Not all items are of type Project.");

            var project = projects.ToList().Find(p => p.Identifier == PROJECT_IDENTIFIER);
            Assert.IsNotNull(project, "Get project returned null.");
            Assert.IsInstanceOfType(project, typeof(Project), "Entity is not of type project.");

            Assert.IsNotNull(project.Trackers, "Trackers list is null.");
            Assert.IsTrue(project.Trackers.Count == NUMBER_OF_TRACKERS, "Trackers count != " + NUMBER_OF_TRACKERS);
            CollectionAssert.AllItemsAreNotNull(project.Trackers.ToList(), "Trackers list contains null items.");
            CollectionAssert.AllItemsAreUnique(project.Trackers.ToList(), "Trackers items are not unique.");
            CollectionAssert.AllItemsAreInstancesOfType(project.Trackers.ToList(), typeof(ProjectTracker), "Not all items are of type ProjectTracker.");

            Assert.IsNotNull(project.IssueCategories, "Issue categories list is null.");
            Assert.IsTrue(project.IssueCategories.Count == NUMBER_OF_ISSUE_CATEGORIES, "Issue categories count != " + NUMBER_OF_ISSUE_CATEGORIES);
            CollectionAssert.AllItemsAreNotNull(project.IssueCategories.ToList(), "Issue categories list contains null items.");
            CollectionAssert.AllItemsAreUnique(project.IssueCategories.ToList(), "Issue categories items are not unique.");
            CollectionAssert.AllItemsAreInstancesOfType(project.IssueCategories.ToList(), typeof(ProjectIssueCategory), "Not all items are of type ProjectIssueCategory.");

            Assert.IsNotNull(project.EnabledModules, "Enabled modules list is null.");
            Assert.IsTrue(project.EnabledModules.Count == NUMBER_OF_ENABLED_MODULES, "Enabled modules count != " + NUMBER_OF_ENABLED_MODULES);
            CollectionAssert.AllItemsAreNotNull(project.EnabledModules.ToList(), "Enabled modules list contains null items.");
            CollectionAssert.AllItemsAreUnique(project.EnabledModules.ToList(), "Enabled modules items are not unique.");
            CollectionAssert.AllItemsAreInstancesOfType(project.EnabledModules.ToList(), typeof(ProjectEnabledModule), "Not all items are of type ProjectEnabledModule.");
        }

        [TestMethod]
        public void Should_Get_Project_News()
        {
            var news = redmineManager.GetObjects<News>(new NameValueCollection { { RedmineKeys.PROJECT_ID, PROJECT_IDENTIFIER } });

            Assert.IsNotNull(news, "Get all news returned null");
            Assert.IsTrue(news.Count == NUMBER_OF_NEWS_BY_PROJECT_ID, "News count != " + NUMBER_OF_NEWS_BY_PROJECT_ID);
            CollectionAssert.AllItemsAreNotNull(news, "News list contains null items.");
            CollectionAssert.AllItemsAreUnique(news, "News items are not unique.");
            CollectionAssert.AllItemsAreInstancesOfType(news, typeof(News), "Not all items are of type news.");
        }

        [TestMethod]
        public void Should_Create_Projects()
        {
            Project project = new Project();
            project.Name = NEW_PROJECT_NAME;
            project.Identifier = NEW_PROJECT_IDENTIFIER;
            project.Description = NEW_PROJECT_DESCRIPTION;
            project.HomePage = NEW_PROJECT_HOMEPAGE;
            project.IsPublic = NEW_PROJECT_ISPUBLIC;
            project.Parent = new IdentifiableName { Id = NEW_PROJECT_PARENT_ID };
            project.InheritMembers = NEW_PROJECT_INHERIT_MEMBERS;
            project.Trackers = new List<ProjectTracker> { new ProjectTracker { Id = NEW_PROJECT_TRACKER_ID } };
            project.EnabledModules = new List<ProjectEnabledModule>();
            project.EnabledModules.Add(new ProjectEnabledModule { Name = NEW_PROJECT_ENABLED_MODULE_NAME });

            Project savedProject = redmineManager.CreateObject<Project>(project);

            Assert.IsNotNull(savedProject, "Create project returned null.");
            Assert.AreEqual(savedProject.Identifier, NEW_PROJECT_IDENTIFIER, "Project identifier is invalid.");
        }

        [TestMethod]
        public void Should_Update_Project()
        {
            var project = redmineManager.GetObject<Project>(UPDATED_PROJECT_IDENTIFIER, new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.TRACKERS + "," + RedmineKeys.ISSUE_CATEGORIES + "," + RedmineKeys.ENABLED_MODULES } });
            project.Name = UPDATED_PROJECT_NAME;
            project.Description = UPDATED_PROJECT_DESCRIPTION;
            project.HomePage = UPDATED_PROJECT_HOMEPAGE;
            project.IsPublic = UPDATED_PROJECT_ISPUBLIC;
            project.Parent = UPDATED_PROJECT_PARENT;
            project.InheritMembers = UPDATED_PROJECT_INHERIT_MEMBERS;
            project.Trackers = UPDATED_PROJECT_TRACKERS;

            redmineManager.UpdateObject<Project>(UPDATED_PROJECT_IDENTIFIER, project);

            var updatedProject = redmineManager.GetObject<Project>(UPDATED_PROJECT_IDENTIFIER, new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.TRACKERS + "," + RedmineKeys.ISSUE_CATEGORIES + "," + RedmineKeys.ENABLED_MODULES } });

            Assert.IsNotNull(updatedProject, "Updated project is null.");
            Assert.AreEqual(updatedProject.Name, UPDATED_PROJECT_NAME, "Project name was not updated.");
        }

        [TestMethod]
        public void Should_Delete_Project()
        {
            try
            {
                redmineManager.DeleteObject<Project>(DELETED_PROJECT_IDENTIFIER, null);
            }
            catch (RedmineException)
            {
                Assert.Fail("Project could not be deleted.");
                return;
            }

            try
            {
                Project project = redmineManager.GetObject<Project>(DELETED_PROJECT_IDENTIFIER, null);
            }
            catch (RedmineException exc)
            {
                StringAssert.Contains(exc.Message, "Not Found");
                return;
            }
            Assert.Fail("Test failed");
        }

        [TestMethod]
        public void Should_Compare_Projects()
        {
            var project = redmineManager.GetObject<Project>(PROJECT_IDENTIFIER, new NameValueCollection() { { RedmineKeys.INCLUDE, RedmineKeys.TRACKERS + "," + RedmineKeys.ISSUE_CATEGORIES + "," + RedmineKeys.ENABLED_MODULES } });
            var projectToCompare = redmineManager.GetObject<Project>(PROJECT_IDENTIFIER, new NameValueCollection() { { RedmineKeys.INCLUDE, RedmineKeys.TRACKERS + "," + RedmineKeys.ISSUE_CATEGORIES + "," + RedmineKeys.ENABLED_MODULES } });

            Assert.IsNotNull(project, "Project is null.");
            Assert.IsTrue(project.Equals(projectToCompare), "Compared projects are not equal.");
        }
    }
}
