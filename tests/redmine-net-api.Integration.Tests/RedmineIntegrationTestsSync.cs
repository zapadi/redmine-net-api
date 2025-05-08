using System.Collections.Specialized;
using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Redmine.Net.Api;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Types;
using Version = Redmine.Net.Api.Types.Version;

namespace Padi.DotNet.RedmineAPI.Integration.Tests;

[Collection(Constants.RedmineTestContainerCollection)]
public class RedmineIntegrationTestsSync(RedmineTestContainerFixture fixture)
{
    private readonly RedmineManager _redmineManager = fixture.RedmineManager;

    [Fact]
    public void Should_ReturnProjects()
    {
        var list = _redmineManager.Get<Project>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public void Should_ReturnRoles()
    {
        var list = _redmineManager.Get<Role>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public void Should_ReturnAttachments()
    {
        var list = _redmineManager.Get<Attachment>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public void Should_ReturnCustomFields()
    {
        var list = _redmineManager.Get<CustomField>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public void Should_ReturnGroups()
    {
        var list = _redmineManager.Get<Group>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public void Should_ReturnFiles()
    {
        var list = _redmineManager.Get<Redmine.Net.Api.Types.File>(new RequestOptions()
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
    public void Should_ReturnIssues()
    {
        var list = _redmineManager.Get<Issue>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public void Should_ReturnIssueCategories()
    {
        var list = _redmineManager.Get<IssueCategory>(new RequestOptions()
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
    public void Should_ReturnIssueCustomFields()
    {
        var list = _redmineManager.Get<IssueCustomField>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public void Should_ReturnIssuePriorities()
    {
        var list = _redmineManager.Get<IssuePriority>(new RequestOptions()
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
    public void Should_ReturnIssueRelations()
    {
        var list = _redmineManager.Get<IssueRelation>(new RequestOptions()
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
    public void Should_ReturnIssueStatuses()
    {
        var list = _redmineManager.Get<IssueStatus>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public void Should_ReturnJournals()
    {
        var list = _redmineManager.Get<Journal>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public void Should_ReturnNews()
    {
        var list = _redmineManager.Get<News>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public void Should_ReturnProjectMemberships()
    {
        var list = _redmineManager.Get<ProjectMembership>(new RequestOptions()
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
    public void Should_ReturnQueries()
    {
        var list = _redmineManager.Get<Query>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public void Should_ReturnSearches()
    {
        var list = _redmineManager.Get<Search>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public void Should_ReturnTimeEntries()
    {
        var list = _redmineManager.Get<TimeEntry>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public void Should_ReturnTimeEntryActivities()
    {
        var list = _redmineManager.Get<TimeEntryActivity>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public void Should_ReturnTrackers()
    {
        var list = _redmineManager.Get<Tracker>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public void Should_ReturnUsers()
    {
        var list = _redmineManager.Get<User>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }

    [Fact]
    public void Should_ReturnVersions()
    {
        var list = _redmineManager.Get<Version>(new RequestOptions()
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
    public void Should_ReturnWatchers()
    {
        var list = _redmineManager.Get<Watcher>();
        Assert.NotNull(list);
        Assert.NotEmpty(list);
    }
}