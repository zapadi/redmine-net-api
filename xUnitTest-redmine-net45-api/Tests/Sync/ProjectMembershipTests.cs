/*
   Copyright 2011 - 2016 Adrian Popescu.

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
using Redmine.Net.Api;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Types;
using Xunit;

namespace xUnitTestredminenet45api
{
    [Collection("RedmineCollection")]
    public class ProjectMembershipTests
    {
        public ProjectMembershipTests(RedmineFixture fixture)
        {
            this.fixture = fixture;
        }

        private const string PROJECT_IDENTIFIER = "redmine-net-testq";
        private const int NUMBER_OF_PROJECT_MEMBERSHIPS = 3;
        //PM data - used for create
        private const int NEW_PROJECT_MEMBERSHIP_USER_ID = 2;
        private const int NEW_PROJECT_MEMBERSHIP_ROLE_ID = 5;
        private const string PROJECT_MEMBERSHIP_ID = "143";
        //PM data - used for update
        private const string UPDATED_PROJECT_MEMBERSHIP_ID = "143";
        private const int UPDATED_PROJECT_MEMBERSHIP_ROLE_ID = 4;
        private const string DELETED_PROJECT_MEMBERSHIP_ID = "142";

        private readonly RedmineFixture fixture;

        [Fact, Order(1)]
        public void Should_Add_Project_Membership()
        {
            var pm = new ProjectMembership
            {
                User = new IdentifiableName {Id = NEW_PROJECT_MEMBERSHIP_USER_ID},
                Roles = new List<MembershipRole> {new MembershipRole {Id = NEW_PROJECT_MEMBERSHIP_ROLE_ID}}
            };

            var createdPm = fixture.RedmineManager.CreateObject(pm, PROJECT_IDENTIFIER);

            Assert.NotNull(createdPm);
            Assert.True(createdPm.User.Id == NEW_PROJECT_MEMBERSHIP_USER_ID, "User is invalid.");
            Assert.NotNull(createdPm.Roles);
            Assert.True(createdPm.Roles.Exists(r => r.Id == NEW_PROJECT_MEMBERSHIP_ROLE_ID),
                string.Format("Role id {0} does not exist.", NEW_PROJECT_MEMBERSHIP_ROLE_ID));
        }

        [Fact,Order(99)]
        public void Should_Delete_Project_Membership()
        {
            var exception =
                (RedmineException)
                    Record.Exception(
                        () =>
                            fixture.RedmineManager.DeleteObject<ProjectMembership>(DELETED_PROJECT_MEMBERSHIP_ID, null));
            Assert.Null(exception);
            Assert.Throws<NotFoundException>(
                () => fixture.RedmineManager.GetObject<ProjectMembership>(DELETED_PROJECT_MEMBERSHIP_ID, null));
        }

        [Fact, Order(2)]
        public void Should_Get_Memberships_By_Project_Identifier()
        {
            var projectMemberships =
                fixture.RedmineManager.GetObjects<ProjectMembership>(new NameValueCollection
                {
                    {RedmineKeys.PROJECT_ID, PROJECT_IDENTIFIER}
                });

            Assert.NotNull(projectMemberships);
            Assert.True(projectMemberships.Count == NUMBER_OF_PROJECT_MEMBERSHIPS,
                "Project memberships count != " + NUMBER_OF_PROJECT_MEMBERSHIPS);
            Assert.All(projectMemberships, pm => Assert.IsType<ProjectMembership>(pm));
        }

        [Fact, Order(3)]
        public void Should_Get_Project_Membership_By_Id()
        {
            var projectMembership = fixture.RedmineManager.GetObject<ProjectMembership>(PROJECT_MEMBERSHIP_ID, null);

            Assert.NotNull(projectMembership);
            Assert.NotNull(projectMembership.Project);
            Assert.True(projectMembership.User != null || projectMembership.Group != null,
                "User and group are both null.");
            Assert.NotNull(projectMembership.Roles);
            Assert.All(projectMembership.Roles, r => Assert.IsType<MembershipRole>(r));
        }

        [Fact, Order(4)]
        public void Should_Update_Project_Membership()
        {
            var pm = fixture.RedmineManager.GetObject<ProjectMembership>(UPDATED_PROJECT_MEMBERSHIP_ID, null);
            pm.Roles.Add(new MembershipRole {Id = UPDATED_PROJECT_MEMBERSHIP_ROLE_ID});

            fixture.RedmineManager.UpdateObject(UPDATED_PROJECT_MEMBERSHIP_ID, pm);

            var updatedPm = fixture.RedmineManager.GetObject<ProjectMembership>(UPDATED_PROJECT_MEMBERSHIP_ID, null);

            Assert.NotNull(updatedPm);
            Assert.NotNull(updatedPm.Roles);
            Assert.All(updatedPm.Roles, r => Assert.IsType<MembershipRole>(r));
            Assert.True(updatedPm.Roles.Find(r => r.Id == UPDATED_PROJECT_MEMBERSHIP_ROLE_ID) != null,
                string.Format("Role with id {0} was not found in roles list.", UPDATED_PROJECT_MEMBERSHIP_ROLE_ID));
        }
    }
}