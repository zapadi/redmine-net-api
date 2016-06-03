using System;
using System.Collections.Specialized;
using Redmine.Net.Api;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Types;
using Xunit;
using Version = Redmine.Net.Api.Types.Version;

namespace xUnitTestredminenet45api
{
    [Collection("RedmineCollection")]
    public class VersionTests
    {
        private const string PROJECT_ID = "redmine-net-api";
        private const int NUMBER_OF_VERSIONS = 5;

        //version data - used for create
        private const string NEW_VERSION_NAME = "VersionTesting";
        private const VersionStatus NEW_VERSION_STATUS = VersionStatus.locked;
        private const VersionSharing NEW_VERSION_SHARING = VersionSharing.hierarchy;
        private const string NEW_VERSION_DESCRIPTION = "Version description";

        private const string VERSION_ID = "6";

        //version data - used for update 
        private const string UPDATED_VERSION_ID = "15";
        private const string UPDATED_VERSION_NAME = "Updated version";
        private const VersionStatus UPDATED_VERSION_STATUS = VersionStatus.closed;
        private const VersionSharing UPDATED_VERSION_SHARING = VersionSharing.system;
        private const string UPDATED_VERSION_DESCRIPTION = "Updated description";

        private const string DELETED_VERSION_ID = "22";

        private readonly RedmineFixture fixture;
        private DateTime NEW_VERSION_DUE_DATE = DateTime.Now.AddDays(7);
        private readonly DateTime UPDATED_VERSION_DUE_DATE = DateTime.Now.AddMonths(1);

        public VersionTests(RedmineFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void Should_Get_Versions_By_Project_Id()
        {
            var versions =
                fixture.Manager.GetObjects<Version>(new NameValueCollection {{RedmineKeys.PROJECT_ID, PROJECT_ID}});

            Assert.NotNull(versions);
            Assert.NotEmpty(versions);
            Assert.True(versions.Count == NUMBER_OF_VERSIONS, "Versions count != " + NUMBER_OF_VERSIONS);
            Assert.All(versions, v => Assert.IsType<Version>(v));
        }

        [Fact]
        public void Should_Create_Version()
        {
            var version = new Version();
            version.Name = NEW_VERSION_NAME;
            version.Status = NEW_VERSION_STATUS;
            version.Sharing = NEW_VERSION_SHARING;
            version.DueDate = NEW_VERSION_DUE_DATE;
            version.Description = NEW_VERSION_DESCRIPTION;

            var savedVersion = fixture.Manager.CreateObject(version, PROJECT_ID);

            Assert.NotNull(savedVersion);
            Assert.NotNull(savedVersion.Project);
            Assert.True(savedVersion.Name.Equals(NEW_VERSION_NAME), "Version name is invalid.");
            Assert.True(savedVersion.Status.Equals(NEW_VERSION_STATUS), "Version status is invalid.");
            Assert.True(savedVersion.Sharing.Equals(NEW_VERSION_SHARING), "Version sharing is invalid.");
            Assert.NotNull(savedVersion.DueDate);
            Assert.True(savedVersion.DueDate.Value.Date.Equals(NEW_VERSION_DUE_DATE.Date),
                "Version due date is invalid.");
            Assert.True(savedVersion.Description.Equals(NEW_VERSION_DESCRIPTION), "Version description is invalid.");
        }

        [Fact]
        public void Should_Get_Version_By_Id()
        {
            var version = fixture.Manager.GetObject<Version>(VERSION_ID, null);

            Assert.NotNull(version);
        }

        [Fact]
        public void Should_Compare_Versions()
        {
            var version = fixture.Manager.GetObject<Version>(VERSION_ID, null);
            var versionToCompare = fixture.Manager.GetObject<Version>(VERSION_ID, null);

            Assert.NotNull(version);
            Assert.True(version.Equals(versionToCompare), "Versions are not equal.");
        }

        [Fact]
        public void Should_Update_Version()
        {
            var version = fixture.Manager.GetObject<Version>(UPDATED_VERSION_ID, null);
            version.Name = UPDATED_VERSION_NAME;
            version.Status = UPDATED_VERSION_STATUS;
            version.Sharing = UPDATED_VERSION_SHARING;
            version.DueDate = UPDATED_VERSION_DUE_DATE;
            version.Description = UPDATED_VERSION_DESCRIPTION;

            fixture.Manager.UpdateObject(UPDATED_VERSION_ID, version);

            var updatedVersion = fixture.Manager.GetObject<Version>(UPDATED_VERSION_ID, null);

            Assert.NotNull(version);
            Assert.True(updatedVersion.Name.Equals(version.Name), "Version name not updated.");
            Assert.True(updatedVersion.Status.Equals(version.Status), "Status not updated");
            Assert.True(updatedVersion.Sharing.Equals(version.Sharing), "Sharing not updated");
            Assert.True(DateTime.Compare(updatedVersion.DueDate.Value.Date, version.DueDate.Value.Date) == 0,
                "DueDate not updated");
            Assert.True(updatedVersion.Description.Equals(version.Description), "Description not updated");
        }

        [Fact]
        public void Should_Delete_Version()
        {
            var exception =
                (RedmineException)
                    Record.Exception(() => fixture.Manager.DeleteObject<Version>(DELETED_VERSION_ID, null));
            Assert.Null(exception);
            Assert.Throws<NotFoundException>(() => fixture.Manager.GetObject<Version>(DELETED_VERSION_ID, null));
        }
    }
}