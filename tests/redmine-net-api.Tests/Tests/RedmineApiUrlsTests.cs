using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Redmine.Net.Api;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Types;
using Xunit;
using Version = Redmine.Net.Api.Types.Version;


namespace Padi.DotNet.RedmineAPI.Tests.Tests;

public class RedmineApiUrlsTests(RedmineApiUrlsFixture fixture) : IClassFixture<RedmineApiUrlsFixture>
{
    private string GetUriWithFormat(string path)
    {
        return string.Format(path, fixture.Format);
    }
    
    [Fact]
    public void MyAccount_ReturnsCorrectUrl()
    {
        var result = fixture.Sut.MyAccount();
        Assert.Equal(GetUriWithFormat("my/account.{0}"), result);
    }

    [Theory]
    [InlineData("123", "456", "issues/123/watchers/456.{0}")]
    public void IssueWatcherRemove_WithValidIds_ReturnsCorrectUrl(string issueId, string userId, string expected)
    {
        var result = fixture.Sut.IssueWatcherRemove(issueId, userId);
        Assert.Equal(GetUriWithFormat(expected), result);
    }

    [Theory]
    [InlineData("test.txt", "uploads.{0}?filename=test.txt")]
    [InlineData("file with spaces.pdf", "uploads.{0}?filename=file%20with%20spaces.pdf")]
    public void UploadFragment_WithFileName_ReturnsCorrectlyEncodedUrl(string fileName, string expected)
    {
        var result = fixture.Sut.UploadFragment(fileName);
        Assert.Equal(GetUriWithFormat(expected), result);
    }

    [Theory]
    [InlineData("project1", "versions")]
    [InlineData("project1", "issue_categories")]
    public void ProjectParentFragment_ForDifferentTypes_ReturnsCorrectUrl(string projectId, string fragment)
    {
        var expected = $"projects/{projectId}/{fragment}.{{0}}";
        var result = fixture.Sut.ProjectParentFragment(projectId, fragment);
        Assert.Equal(GetUriWithFormat(expected), result);
    }

    [Theory]
    [InlineData("issue1", "relations")]
    [InlineData("issue1", "watchers")]
    public void IssueParentFragment_ForDifferentTypes_ReturnsCorrectUrl(string issueId, string fragment)
    {
        var expected = $"issues/{issueId}/{fragment}.{{0}}";
        var result = fixture.Sut.IssueParentFragment(issueId, fragment);
        Assert.Equal(GetUriWithFormat(expected), result);
    }

    [Theory]
    [MemberData(nameof(GetFragmentTestData))]
    public void GetFragment_ForAllTypes_ReturnsCorrectUrl(Type type, string id, string expected) 
    {
        var result = fixture.Sut.GetFragment(type, id);
        Assert.Equal(GetUriWithFormat(expected), result);
    }

    [Theory]
    [MemberData(nameof(CreateEntityTestData))]
    public void CreateEntity_ForAllTypes_ReturnsCorrectUrl(Type type, string ownerId, string expected) 
    {
        var result = fixture.Sut.CreateEntityFragment(type, ownerId);
        Assert.Equal(GetUriWithFormat(expected), result);
    }

    [Theory]
    [MemberData(nameof(GetListTestData))]
    public void GetList_ForAllTypes_ReturnsCorrectUrl(Type type, string ownerId, string expected) 
    {
        var result = fixture.Sut.GetListFragment(type, ownerId);
        Assert.Equal(GetUriWithFormat(expected), result);
    }

    [Theory]
    [MemberData(nameof(InvalidTypeTestData))]
    public void GetList_WithInvalidType_ThrowRedmineException(Type invalidType)
    {
        var exception = Assert.Throws<RedmineException>(() => fixture.Sut.GetListFragment(invalidType));
        Assert.Contains("There is no uri fragment defined for type", exception.Message);
    }
   
    [Theory]
    [MemberData(nameof(GetListWithIssueIdTestData))]
    public void GetListFragment_WithIssueIdInRequestOptions_ReturnsCorrectUrl(Type type, string issueId, string expected) 
    {
        var requestOptions = new RequestOptions
        {
            QueryString = new NameValueCollection
            {
                { RedmineKeys.ISSUE_ID, issueId }
            }
        };

        var result = fixture.Sut.GetListFragment(type, requestOptions);
        Assert.Equal(GetUriWithFormat(expected), result);
    }
    
    [Theory]
    [MemberData(nameof(GetListWithProjectIdTestData))]
    public void GetListFragment_WithProjectIdInRequestOptions_ReturnsCorrectUrl(Type type, string projectId, string expected) 
    {
        var requestOptions = new RequestOptions
        {
            QueryString = new NameValueCollection
            {
                { RedmineKeys.PROJECT_ID, projectId }
            }
        };

        var result = fixture.Sut.GetListFragment(type, requestOptions);
        Assert.Equal(GetUriWithFormat(expected), result);
    }

    [Theory]
    [MemberData(nameof(GetListWithBothIdsTestData))]
    public void GetListFragment_WithBothIds_PrioritizesProjectId(Type type, string projectId, string issueId, string expected) 
    {
        var requestOptions = new RequestOptions
        {
            QueryString = new NameValueCollection
            {
                { RedmineKeys.PROJECT_ID, projectId },
                { RedmineKeys.ISSUE_ID, issueId }
            }
        };

        var result = fixture.Sut.GetListFragment(type, requestOptions);
        Assert.Equal(GetUriWithFormat(expected), result);
    }

    [Theory]
    [MemberData(nameof(GetListWithNoIdsTestData))]
    public void GetListFragment_WithNoIds_ReturnsDefaultUrl(Type type, string expected) 
    {
        var result = fixture.Sut.GetListFragment(type, new RequestOptions());
        Assert.Equal(GetUriWithFormat(expected), result);
    }
    
    [Theory]
    [MemberData(nameof(RedmineTypeTestData))]
    public void GetListFragment_ForAllTypes_ReturnsCorrectUrl(Type type, string parentId, string expected)
    {
        var result = fixture.Sut.GetListFragment(type, parentId);
        Assert.Equal(GetUriWithFormat(expected), result);
    }

    [Theory]
    [MemberData(nameof(GetListEntityRequestOptionTestData))]
    public void GetListFragment_WithEmptyOptions_ReturnsCorrectUrl(Type type, RequestOptions requestOptions, string expected)
    {
        var result = fixture.Sut.GetListFragment(type, requestOptions);
        Assert.Equal(GetUriWithFormat(expected), result);
    }

    [Theory]
    [MemberData(nameof(RedmineTypeTestData))]
    public void GetListFragment_WithNullOptions_ReturnsCorrectUrl(Type type, string parentId, string expected)
    {
        var result = fixture.Sut.GetListFragment(type, parentId);
        Assert.Equal(GetUriWithFormat(expected), result);
    }

    [Theory]
    [MemberData(nameof(GetListWithNullRequestOptionsTestData))]
    public void GetListFragment_WithNullRequestOptions_ReturnsDefaultUrl(Type type, string expected) 
    {
        var result = fixture.Sut.GetListFragment(type, (RequestOptions)null);
        Assert.Equal(GetUriWithFormat(expected), result);
    }

    [Theory]
    [MemberData(nameof(GetListWithEmptyQueryStringTestData))]
    public void GetListFragment_WithEmptyQueryString_ReturnsDefaultUrl(Type type, string expected) 
    {
        var requestOptions = new RequestOptions
        {
            QueryString = null
        };

        var result = fixture.Sut.GetListFragment(type, requestOptions);
        Assert.Equal(GetUriWithFormat(expected), result);
    }

    [Fact]
    public void GetListFragment_WithCustomQueryParameters_DoesNotAffectUrl()
    {
        var requestOptions = new RequestOptions
        {
            QueryString = new NameValueCollection
            {
                { "status_id", "1" },
                { "assigned_to_id", "me" },
                { "sort", "priority:desc" }
            }
        };

        var result = fixture.Sut.GetListFragment<Issue>(requestOptions);
        Assert.Equal(GetUriWithFormat("issues.{0}"), result);
    }

    [Theory]
    [MemberData(nameof(GetListWithInvalidTypeTestData))]
    public void GetListFragment_WithInvalidType_ThrowsRedmineException(Type invalidType)
    {
        var exception = Assert.Throws<RedmineException>(() => fixture.Sut.GetListFragment(invalidType));
        
        Assert.Contains("There is no uri fragment defined for type", exception.Message);
    }

    public static TheoryData<Type, string, string, string> GetListWithBothIdsTestData()
    {
        return new TheoryData<Type, string, string, string>
        {
            { 
                typeof(Version), 
                "project1", 
                "issue1", 
                "projects/project1/versions.{0}" 
            },
            { 
                typeof(IssueCategory), 
                "project2", 
                "issue2", 
                "projects/project2/issue_categories.{0}" 
            }
        };
    }
    
    public static IEnumerable<TheoryDataRow<Type, string, string>> RedmineTypeTestData()
    {
        yield return new TheoryDataRow<Type, string, string>(typeof(Issue), null, "issues.{0}")
            .WithTestDisplayName("Issue → issues.{0}");
        yield return new TheoryDataRow<Type, string, string>(typeof(Project), null, "projects.{0}")
            .WithTestDisplayName("Project → projects.{0}");
        yield return new TheoryDataRow<Type, string, string>(typeof(User), null, "users.{0}")
            .WithTestDisplayName("User → users.{0}");
        yield return new TheoryDataRow<Type, string, string>(typeof(TimeEntry), null, "time_entries.{0}")
            .WithTestDisplayName("TimeEntry → time_entries.{0}");
        yield return new TheoryDataRow<Type, string, string>(typeof(CustomField), null, "custom_fields.{0}")
            .WithTestDisplayName("CustomField → custom_fields.{0}");
        yield return new TheoryDataRow<Type, string, string>(typeof(Group), null, "groups.{0}")
            .WithTestDisplayName("Group → groups.{0}");
        yield return new TheoryDataRow<Type, string, string>(typeof(News), null, "news.{0}")
            .WithTestDisplayName("News → news.{0}");
        yield return new TheoryDataRow<Type, string, string>(typeof(Query), null, "queries.{0}")
            .WithTestDisplayName("Query → queries.{0}");
        yield return new TheoryDataRow<Type, string, string>(typeof(Role), null, "roles.{0}")
            .WithTestDisplayName("Role → roles.{0}");
        yield return new TheoryDataRow<Type, string, string>(typeof(IssueStatus), null, "issue_statuses.{0}")
            .WithTestDisplayName("IssueStatus → issue_statuses.{0}");
        yield return new TheoryDataRow<Type, string, string>(typeof(Tracker), null, "trackers.{0}")
            .WithTestDisplayName("Tracker → trackers.{0}");
        yield return new TheoryDataRow<Type, string, string>(typeof(IssuePriority), null, "enumerations/issue_priorities.{0}")
            .WithTestDisplayName("IssuePriority → enumerations/issue_priorities.{0}");
        yield return new TheoryDataRow<Type, string, string>(typeof(TimeEntryActivity), null, "enumerations/time_entry_activities.{0}")
            .WithTestDisplayName("TimeEntryActivity → enumerations/time_entry_activities.{0}");
        yield return new TheoryDataRow<Type, string, string>(typeof(Version), "1", "projects/1/versions.{0}")
            .WithTestDisplayName("Version (parent=1) → projects/1/versions.{0}");
        yield return new TheoryDataRow<Type, string, string>(typeof(IssueCategory), "1", "projects/1/issue_categories.{0}")
            .WithTestDisplayName("IssueCategory (parent=1) → projects/1/issue_categories.{0}");
        yield return new TheoryDataRow<Type, string, string>(typeof(ProjectMembership), "1", "projects/1/memberships.{0}")
            .WithTestDisplayName("ProjectMembership (parent=1) → projects/1/memberships.{0}");
        yield return new TheoryDataRow<Type, string, string>(typeof(IssueRelation), "1", "issues/1/relations.{0}")
            .WithTestDisplayName("IssueRelation (parent=1) → issues/1/relations.{0}");
        yield return new TheoryDataRow<Type, string, string>(typeof(Attachment), null, "attachments.{0}")
            .WithTestDisplayName("Attachment → attachments.{0}");
        yield return new TheoryDataRow<Type, string, string>(typeof(IssueCustomField), null, "custom_fields.{0}")
            .WithTestDisplayName("IssueCustomField → custom_fields.{0}");
        yield return new TheoryDataRow<Type, string, string>(typeof(Journal), null, "journals.{0}")
            .WithTestDisplayName("Journal → journals.{0}");
        yield return new TheoryDataRow<Type, string, string>(typeof(Search), null, "search.{0}")
            .WithTestDisplayName("Search → search.{0}");
        yield return new TheoryDataRow<Type, string, string>(typeof(Watcher), null, "watchers.{0}")
            .WithTestDisplayName("Watcher → watchers.{0}");
    }
    
    public static TheoryData<Type, string, string> GetFragmentTestData()
    {
        return new TheoryData<Type, string, string>
        {
            { typeof(Attachment), "1", "attachments/1.{0}" },
            { typeof(CustomField), "2", "custom_fields/2.{0}" },
            { typeof(Group), "3", "groups/3.{0}" },
            { typeof(Issue), "4", "issues/4.{0}" },
            { typeof(IssueCategory), "5", "issue_categories/5.{0}" },
            { typeof(IssueCustomField), "6", "custom_fields/6.{0}" },
            { typeof(IssuePriority), "7", "enumerations/issue_priorities/7.{0}" },
            { typeof(IssueRelation), "8", "relations/8.{0}" },
            { typeof(IssueStatus), "9", "issue_statuses/9.{0}" },
            { typeof(Journal), "10", "journals/10.{0}" },
            { typeof(News), "11", "news/11.{0}" },
            { typeof(Project), "12", "projects/12.{0}" },
            { typeof(ProjectMembership), "13", "memberships/13.{0}" },
            { typeof(Query), "14", "queries/14.{0}" },
            { typeof(Role), "15", "roles/15.{0}" },
            { typeof(Search), "16", "search/16.{0}" },
            { typeof(TimeEntry), "17", "time_entries/17.{0}" },
            { typeof(TimeEntryActivity), "18", "enumerations/time_entry_activities/18.{0}" },
            { typeof(Tracker), "19", "trackers/19.{0}" },
            { typeof(User), "20", "users/20.{0}" },
            { typeof(Version), "21", "versions/21.{0}" },
            { typeof(Watcher), "22", "watchers/22.{0}" }
        };
    }

    public static TheoryData<Type, string, string> CreateEntityTestData()
    {
        return new TheoryData<Type, string, string>
        {
            { typeof(Version), "project1", "projects/project1/versions.{0}" },
            { typeof(IssueCategory), "project1", "projects/project1/issue_categories.{0}" },
            { typeof(ProjectMembership), "project1", "projects/project1/memberships.{0}" },
            
            { typeof(IssueRelation), "issue1", "issues/issue1/relations.{0}" },
            
            { typeof(File), "project1", "projects/project1/files.{0}" },
            { typeof(Upload), null, "uploads.{0}" },
            { typeof(Attachment), "issue1", "/attachments/issues/issue1.{0}" },
            
            { typeof(Issue), null, "issues.{0}" },
            { typeof(Project), null, "projects.{0}" },
            { typeof(User), null, "users.{0}" },
            { typeof(TimeEntry), null, "time_entries.{0}" },
            { typeof(News), null, "news.{0}" },
            { typeof(Query), null, "queries.{0}" },
            { typeof(Role), null, "roles.{0}" },
            { typeof(Group), null, "groups.{0}" },
            { typeof(CustomField), null, "custom_fields.{0}" },
            { typeof(IssueStatus), null, "issue_statuses.{0}" },
            { typeof(Tracker), null, "trackers.{0}" },
            { typeof(IssuePriority), null, "enumerations/issue_priorities.{0}" },
            { typeof(TimeEntryActivity), null, "enumerations/time_entry_activities.{0}" }
        };
    }
    
    public static TheoryData<Type, RequestOptions, string> GetListEntityRequestOptionTestData()
    {
        var rqWithProjectId = new RequestOptions()
        {
            QueryString = new NameValueCollection()
            {
                {RedmineKeys.PROJECT_ID, "project1"}
            }
        };
        var rqWithPIssueId = new RequestOptions()
        {
            QueryString = new NameValueCollection()
            {   
                {RedmineKeys.ISSUE_ID, "issue1"}
            }
        };
        return new TheoryData<Type, RequestOptions, string>
        {
            { typeof(Version), rqWithProjectId, "projects/project1/versions.{0}" },
            { typeof(IssueCategory), rqWithProjectId, "projects/project1/issue_categories.{0}" },
            { typeof(ProjectMembership), rqWithProjectId, "projects/project1/memberships.{0}" },
            
            { typeof(IssueRelation), rqWithPIssueId, "issues/issue1/relations.{0}" },
            
            { typeof(File), rqWithProjectId, "projects/project1/files.{0}" },
            { typeof(Attachment), rqWithPIssueId, "attachments.{0}" },
            
            { typeof(Issue), null, "issues.{0}" },
            { typeof(Project), null, "projects.{0}" },
            { typeof(User), null, "users.{0}" },
            { typeof(TimeEntry), null, "time_entries.{0}" },
            { typeof(News), null, "news.{0}" },
            { typeof(Query), null, "queries.{0}" },
            { typeof(Role), null, "roles.{0}" },
            { typeof(Group), null, "groups.{0}" },
            { typeof(CustomField), null, "custom_fields.{0}" },
            { typeof(IssueStatus), null, "issue_statuses.{0}" },
            { typeof(Tracker), null, "trackers.{0}" },
            { typeof(IssuePriority), null, "enumerations/issue_priorities.{0}" },
            { typeof(TimeEntryActivity), null, "enumerations/time_entry_activities.{0}" }
        };
    }

    public static TheoryData<Type, string, string> GetListTestData()
    {
        return new TheoryData<Type, string, string>
        {
            { typeof(Version), "project1", "projects/project1/versions.{0}" },
            { typeof(IssueCategory), "project1", "projects/project1/issue_categories.{0}" },
            { typeof(ProjectMembership), "project1", "projects/project1/memberships.{0}" },
            
            { typeof(IssueRelation), "issue1", "issues/issue1/relations.{0}" },
            
            { typeof(File), "project1", "projects/project1/files.{0}" },
            
            { typeof(Issue), null, "issues.{0}" },
            { typeof(Project), null, "projects.{0}" },
            { typeof(User), null, "users.{0}" },
            { typeof(TimeEntry), null, "time_entries.{0}" },
            { typeof(News), null, "news.{0}" },
            { typeof(Query), null, "queries.{0}" },
            { typeof(Role), null, "roles.{0}" },
            { typeof(Group), null, "groups.{0}" },
            { typeof(CustomField), null, "custom_fields.{0}" },
            { typeof(IssueStatus), null, "issue_statuses.{0}" },
            { typeof(Tracker), null, "trackers.{0}" },
            { typeof(IssuePriority), null, "enumerations/issue_priorities.{0}" },
            { typeof(TimeEntryActivity), null, "enumerations/time_entry_activities.{0}" }
        };
    }
    
    public static TheoryData<Type, string, string> GetListWithIssueIdTestData()
    {
        return new TheoryData<Type, string, string>
        {
            { typeof(IssueRelation), "issue1", "issues/issue1/relations.{0}" },
        };
    }
    
    public static TheoryData<Type, string, string> GetListWithProjectIdTestData()
    {
        return new TheoryData<Type, string, string>
        {
            { typeof(Version), "1", "projects/1/versions.{0}" },
            { typeof(IssueCategory), "1", "projects/1/issue_categories.{0}" },
            { typeof(ProjectMembership), "1", "projects/1/memberships.{0}" },
            { typeof(File), "1", "projects/1/files.{0}" },
        };
    }
    
    public static TheoryData<Type, string> GetListWithNullRequestOptionsTestData()
    {
        return new TheoryData<Type, string>
        {
            { typeof(Issue), "issues.{0}" },
            { typeof(Project), "projects.{0}" },
            { typeof(User), "users.{0}" }
        };
    }
    
    public static TheoryData<Type, string> GetListWithEmptyQueryStringTestData()
    {
        return new TheoryData<Type, string>
        {
            { typeof(Issue), "issues.{0}" },
            { typeof(Project), "projects.{0}" },
            { typeof(User), "users.{0}" }
        };
    }
    
    public static TheoryData<Type> GetListWithInvalidTypeTestData()
    {
        return
        [
            typeof(string),
            typeof(int),
            typeof(DateTime),
            typeof(object)
        ];
    }
    
    public static TheoryData<Type, string> GetListWithNoIdsTestData()
    {
        return new TheoryData<Type, string>
        {
            { typeof(Issue), "issues.{0}" },
            { typeof(Project), "projects.{0}" },
            { typeof(User), "users.{0}" },
            { typeof(TimeEntry), "time_entries.{0}" },
            { typeof(CustomField), "custom_fields.{0}" }
        };
    }
    
    public static TheoryData<Type> InvalidTypeTestData()
    {
        return
        [
            typeof(object),
            typeof(int)
        ];
    }
}