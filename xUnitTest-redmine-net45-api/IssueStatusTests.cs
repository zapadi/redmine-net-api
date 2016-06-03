using System;
using Xunit;
using Redmine.Net.Api.Types;

namespace xUnitTestredminenet45api
{
	[Collection("RedmineCollection")]
	public class IssueStatusTests
	{
		private const int NUMBER_OF_ISSUE_STATUSES = 7;
		private const bool EXISTS_CLOSED_ISSUE_STATUSES = true;
		private const bool EXISTS_DEFAULT_ISSUE_STATUSES = true;

		RedmineFixture fixture;
		public IssueStatusTests (RedmineFixture fixture)
		{
			this.fixture = fixture;
		}

		[Fact]
		public void Should_Get_All_Issue_Statuses()
		{
			var issueStatuses = fixture.redmineManager.GetObjects<IssueStatus>(null);

			Assert.NotNull(issueStatuses);
			Assert.True(issueStatuses.Count == NUMBER_OF_ISSUE_STATUSES, "Issue statuses count != " + NUMBER_OF_ISSUE_STATUSES);
			Assert.All (issueStatuses, i => Assert.IsType<IssueStatus> (i));

			Assert.True(issueStatuses.Exists(i => i.IsClosed) == EXISTS_CLOSED_ISSUE_STATUSES, EXISTS_CLOSED_ISSUE_STATUSES ? "Closed issue statuses were expected to exist." : "Closed issue statuses were not expected to exist.");
			Assert.True(issueStatuses.Exists(i => i.IsDefault) == EXISTS_DEFAULT_ISSUE_STATUSES, EXISTS_DEFAULT_ISSUE_STATUSES ? "Default issue statuses were expected to exist." : "Default issue statuses were not expected to exist.");
		}
	}
}

