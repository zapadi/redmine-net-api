#if !(NET20 || NET40)
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Redmine.Net.Api.Async;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineApi.Tests.Tests.Async
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
			await fixture.RedmineManager.AddWatcherToIssueAsync(WATCHER_ISSUE_ID, WATCHER_USER_ID);

			var issue = await fixture.RedmineManager.GetObjectAsync<Issue>(WATCHER_ISSUE_ID.ToString(), new NameValueCollection { { "include", "watchers" } });

			Assert.NotNull(issue);
			Assert.True(issue.Watchers.Count == 1, "Number of watchers != 1");
			Assert.True(((List<Watcher>)issue.Watchers).Find(w => w.Id == WATCHER_USER_ID) != null, "Watcher not added to issue.");
		}

		[Fact]
		public async Task Should_Remove_Watcher_From_Issue()
		{
			await fixture.RedmineManager.RemoveWatcherFromIssueAsync(WATCHER_ISSUE_ID, WATCHER_USER_ID);

			var issue = await fixture.RedmineManager.GetObjectAsync<Issue>(WATCHER_ISSUE_ID.ToString(), new NameValueCollection { { "include", "watchers" } });

			Assert.True(issue.Watchers == null || ((List<Watcher>)issue.Watchers).Find(w => w.Id == WATCHER_USER_ID) == null);
		}
	}
}
#endif