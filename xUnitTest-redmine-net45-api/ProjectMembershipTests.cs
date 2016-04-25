using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using Xunit;
using Redmine.Net.Api.Types;
using Redmine.Net.Api;
using Redmine.Net.Api.Exceptions;

namespace xUnitTestredminenet45api
{
	[Collection("RedmineCollection")]
	public class ProjectMembershipTests
	{
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

		RedmineFixture fixture;
		public ProjectMembershipTests (RedmineFixture fixture)
		{
			this.fixture = fixture;
		}

		[Fact]
		public void Should_Get_Memberships_By_Project_Identifier()
		{
			var projectMemberships = fixture.redmineManager.GetObjects<ProjectMembership>(new NameValueCollection { { RedmineKeys.PROJECT_ID, PROJECT_IDENTIFIER } });

			Assert.NotNull(projectMemberships);
			Assert.True(projectMemberships.Count == NUMBER_OF_PROJECT_MEMBERSHIPS, "Project memberships count != " + NUMBER_OF_PROJECT_MEMBERSHIPS);
			Assert.All (projectMemberships, pm => Assert.IsType<ProjectMembership> (pm));
		}

		[Fact]
		public void Should_Add_Project_Membership()
		{
			ProjectMembership pm = new ProjectMembership();
			pm.User = new IdentifiableName { Id = NEW_PROJECT_MEMBERSHIP_USER_ID };
			pm.Roles = new List<MembershipRole>();
			pm.Roles.Add(new MembershipRole { Id = NEW_PROJECT_MEMBERSHIP_ROLE_ID });

			ProjectMembership createdPM = fixture.redmineManager.CreateObject<ProjectMembership>(pm, PROJECT_IDENTIFIER);

			Assert.NotNull(createdPM);
			Assert.True(createdPM.User.Id == NEW_PROJECT_MEMBERSHIP_USER_ID, "User is invalid.");
			Assert.NotNull(createdPM.Roles);
			Assert.True(createdPM.Roles.Exists(r => r.Id == NEW_PROJECT_MEMBERSHIP_ROLE_ID), string.Format("Role id {0} does not exist.", NEW_PROJECT_MEMBERSHIP_ROLE_ID));
		}

		[Fact]
		public void Should_Get_Project_Membership_By_Id()
		{
			var projectMembership = fixture.redmineManager.GetObject<ProjectMembership>(PROJECT_MEMBERSHIP_ID, null);

			Assert.NotNull(projectMembership);
			Assert.NotNull(projectMembership.Project);
			Assert.True(projectMembership.User != null || projectMembership.Group != null, "User and group are both null.");
			Assert.NotNull(projectMembership.Roles);
			Assert.All (projectMembership.Roles, r => Assert.IsType<MembershipRole> (r));
		}

		[Fact]
		public void Should_Compare_Project_Memberships()
		{
			var projectMembership = fixture.redmineManager.GetObject<ProjectMembership>(PROJECT_MEMBERSHIP_ID, null);
			var projectMembershipToCompare = fixture.redmineManager.GetObject<ProjectMembership>(PROJECT_MEMBERSHIP_ID, null);

			Assert.NotNull(projectMembership);
			Assert.True(projectMembership.Equals(projectMembershipToCompare), "Project memberships are not equal.");
		}

		[Fact]
		public void Should_Update_Project_Membership()
		{
			var pm = fixture.redmineManager.GetObject<ProjectMembership>(UPDATED_PROJECT_MEMBERSHIP_ID, null);
			pm.Roles.Add(new MembershipRole { Id = UPDATED_PROJECT_MEMBERSHIP_ROLE_ID });

			fixture.redmineManager.UpdateObject<ProjectMembership>(UPDATED_PROJECT_MEMBERSHIP_ID, pm);

			var updatedPM = fixture.redmineManager.GetObject<ProjectMembership>(UPDATED_PROJECT_MEMBERSHIP_ID, null);

			Assert.NotNull(updatedPM);
			Assert.NotNull(updatedPM.Roles);
			Assert.All (updatedPM.Roles, r => Assert.IsType<MembershipRole> (r));

			Assert.True(updatedPM.Roles.Find(r => r.Id == UPDATED_PROJECT_MEMBERSHIP_ROLE_ID) != null, string.Format("Role with id {0} was not found in roles list.", UPDATED_PROJECT_MEMBERSHIP_ROLE_ID));
		}

		[Fact]
		public void Should_Delete_Project_Membership()
		{
			RedmineException exception = (RedmineException)Record.Exception(() => fixture.redmineManager.DeleteObject<ProjectMembership>(DELETED_PROJECT_MEMBERSHIP_ID, null));
			Assert.Null (exception);
			Assert.Throws<NotFoundException>(() => fixture.redmineManager.GetObject<ProjectMembership>(DELETED_PROJECT_MEMBERSHIP_ID, null));
		}
	}
}

