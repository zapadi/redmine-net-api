using System;
using Xunit;
using Redmine.Net.Api.Types;

namespace xUnitTestredminenet45api
{
	[Collection("RedmineCollection")]
	public class TrackerTests
	{
		RedmineFixture fixture;

		private const int NUMBER_OF_TRACKERS = 2;

		public TrackerTests (RedmineFixture fixture)
		{
			this.fixture = fixture;
		}

		[Fact]
		public void RedmineTrackers_ShouldGetAllTrackers()
		{
			var trackers = fixture.RedmineManager.GetObjects<Tracker>(null);

			Assert.NotNull(trackers);
			Assert.NotEmpty (trackers);
			Assert.True(trackers.Count == NUMBER_OF_TRACKERS, "Trackers count != " + NUMBER_OF_TRACKERS);
			Assert.All (trackers, t => Assert.IsType<Tracker> (t));
		}
	}
}

