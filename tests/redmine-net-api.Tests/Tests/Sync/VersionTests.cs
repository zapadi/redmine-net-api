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

using System;
using System.Collections.Specialized;
using Padi.RedmineApi.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Types;
using Xunit;
using Version = Redmine.Net.Api.Types.Version;

namespace Padi.RedmineApi.Tests.Tests.Sync
{
	[Trait("Redmine-Net-Api", "Versions")]
#if !(NET20 || NET40)
    [Collection("RedmineCollection")]
#endif
    public class VersionTests
    {
        public VersionTests(RedmineFixture fixture)
        {
            this.fixture = fixture;
        }

	    private readonly RedmineFixture fixture;

	    private const string PROJECT_ID = "redmine-net-api";

        [Fact]
        [Order(1)]
        public void Should_Create_Version()
        {
	        const string NEW_VERSION_NAME = "VersionTesting";
	        const VersionStatus NEW_VERSION_STATUS = VersionStatus.Locked;
	        const VersionSharing NEW_VERSION_SHARING = VersionSharing.Hierarchy;
	        DateTime newVersionDueDate = DateTime.Now.AddDays(7);
	        const string NEW_VERSION_DESCRIPTION = "Version description";

	        var version = new Version
            {
                Name = NEW_VERSION_NAME,
                Status = NEW_VERSION_STATUS,
                Sharing = NEW_VERSION_SHARING,
                DueDate = newVersionDueDate,
                Description = NEW_VERSION_DESCRIPTION
            };

            var savedVersion = fixture.RedmineManager.CreateObject(version, PROJECT_ID);

            Assert.NotNull(savedVersion);
            Assert.NotNull(savedVersion.Project);
            Assert.True(savedVersion.Name.Equals(NEW_VERSION_NAME), "Version name is invalid.");
            Assert.True(savedVersion.Status.Equals(NEW_VERSION_STATUS), "Version status is invalid.");
            Assert.True(savedVersion.Sharing.Equals(NEW_VERSION_SHARING), "Version sharing is invalid.");
            Assert.NotNull(savedVersion.DueDate);
            Assert.True(savedVersion.DueDate.Value.Date.Equals(newVersionDueDate.Date), "Version due date is invalid.");
            Assert.True(savedVersion.Description.Equals(NEW_VERSION_DESCRIPTION), "Version description is invalid.");
        }

        [Fact]
        [Order(99)]
        public void Should_Delete_Version()
        {
	        const string DELETED_VERSION_ID = "22";
	        var exception =
                (RedmineException)
                    Record.Exception(() => fixture.RedmineManager.DeleteObject<Version>(DELETED_VERSION_ID));
            Assert.Null(exception);
            Assert.Throws<NotFoundException>(() => fixture.RedmineManager.GetObject<Version>(DELETED_VERSION_ID, null));
        }

        [Fact]
        [Order(3)]
        public void Should_Get_Version_By_Id()
        {
	        const string VERSION_ID = "6";

	        var version = fixture.RedmineManager.GetObject<Version>(VERSION_ID, null);

            Assert.NotNull(version);
        }

        [Fact]
        [Order(2)]
        public void Should_Get_Versions_By_Project_Id()
        {
	        const int NUMBER_OF_VERSIONS = 5;
	        var versions =
                fixture.RedmineManager.GetObjects<Version>(new NameValueCollection
                {
                    {RedmineKeys.PROJECT_ID, PROJECT_ID}
                });

            Assert.NotNull(versions);
            Assert.True(versions.Count == NUMBER_OF_VERSIONS, "Versions count ( "+versions.Count+" ) != " + NUMBER_OF_VERSIONS);
        }

        [Fact]
        [Order(4)]
        public void Should_Update_Version()
        {
	        const string UPDATED_VERSION_ID = "15";
	        const string UPDATED_VERSION_NAME = "Updated version";
	        const VersionStatus UPDATED_VERSION_STATUS = VersionStatus.Closed;
	        const VersionSharing UPDATED_VERSION_SHARING = VersionSharing.System;
	        const string UPDATED_VERSION_DESCRIPTION = "Updated description";

            DateTime updatedVersionDueDate = DateTime.Now.AddMonths(1);

	        var version = fixture.RedmineManager.GetObject<Version>(UPDATED_VERSION_ID, null);
            version.Name = UPDATED_VERSION_NAME;
            version.Status = UPDATED_VERSION_STATUS;
            version.Sharing = UPDATED_VERSION_SHARING;
            version.DueDate = updatedVersionDueDate;
            version.Description = UPDATED_VERSION_DESCRIPTION;

            fixture.RedmineManager.UpdateObject(UPDATED_VERSION_ID, version);

            var updatedVersion = fixture.RedmineManager.GetObject<Version>(UPDATED_VERSION_ID, null);

            Assert.NotNull(version);
            Assert.True(updatedVersion.Name.Equals(version.Name), "Version name not updated.");
            Assert.True(updatedVersion.Status.Equals(version.Status), "Status not updated");
            Assert.True(updatedVersion.Sharing.Equals(version.Sharing), "Sharing not updated");
            Assert.True(updatedVersion.DueDate != null && DateTime.Compare(updatedVersion.DueDate.Value.Date, version.DueDate.Value.Date) == 0,
                "DueDate not updated");
            Assert.True(updatedVersion.Description.Equals(version.Description), "Description not updated");
        }
    }
}