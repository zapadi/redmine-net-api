using System;
using Xunit;
using Redmine.Net.Api;
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
		private const int watcherIssueId = 91;
		private const int watcherUserId = 2;

		RedmineFixture fixture;
		public IssueAsyncTests(RedmineFixture fixture)
		{
			this.fixture = fixture;
		}

		[Fact]
		public async Task Should_Add_Watcher_To_Issue()
		{
			await fixture.RedmineManager.AddWatcherAsync(watcherIssueId, watcherUserId);

			Issue issue = await fixture.RedmineManager.GetObjectAsync<Issue>(watcherIssueId.ToString(), new NameValueCollection { { "include", "watchers" } });

			Assert.NotNull(issue);
			Assert.True(issue.Watchers.Count == 1, "Number of watchers != 1");
			Assert.True(((List<Watcher>)issue.Watchers).Find(w => w.Id == watcherUserId) != null, "Watcher not added to issue.");
		}

		[Fact]
		public async Task Should_Remove_Watcher_From_Issue()
		{
			await fixture.RedmineManager.RemoveWatcherAsync(watcherIssueId, watcherUserId);

			Issue issue = await fixture.RedmineManager.GetObjectAsync<Issue>(watcherIssueId.ToString(), new NameValueCollection { { "include", "watchers" } });

			Assert.True(issue.Watchers == null || ((List<Watcher>)issue.Watchers).Find(w => w.Id == watcherUserId) == null);
		}
	}
}

