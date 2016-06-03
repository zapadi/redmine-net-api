using Redmine.Net.Api.Types;
using Xunit;

namespace xUnitTestredminenet45api
{
    [Collection("RedmineCollection")]
    public class TimeEntryActivityTests
    {
        private const int NUMBER_OF_TIME_ENTRY_ACTIVITIES = 3;
        private const bool EXISTS_DEFAULT_TIME_ENTRY_ACTIVITIES = true;

        private readonly RedmineFixture fixture;

        public TimeEntryActivityTests(RedmineFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void Should_Get_All_TimeEntryActivities()
        {
            var timeEntryActivities = fixture.Manager.GetObjects<TimeEntryActivity>(null);

            Assert.NotNull(timeEntryActivities);
            Assert.True(timeEntryActivities.Count == NUMBER_OF_TIME_ENTRY_ACTIVITIES,
                "Time entry activities count != " + NUMBER_OF_TIME_ENTRY_ACTIVITIES);
            Assert.All(timeEntryActivities, t => Assert.IsType<TimeEntryActivity>(t));
            Assert.True(timeEntryActivities.Exists(tea => tea.IsDefault) == EXISTS_DEFAULT_TIME_ENTRY_ACTIVITIES,
                EXISTS_DEFAULT_TIME_ENTRY_ACTIVITIES
                    ? "Default time entry activity was expected to exist."
                    : "Default time entry antivity was not expected to exist.");
        }
    }
}