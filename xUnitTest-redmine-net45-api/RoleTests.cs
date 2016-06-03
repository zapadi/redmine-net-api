using Redmine.Net.Api.Types;
using Xunit;

namespace xUnitTestredminenet45api
{
    [Collection("RedmineCollection")]
    public class RoleTests
    {
        private const int NUMBER_OF_ROLES = 3;
        private const string ROLE_ID = "5";
        private const string ROLE_NAME = "CustomRole";
        private const int NUMBER_OF_ROLE_PERMISSIONS = 1;

        private readonly RedmineFixture fixture;

        public RoleTests(RedmineFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void Should_Get_All_Roles()
        {
            var roles = fixture.Manager.GetObjects<Role>(null);

            Assert.NotNull(roles);
            Assert.True(roles.Count == NUMBER_OF_ROLES, "Roles count != " + NUMBER_OF_ROLES);
            Assert.All(roles, r => Assert.IsType<Role>(r));
        }

        [Fact]
        public void Should_Get_Role_By_Id()
        {
            var role = fixture.Manager.GetObject<Role>(ROLE_ID, null);

            Assert.NotNull(role);
            Assert.True(role.Name.Equals(ROLE_NAME), "Role name is invalid.");
            ;
            Assert.NotNull(role.Permissions);
            Assert.True(role.Permissions.Count == NUMBER_OF_ROLE_PERMISSIONS,
                "Permissions count != " + NUMBER_OF_ROLE_PERMISSIONS);
            Assert.All(role.Permissions, p => Assert.IsType<Permission>(p));
        }
    }
}