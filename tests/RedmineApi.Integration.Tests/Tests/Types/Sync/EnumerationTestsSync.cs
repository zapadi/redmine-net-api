
using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Padi.RedmineApi.Types;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Sync;

[Collection(Constants.RedmineTestContainerCollection)]
public class EnumerationTests(RedmineTestContainerFixture fixture)
{
    [Fact] public void GetDocumentCategories_Should_Succeed()   => Assert.NotNull(fixture.RedmineManager.Get<DocumentCategory>());
    [Fact] public void GetIssuePriorities_Should_Succeed()      => Assert.NotNull(fixture.RedmineManager.Get<IssuePriority>());
    [Fact] public void GetTimeEntryActivities_Should_Succeed()  => Assert.NotNull(fixture.RedmineManager.Get<TimeEntryActivity>());
}