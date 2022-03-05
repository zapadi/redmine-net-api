/*
   Copyright 2011 - 2022 Adrian Popescu

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System.Collections.Generic;
using System.Collections.Specialized;
using Padi.RedmineApi.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineApi.Tests.Tests.Sync
{
    [Trait("Redmine-Net-Api", "Projects")]
#if !(NET20 || NET40)
    [Collection("RedmineCollection")]
#endif
    [Order(1)]
    public class ProjectTests
    {
        public ProjectTests(RedmineFixture fixture)
        {
            this.fixture = fixture;
        }

        private const string PROJECT_IDENTIFIER = "redmine-net-api-project-test";
        private const string PROJECT_NAME = "Redmine Net Api Project Test";

        private readonly RedmineFixture fixture;

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
                EnabledModules = new List<ProjectEnabledModule>
                {
                    new ProjectEnabledModule {Name = "issue_tracking"},
                    new ProjectEnabledModule {Name = "time_tracking"}
                },
                Trackers = new List<ProjectTracker>
                {
                    (ProjectTracker) IdentifiableName.Create<ProjectTracker>( 1),
                    (ProjectTracker) IdentifiableName.Create<ProjectTracker>(2)
                }
            };

            return project;
        }

        private static Project CreateTestProjectWithInvalidTrackersId()
        {
            var project = new Project
            {
                Name = "Redmine Net Api Project Test Invalid Trackers",
                Identifier = "rnaptit",
                Trackers = new List<ProjectTracker>
                {
                    (ProjectTracker) IdentifiableName.Create<ProjectTracker>(999999),
                    (ProjectTracker) IdentifiableName.Create<ProjectTracker>(999998)
                }
            };

            return project;
        }

        private static Project CreateTestProjectWithParentSet(int parentId)
        {
            var project = new Project
            {
                Name = "Redmine Net Api Project With Parent Set",
                Identifier = "rnapwps",
                Parent = IdentifiableName.Create<IdentifiableName>(parentId)
            };

            return project;
        }

        [Fact, Order(0)]
        public void Should_Create_Project_With_Required_Properties()
        {
            var savedProject = fixture.RedmineManager.CreateObject(CreateTestProjectWithRequiredPropertiesSet());

            Assert.NotNull(savedProject);
            Assert.NotEqual(0, savedProject.Id);
            Assert.True(savedProject.Name.Equals(PROJECT_NAME), "Project name is invalid.");
            Assert.True(savedProject.Identifier.Equals(PROJECT_IDENTIFIER), "Project identifier is invalid.");
        }

        [Fact, Order(1)]
        public void Should_Create_Project_With_All_Properties_Set()
        {
            var savedProject = fixture.RedmineManager.CreateObject(CreateTestProjectWithAllPropertiesSet());

            Assert.NotNull(savedProject);
            Assert.NotEqual(0, savedProject.Id);
            Assert.True(savedProject.Identifier.Equals("rnaptap"), "Project identifier is invalid.");
            Assert.True(savedProject.Name.Equals("Redmine Net Api Project Test All Properties"),
                "Project name is invalid.");
        }

        [Fact, Order(2)]
        public void Should_Create_Project_With_Parent()
        {
            var parentProject =
                fixture.RedmineManager.CreateObject(new Project { Identifier = "parent-project", Name = "Parent project" });

            var savedProject = fixture.RedmineManager.CreateObject(CreateTestProjectWithParentSet(parentProject.Id));

            Assert.NotNull(savedProject);
            Assert.True(savedProject.Parent.Id == parentProject.Id, "Parent project is invalid.");
        }

        [Fact, Order(3)]
        public void Should_Get_Redmine_Net_Api_Project_Test_Project()
        {
            var project = fixture.RedmineManager.GetObject<Project>(PROJECT_IDENTIFIER, null);

            Assert.NotNull(project);
            Assert.IsType<Project>(project);
            Assert.Equal(project.Identifier, PROJECT_IDENTIFIER);
            Assert.Equal(project.Name, PROJECT_NAME);
        }

        [Fact, Order(4)]
        public void Should_Get_Test_Project_With_All_Properties_Set()
        {
            var project = fixture.RedmineManager.GetObject<Project>("rnaptap", new NameValueCollection
            {
                {RedmineKeys.INCLUDE, string.Join(",", RedmineKeys.TRACKERS, RedmineKeys.ENABLED_MODULES)}
            });

            Assert.NotNull(project);
            Assert.IsType<Project>(project);
            Assert.True(project.Name.Equals("Redmine Net Api Project Test All Properties"), "Project name not equal.");
            Assert.True(project.Identifier.Equals("rnaptap"), "Project identifier not equal.");
            Assert.True(project.Description.Equals("This is a test project."), "Project description not equal.");
            Assert.True(project.HomePage.Equals("www.redminetest.com"), "Project homepage not equal.");
            Assert.True(project.IsPublic.Equals(true),
                "Project is_public not equal. (This property is available starting with 2.6.0)");

            Assert.NotNull(project.Trackers);
            Assert.True(project.Trackers.Count == 2, "Trackers count != " + 2);

            Assert.NotNull(project.EnabledModules);
            Assert.True(project.EnabledModules.Count == 2,
                "Enabled modules count (" + project.EnabledModules.Count + ") != " + 2);
        }

        [Fact, Order(5)]
        public void Should_Update_Redmine_Net_Api_Project_Test_Project()
        {
            const string UPDATED_PROJECT_NAME = "Project created using API updated";
            const string UPDATED_PROJECT_DESCRIPTION = "Test project description updated";
            const string UPDATED_PROJECT_HOMEPAGE = "http://redmineTestsUpdated.com";
            const bool UPDATED_PROJECT_ISPUBLIC = true;
            const bool UPDATED_PROJECT_INHERIT_MEMBERS = false;

            var project = fixture.RedmineManager.GetObject<Project>(PROJECT_IDENTIFIER, null);
            project.Name = UPDATED_PROJECT_NAME;
            project.Description = UPDATED_PROJECT_DESCRIPTION;
            project.HomePage = UPDATED_PROJECT_HOMEPAGE;
            project.IsPublic = UPDATED_PROJECT_ISPUBLIC;
            project.InheritMembers = UPDATED_PROJECT_INHERIT_MEMBERS;

            var exception =
                (RedmineException)
                Record.Exception(() => fixture.RedmineManager.UpdateObject(PROJECT_IDENTIFIER, project));
            Assert.Null(exception);

            var updatedProject = fixture.RedmineManager.GetObject<Project>(PROJECT_IDENTIFIER, null);

            Assert.True(updatedProject.Name.Equals(UPDATED_PROJECT_NAME), "Project name was not updated.");
            Assert.True(updatedProject.Description.Equals(UPDATED_PROJECT_DESCRIPTION),
                "Project description was not updated.");
            Assert.True(updatedProject.HomePage.Equals(UPDATED_PROJECT_HOMEPAGE), "Project homepage was not updated.");
            Assert.True(updatedProject.IsPublic.Equals(UPDATED_PROJECT_ISPUBLIC),
                "Project is_public was not updated. (This property is available starting with 2.6.0)");
        }

        [Fact, Order(7)]
        public void Should_Throw_Exception_When_Create_Empty_Project()
        {
            Assert.Throws<RedmineException>(() => fixture.RedmineManager.CreateObject(new Project()));
        }

        [Fact, Order(8)]
        public void Should_Throw_Exception_When_Project_Identifier_Is_Invalid()
        {
            Assert.Throws<NotFoundException>(() => fixture.RedmineManager.GetObject<Project>("99999999", null));
        }

        [Fact, Order(9)]
        public void Should_Delete_Project_And_Parent_Project()
        {
            var exception =
                (RedmineException)Record.Exception(() => fixture.RedmineManager.DeleteObject<Project>("rnapwps"));
            Assert.Null(exception);
            Assert.Throws<NotFoundException>(() => fixture.RedmineManager.GetObject<Project>("rnapwps", null));

            exception =
                (RedmineException)
                    Record.Exception(() => fixture.RedmineManager.DeleteObject<Project>("parent-project"));
            Assert.Null(exception);
            Assert.Throws<NotFoundException>(() => fixture.RedmineManager.GetObject<Project>("parent-project", null));
        }

        [Fact, Order(10)]
        public void Should_Delete_Project_With_All_Properties_Set()
        {
            var exception =
                (RedmineException)Record.Exception(() => fixture.RedmineManager.DeleteObject<Project>("rnaptap"));
            Assert.Null(exception);
            Assert.Throws<NotFoundException>(() => fixture.RedmineManager.GetObject<Project>("rnaptap", null));
        }

        [Fact, Order(11)]
        public void Should_Delete_Redmine_Net_Api_Project_Test_Project()
        {
            var exception =
                (RedmineException)
                    Record.Exception(() => fixture.RedmineManager.DeleteObject<Project>(PROJECT_IDENTIFIER));
            Assert.Null(exception);
            Assert.Throws<NotFoundException>(() => fixture.RedmineManager.GetObject<Project>(PROJECT_IDENTIFIER, null));
        }

        [Fact, Order(12)]
        public void Should_Throw_Exception_Create_Project_Invalid_Trackers()
        {
            Assert.Throws<NotFoundException>(
                () => fixture.RedmineManager.CreateObject(CreateTestProjectWithInvalidTrackersId()));
        }
    }
}