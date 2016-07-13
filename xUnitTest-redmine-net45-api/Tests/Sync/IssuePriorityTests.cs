using Xunit;
using Redmine.Net.Api.Types;

namespace xUnitTestredminenet45api
{
	[Collection("RedmineCollection")]
	public class IssuePriorityTests
	{
		private const int NUMBER_OF_ISSUE_PRIORITIES = 4;

	    private readonly RedmineFixture fixture;
		public IssuePriorityTests (RedmineFixture fixture)
		{
			this.fixture = fixture;
		}

		[Fact]
		public void Should_Get_All_Issue_Priority()
		{
			var issuePriorities = fixture.RedmineManager.GetObjects<IssuePriority>(null);

			Assert.NotNull(issuePriorities);
			Assert.True(issuePriorities.Count == NUMBER_OF_ISSUE_PRIORITIES, "Issue priorities count != " + NUMBER_OF_ISSUE_PRIORITIES);
			Assert.All (issuePriorities, ip => Assert.IsType<IssuePriority> (ip));
			Assert.True(issuePriorities.Exists(ip => ip.IsDefault), "List does not contain a default issue priority.");
		}
	}
}