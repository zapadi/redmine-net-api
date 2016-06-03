using Redmine.Net.Api.Types;
using Xunit;

namespace xUnitTestredminenet45api
{
    [Collection("RedmineCollection")]
    public class TrackerTests
    {
        private const int NUMBER_OF_TRACKERS = 2;
        private readonly RedmineFixture fixture;

        public TrackerTests(RedmineFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void RedmineTrackers_ShouldGetAllTrackers()
        {
            var trackers = fixture.Manager.GetObjects<Tracker>(null);

            Assert.NotNull(trackers);
            Assert.NotEmpty(trackers);
            Assert.True(trackers.Count == NUMBER_OF_TRACKERS, "Trackers count != " + NUMBER_OF_TRACKERS);
            Assert.All(trackers, t => Assert.IsType<Tracker>(t));
        }
    }
}