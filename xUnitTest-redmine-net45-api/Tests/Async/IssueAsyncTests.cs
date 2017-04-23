using Xunit;
using Redmine.Net.Api.Async;
using Redmine.Net.Api.Types;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace xUnitTestredminenet45api
{
	[Collection("RedmineCollection")]
	public class IssueAsyncTests
	{
		private const int WATCHER_ISSUE_ID = 91;
		private const int WATCHER_USER_ID = 2;

	    private readonly RedmineFixture fixture;
		public IssueAsyncTests(RedmineFixture fixture)
		{
			this.fixture = fixture;
		}

		[Fact]
		public async Task Should_Add_Watcher_To_Issue()
		{
			await fixture.RedmineManager.AddWatcherAsync(WATCHER_ISSUE_ID, WATCHER_USER_ID);

			Issue issue = await fixture.RedmineManager.GetObjectAsync<Issue>(WATCHER_ISSUE_ID.ToString(), new NameValueCollection { { "include", "watchers" } });

			Assert.NotNull(issue);
			Assert.True(issue.Watchers.Count == 1, "Number of watchers != 1");
			Assert.True(((List<Watcher>)issue.Watchers).Find(w => w.Id == WATCHER_USER_ID) != null, "Watcher not added to issue.");
		}

		[Fact]
		public async Task Should_Remove_Watcher_From_Issue()
		{
			await fixture.RedmineManager.RemoveWatcherAsync(WATCHER_ISSUE_ID, WATCHER_USER_ID);

			Issue issue = await fixture.RedmineManager.GetObjectAsync<Issue>(WATCHER_ISSUE_ID.ToString(), new NameValueCollection { { "include", "watchers" } });

			Assert.True(issue.Watchers == null || ((List<Watcher>)issue.Watchers).Find(w => w.Id == WATCHER_USER_ID) == null);
		}
	}
}