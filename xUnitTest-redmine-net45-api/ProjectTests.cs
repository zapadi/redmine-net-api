using System;
using Xunit;
using Redmine.Net.Api.Types;
using Redmine.Net.Api;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;

namespace xUnitTestredminenet45api
{
	[Collection("RedmineCollection")]
	public class ProjectTests
	{
		private const string PROJECT_IDENTIFIER = "redmine-net-api";
		private const int NUMBER_OF_TRACKERS = 3;
		private const int NUMBER_OF_ISSUE_CATEGORIES = 2;
		private const int NUMBER_OF_ENABLED_MODULES = 9;
		private const int NUMBER_OF_NEWS_BY_PROJECT_ID =1;
		private const int NUMBER_OF_PROJECTS = 6;

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

		RedmineFixture fixture;
		public ProjectTests (RedmineFixture fixture)
		{
			this.fixture = fixture;
		}

		[Fact]
		public void Should_Get_Project()
		{
			var project = fixture.redmineManager.GetObject<Project>(PROJECT_IDENTIFIER, null);

			Assert.NotNull(project);
			Assert.IsType<Project>(project);
		}

		[Fact]
		public void Should_Get_Project_With_Trackers()
		{
			var project = fixture.redmineManager.GetObject<Project>(PROJECT_IDENTIFIER, new NameValueCollection()
				{
					{RedmineKeys.INCLUDE, RedmineKeys.TRACKERS}
				});

			Assert.NotNull(project);
			Assert.IsType<Project>(project);

			Assert.NotNull(project.Trackers);
			Assert.True(project.Trackers.Count == NUMBER_OF_TRACKERS, "Trackers count != " + NUMBER_OF_TRACKERS);
			Assert.All (project.Trackers, t => Assert.IsType<ProjectTracker> (t));
		}

		[Fact]
		public void Should_Get_Project_With_IssueCategories()
		{
			var project = fixture.redmineManager.GetObject<Project>(PROJECT_IDENTIFIER, new NameValueCollection()
				{
					{RedmineKeys.INCLUDE, RedmineKeys.ISSUE_CATEGORIES}
				});

			Assert.NotNull(project);
			Assert.IsType<Project>(project);

			Assert.NotNull(project.IssueCategories);
			Assert.True(project.IssueCategories.Count == NUMBER_OF_ISSUE_CATEGORIES, "Issue categories count != " + NUMBER_OF_ISSUE_CATEGORIES);
			Assert.All (project.IssueCategories, ic => Assert.IsType<ProjectIssueCategory> (ic));
		}

		[Fact]
		public void Should_Get_Project_With_EnabledModules()
		{
			var project = fixture.redmineManager.GetObject<Project>(PROJECT_IDENTIFIER, new NameValueCollection()
				{
					{RedmineKeys.INCLUDE, RedmineKeys.ENABLED_MODULES}
				});

			Assert.NotNull(project);
			Assert.IsType<Project>(project);

			Assert.NotNull(project.EnabledModules);
			Assert.True(project.EnabledModules.Count == NUMBER_OF_ENABLED_MODULES, "Enabled modules count != " + NUMBER_OF_ENABLED_MODULES);
			Assert.All (project.EnabledModules, em => Assert.IsType<ProjectEnabledModule> (em));
		}

		[Fact]
		public void Should_Get_Project_With_All_Data()
		{
			var project = fixture.redmineManager.GetObject<Project>(PROJECT_IDENTIFIER, new NameValueCollection()
				{
					{RedmineKeys.INCLUDE, RedmineKeys.TRACKERS+","+RedmineKeys.ISSUE_CATEGORIES+","+RedmineKeys.ENABLED_MODULES}
				});

			Assert.NotNull(project);
			Assert.IsType<Project>(project);

			Assert.NotNull(project.Trackers);
			Assert.True(project.Trackers.Count == NUMBER_OF_TRACKERS, "Trackers count != " + NUMBER_OF_TRACKERS);
			Assert.All (project.Trackers, t => Assert.IsType<ProjectTracker> (t));

			Assert.NotNull(project.IssueCategories);
			Assert.True(project.IssueCategories.Count == NUMBER_OF_ISSUE_CATEGORIES, "Issue categories count != " + NUMBER_OF_ISSUE_CATEGORIES);
			Assert.All (project.IssueCategories, ic => Assert.IsType<ProjectIssueCategory> (ic));

			Assert.NotNull(project.EnabledModules);
			Assert.True(project.EnabledModules.Count == NUMBER_OF_ENABLED_MODULES, "Enabled modules count != " + NUMBER_OF_ENABLED_MODULES);
			Assert.All (project.EnabledModules, em => Assert.IsType<ProjectEnabledModule> (em));
		}

		[Fact]
		public void Should_Get_All_Projects_With_All_Data()
		{
			IList<Project> projects = fixture.redmineManager.GetObjects<Project>(new NameValueCollection()
				{
					{RedmineKeys.INCLUDE, RedmineKeys.TRACKERS+","+RedmineKeys.ISSUE_CATEGORIES+","+RedmineKeys.ENABLED_MODULES}
				});

			Assert.NotNull(projects);
			Assert.True(projects.Count == NUMBER_OF_PROJECTS, "Projects count != " + NUMBER_OF_PROJECTS);
			Assert.All (projects, p => Assert.IsType<Project> (p));
				
			var project = projects.ToList().Find(p => p.Identifier == PROJECT_IDENTIFIER);
			Assert.NotNull(project);
			Assert.IsType<Project>(project);

			Assert.NotNull(project.Trackers);
			Assert.True(project.Trackers.Count == NUMBER_OF_TRACKERS, "Trackers count != " + NUMBER_OF_TRACKERS);
			Assert.All (project.Trackers, t => Assert.IsType<ProjectTracker> (t));

			Assert.NotNull(project.IssueCategories);
			Assert.True(project.IssueCategories.Count == NUMBER_OF_ISSUE_CATEGORIES, "Issue categories count != " + NUMBER_OF_ISSUE_CATEGORIES);
			Assert.All (project.IssueCategories, ic => Assert.IsType<ProjectIssueCategory> (ic));

			Assert.NotNull(project.EnabledModules);
			Assert.True(project.EnabledModules.Count == NUMBER_OF_ENABLED_MODULES, "Enabled modules count != " + NUMBER_OF_ENABLED_MODULES);
			Assert.All (project.EnabledModules, em => Assert.IsType<ProjectEnabledModule> (em));
		}

		[Fact]
		public void Should_Get_Project_News()
		{
			var news = fixture.redmineManager.GetObjects<News>(new NameValueCollection { { RedmineKeys.PROJECT_ID, PROJECT_IDENTIFIER } });

			Assert.NotNull(news);
			Assert.True(news.Count == NUMBER_OF_NEWS_BY_PROJECT_ID, "News count != " + NUMBER_OF_NEWS_BY_PROJECT_ID);
			Assert.All (news, n => Assert.IsType<News> (n));
		}

		[Fact]
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

			Project savedProject = fixture.redmineManager.CreateObject<Project>(project);

			Assert.NotNull(savedProject);
			Assert.True(savedProject.Identifier.Equals(NEW_PROJECT_IDENTIFIER), "Project identifier is invalid.");
		}

		[Fact]
		public void Should_Update_Project()
		{
			var project = fixture.redmineManager.GetObject<Project>(UPDATED_PROJECT_IDENTIFIER, new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.TRACKERS + "," + RedmineKeys.ISSUE_CATEGORIES + "," + RedmineKeys.ENABLED_MODULES } });
			project.Name = UPDATED_PROJECT_NAME;
			project.Description = UPDATED_PROJECT_DESCRIPTION;
			project.HomePage = UPDATED_PROJECT_HOMEPAGE;
			project.IsPublic = UPDATED_PROJECT_ISPUBLIC;
			project.Parent = UPDATED_PROJECT_PARENT;
			project.InheritMembers = UPDATED_PROJECT_INHERIT_MEMBERS;
			project.Trackers = UPDATED_PROJECT_TRACKERS;

			fixture.redmineManager.UpdateObject<Project>(UPDATED_PROJECT_IDENTIFIER, project);

			var updatedProject = fixture.redmineManager.GetObject<Project>(UPDATED_PROJECT_IDENTIFIER, new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.TRACKERS + "," + RedmineKeys.ISSUE_CATEGORIES + "," + RedmineKeys.ENABLED_MODULES } });

			Assert.NotNull(updatedProject);
			Assert.True(updatedProject.Name.Equals(UPDATED_PROJECT_NAME), "Project name was not updated.");
		}

		[Fact]
		public void Should_Delete_Project()
		{
			try
			{
				fixture.redmineManager.DeleteObject<Project>(DELETED_PROJECT_IDENTIFIER, null);
			}
			catch (RedmineException)
			{
				Assert.True(false, "Project could not be deleted.");
				return;
			}

			try
			{
				fixture.redmineManager.GetObject<Project>(DELETED_PROJECT_IDENTIFIER, null);
			}
			catch (RedmineException exc)
			{
				Assert.Contains(exc.Message, "Not Found");
				return;
			}
			Assert.True(false, "Test failed");
		}

		[Fact]
		public void Should_Compare_Projects()
		{
			var project = fixture.redmineManager.GetObject<Project>(PROJECT_IDENTIFIER, new NameValueCollection() { { RedmineKeys.INCLUDE, RedmineKeys.TRACKERS + "," + RedmineKeys.ISSUE_CATEGORIES + "," + RedmineKeys.ENABLED_MODULES } });
			var projectToCompare = fixture.redmineManager.GetObject<Project>(PROJECT_IDENTIFIER, new NameValueCollection() { { RedmineKeys.INCLUDE, RedmineKeys.TRACKERS + "," + RedmineKeys.ISSUE_CATEGORIES + "," + RedmineKeys.ENABLED_MODULES } });

			Assert.NotNull(project);
			Assert.True(project.Equals(projectToCompare), "Compared projects are not equal.");
		}
	}
}

