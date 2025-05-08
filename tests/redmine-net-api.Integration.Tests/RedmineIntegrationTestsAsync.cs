using System.Collections.Specialized;
using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Redmine.Net.Api;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Types;
using Version = Redmine.Net.Api.Types.Version;

namespace Padi.DotNet.RedmineAPI.Integration.Tests;

[Collection(Constants.RedmineTestContainerCollection)]
public class RedmineIntegrationTestsAsync(RedmineTestContainerFixture fixture)
{
    private readonly RedmineManager _redmineManager = fixture.RedmineManager;

    [Fact]
    public async Task Should_ReturnProjectsAsync()
    {
        var list = await _redmineManager.GetAsync<Project>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public async Task Should_ReturnRolesAsync()
    {
        var list = await _redmineManager.GetAsync<Role>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public async Task Should_ReturnAttachmentsAsync()
    {
        var list = await _redmineManager.GetAsync<Attachment>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public async Task Should_ReturnCustomFieldsAsync()
    {
        var list = await _redmineManager.GetAsync<CustomField>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public async Task Should_ReturnGroupsAsync()
    {
        var list = await _redmineManager.GetAsync<Group>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public async Task Should_ReturnFilesAsync()
    {
        var list = await _redmineManager.GetAsync<Redmine.Net.Api.Types.File>(new RequestOptions()
        {
            QueryString = new NameValueCollection()
            {
                { RedmineKeys.PROJECT_ID, 1.ToString() }
            }
        });
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public async Task Should_ReturnIssuesAsync()
    {
        var list = await _redmineManager.GetAsync<Issue>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }
    
    [Fact]
    public async Task GetIssue_WithVersions_ShouldReturnAsync()
    {
        var issue = await _redmineManager.GetAsync<Issue>(5.ToInvariantString(), 
            new RequestOptions { 
                QueryString = new NameValueCollection()
                {
                    { RedmineKeys.INCLUDE, RedmineKeys.WATCHERS }
                } 
            }
        );
        Assert.NotNull(issue);
    }

    [Fact]
    public async Task Should_ReturnIssueCategoriesAsync()
    {
        var list = await _redmineManager.GetAsync<IssueCategory>(new RequestOptions()
        {
            QueryString = new NameValueCollection()
            {
                { RedmineKeys.PROJECT_ID, 1.ToString() }
            }
        });
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public async Task Should_ReturnIssueCustomFieldsAsync()
    {
        var list = await _redmineManager.GetAsync<IssueCustomField>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public async Task Should_ReturnIssuePrioritiesAsync()
    {
        var list = await _redmineManager.GetAsync<IssuePriority>(new RequestOptions()
        {
            QueryString = new NameValueCollection()
            {
                { RedmineKeys.ISSUE_ID, 1.ToString() }
            }
        });
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public async Task Should_ReturnIssueRelationsAsync()
    {
        var list = await _redmineManager.GetAsync<IssueRelation>(new RequestOptions()
        {
            QueryString = new NameValueCollection()
            {
                { RedmineKeys.ISSUE_ID, 1.ToString() }
            }
        });
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public async Task Should_ReturnIssueStatusesAsync()
    {
        var list = await _redmineManager.GetAsync<IssueStatus>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public async Task Should_ReturnJournalsAsync()
    {
        var list = await _redmineManager.GetAsync<Journal>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public async Task Should_ReturnNewsAsync()
    {
        var list = await _redmineManager.GetAsync<News>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public async Task Should_ReturnProjectMembershipsAsync()
    {
        var list = await _redmineManager.GetAsync<ProjectMembership>(new RequestOptions()
        {
            QueryString = new NameValueCollection()
            {
                { RedmineKeys.PROJECT_ID, 1.ToString() }
            }
        });
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public async Task Should_ReturnQueriesAsync()
    {
        var list = await _redmineManager.GetAsync<Query>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public async Task Should_ReturnSearchesAsync()
    {
        var list = await _redmineManager.GetAsync<Search>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public async Task Should_ReturnTimeEntriesAsync()
    {
        var list = await _redmineManager.GetAsync<TimeEntry>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public async Task Should_ReturnTimeEntryActivitiesAsync()
    {
        var list = await _redmineManager.GetAsync<TimeEntryActivity>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public async Task Should_ReturnTrackersAsync()
    {
        var list = await _redmineManager.GetAsync<Tracker>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public async Task Should_ReturnUsersAsync()
    {
        var list = await _redmineManager.GetAsync<User>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public async Task Should_ReturnVersionsAsync()
    {
        var list = await _redmineManager.GetAsync<Version>(new RequestOptions()
        {
            QueryString = new NameValueCollection()
            {
                { RedmineKeys.PROJECT_ID, 1.ToString() }
            }
        });
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public async Task Should_ReturnWatchersAsync()
    {
        var list = await _redmineManager.GetAsync<Watcher>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }
}