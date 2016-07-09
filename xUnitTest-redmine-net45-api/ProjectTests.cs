
using System.Collections.Generic;
using System.Collections.Specialized;
using Redmine.Net.Api;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Types;
using Xunit;

namespace xUnitTestredminenet45api
{
    [Trait("Redmine-Net-Api", "Projects")]
    [Collection("RedmineCollection")]
    public class ProjectTests
    {
	    private const string PROJECT_IDENTIFIER = "redmine-net-api-project-test";
      	private const string PROJECT_NAME = "Redmine Net Api Project Test";

	    private readonly RedmineFixture fixture;

        public ProjectTests(RedmineFixture fixture)
        {
            this.fixture = fixture;
        }

	    [Fact]
	    public void Should_Create_Project_With_Required_Properties()
	    {
		    var savedProject = fixture.RedmineManager.CreateObject(CreateTestProjectWithRequiredPropertiesSet());

            Assert.NotNull(savedProject);
		    Assert.NotEqual(savedProject.Id, 0);
		    Assert.True(savedProject.Name.Equals(PROJECT_NAME), "Project name is invalid.");
            Assert.True(savedProject.Identifier.Equals(PROJECT_IDENTIFIER), "Project identifier is invalid.");
	    }

	    [Fact]
        public void Should_Get_Redmine_Net_Api_Project_Test_Project()
        {
            var project = fixture.RedmineManager.GetObject<Project>(PROJECT_IDENTIFIER, null);

            Assert.NotNull(project);
            Assert.IsType<Project>(project);
	        Assert.Equal(project.Identifier,PROJECT_IDENTIFIER);
	        Assert.Equal(project.Name,PROJECT_NAME);
        }

		[Fact]
		public void Should_Update_Redmine_Net_Api_Project_Test_Project()
		{
			const string UPDATED_PROJECT_NAME = "Project created using API updated";
	 		const string UPDATED_PROJECT_DESCRIPTION = "Test project description updated";
	 		const string UPDATED_PROJECT_HOMEPAGE = "http://redmineTestsUpdated.ro";
	 		const bool UPDATED_PROJECT_ISPUBLIC = true;
	 		const bool UPDATED_PROJECT_INHERIT_MEMBERS = false;

			var project = fixture.RedmineManager.GetObject<Project>(PROJECT_IDENTIFIER,null);
			project.Name = UPDATED_PROJECT_NAME;
			project.Description = UPDATED_PROJECT_DESCRIPTION;
			project.HomePage = UPDATED_PROJECT_HOMEPAGE;
			project.IsPublic = UPDATED_PROJECT_ISPUBLIC;
			project.InheritMembers = UPDATED_PROJECT_INHERIT_MEMBERS;

			var exception =(RedmineException)Record.Exception(
						() => fixture.RedmineManager.UpdateObject(PROJECT_IDENTIFIER, project));
			Assert.Null(exception);

			var updatedProject = fixture.RedmineManager.GetObject<Project>(PROJECT_IDENTIFIER,null);

			Assert.True(updatedProject.Name.Equals(UPDATED_PROJECT_NAME), "Project name was not updated.");
			Assert.True(updatedProject.Description.Equals(UPDATED_PROJECT_DESCRIPTION), "Project description was not updated.");
			Assert.True(updatedProject.HomePage.Equals(UPDATED_PROJECT_HOMEPAGE), "Project homepage was not updated.");
			Assert.True(updatedProject.IsPublic.Equals(UPDATED_PROJECT_ISPUBLIC), "Project is_public was not updated.");
			Assert.True(updatedProject.InheritMembers.Equals(UPDATED_PROJECT_INHERIT_MEMBERS), "Project inherit_members was not updated.");
		}

		[Fact]
		public void Should_Delete_Redmine_Net_Api_Project_Test_Project()
		{
			var exception = (RedmineException)Record.Exception(
						() => fixture.RedmineManager.DeleteObject<Project>(PROJECT_IDENTIFIER, null));
			Assert.Null(exception);
			Assert.Throws<NotFoundException>(() => fixture.RedmineManager.GetObject<Project>(PROJECT_IDENTIFIER, null));
		}

	    [Fact]
       	public void Should_Create_Project_With_All_Properties_Set()
       	{
        	var savedProject = fixture.RedmineManager.CreateObject(CreateTestProjectWithAllPropertiesSet());

            Assert.NotNull(savedProject);
		    Assert.NotEqual(savedProject.Id, 0);
            Assert.True(savedProject.Identifier.Equals("rnaptap"), "Project identifier is invalid.");
            Assert.True(savedProject.Name.Equals("Redmine Net Api Project Test All Properties"), "Project name is invalid.");
        }

        [Fact]
        public void Should_Get_test_Project_With_All_Properties_Set()
        {
            var project = fixture.RedmineManager.GetObject<Project>(PROJECT_IDENTIFIER, new NameValueCollection
            {
                {
                    RedmineKeys.INCLUDE,
                    string.Join(",", RedmineKeys.TRACKERS, RedmineKeys.ENABLED_MODULES)
                }
            });

            Assert.NotNull(project);
            Assert.IsType<Project>(project);
	        Assert.True(project.Name.Equals("Redmine Net Api Project Test All Properties"), "Project name not equal.");
           	Assert.True(project.Identifier.Equals("rnaptap"),"Project identifier not equal.");
	        Assert.True(project.Description.Equals("This is a test project."), "Project description not equal.");
            Assert.True(project.HomePage.Equals("www.redminetest.com"), "Project homepage not equal.");
            Assert.True(project.IsPublic.Equals(true), "Project is_public not equal.");
            Assert.True(project.InheritMembers.Equals(true), "Project inherit_members not equal.");

            Assert.NotNull(project.Trackers);
            Assert.True(project.Trackers.Count == 2, "Trackers count != " + 2);
            Assert.All(project.Trackers, t => Assert.IsType<ProjectTracker>(t));

            Assert.NotNull(project.EnabledModules);
            Assert.True(project.EnabledModules.Count == 2, "Enabled modules count != " + 2);
            Assert.All(project.EnabledModules, em => Assert.IsType<ProjectEnabledModule>(em));
        }

	    private static Project CreateTestProjectWithRequiredPropertiesSet()
	    {
		    var project = new Project
		    {
			    Name = PROJECT_NAME,
			    Identifier = PROJECT_IDENTIFIER
		    };

		    return project;
	    }

	    private static Project CreateTestProjectWithAllPropertiesSet()
       	{
			var project = new Project
		    {
				Name = "Redmine Net Api Project Test All Properties",
			    Description = "This is a test project.",
			    Identifier = "rnaptap",
			    HomePage = "www.redminetest.com",
			    IsPublic = true,
			    InheritMembers = true,
			    Status = ProjectStatus.Active,

			    EnabledModules = new List<ProjectEnabledModule>()
			    {
					new ProjectEnabledModule() {Name = "issue_tracking"},
				    new ProjectEnabledModule(){Name = "time_tracking"}
			    },
			    Trackers = new List<ProjectTracker>()
			    {
				    new ProjectTracker {Id = 1},
				    new ProjectTracker {Id = 2}
			    },
		    };

		    return project;
       	}

	    private static Project CreateTestProjectWithParentSet()
       	{
        	var project = new Project
        	{
        		Name = "Redmine Net Api Project With Parent Set",
        		Identifier = "rnapwps",
			    Parent = new IdentifiableName{Id = 1}
        	};

        	return project;
       	}
    }
}