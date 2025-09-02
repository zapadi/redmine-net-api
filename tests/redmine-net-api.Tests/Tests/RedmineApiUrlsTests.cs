using System;
using System.Collections.Specialized;
using Padi.RedmineApi;
using Padi.RedmineApi.Exceptions;
using Padi.RedmineApi.Internals;
using Padi.RedmineApi.Net;
using Padi.RedmineAPI.Tests.Infrastructure.Fixtures;
using Padi.RedmineApi.Types;
using Xunit;
using Version = Padi.RedmineApi.Types.Version;


namespace Padi.RedmineAPI.Tests.Tests;

public class RedmineApiUrlsTests(RedmineApiUrlsFixture fixture) : IClassFixture<RedmineApiUrlsFixture>
{
    [Fact]
    public void MyAccount_ReturnsCorrectUrl()
    {
        var result = fixture.Sut.MyAccount();
        Assert.Equal("my/account.json", result);
    }

    [Theory]
    [MemberData(nameof(ProjectOperationsData))]
    public void ProjectOperations_ReturnsCorrectUrl(string projectId, Func<string, string> operation, string expected)
    {
        var result = operation(projectId);
        Assert.Equal(expected, result);
    }

    [Theory]
    [MemberData(nameof(WikiOperationsData))]
    public void WikiOperations_ReturnsCorrectUrl(string projectId, string pageName, Func<string, string, string> operation, string expected)
    {
        var result = operation(projectId, pageName);
        Assert.Equal(expected, result);
    }
   
    [Theory]
    [InlineData("123", "456", "issues/123/watchers/456.json")]
    public void IssueWatcherRemove_WithValidIds_ReturnsCorrectUrl(string issueId, string userId, string expected)
    {
        var result = fixture.Sut.IssueWatcherRemove(issueId, userId);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(null, "456")]
    [InlineData("123", null)]
    [InlineData("", "456")]
    [InlineData("123", "")]
    public void IssueWatcherRemove_WithInvalidIds_ThrowsRedmineException(string issueId, string userId)
    {
        Assert.Throws<RedmineException>(() => fixture.Sut.IssueWatcherRemove(issueId, userId));
    }

    [Theory]
    [MemberData(nameof(AttachmentOperationsData))]
    public void AttachmentOperations_WithValidInput_ReturnsCorrectUrl(string input, Func<string, string> operation, string expected)
    {
        var result = operation(input);
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [InlineData("test.txt", "uploads.json?filename=test.txt")]
    [InlineData("file with spaces.pdf", "uploads.json?filename=file%20with%20spaces.pdf")]
    public void UploadFragment_WithFileName_ReturnsCorrectlyEncodedUrl(string fileName, string expected)
    {
        var result = fixture.Sut.UploadFragment(fileName);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("project1", "versions")]
    [InlineData("project1", "issue_categories")]
    public void ProjectParentFragment_ForDifferentTypes_ReturnsCorrectUrl(string projectId, string fragment)
    {
        var expected = $"projects/{projectId}/{fragment}.json";
        var result = fixture.Sut.ProjectParentFragment(projectId, fragment);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("issue1", "relations")]
    [InlineData("issue1", "watchers")]
    public void IssueParentFragment_ForDifferentTypes_ReturnsCorrectUrl(string issueId, string fragment)
    {
        var expected = $"issues/{issueId}/{fragment}.json";
        var result = fixture.Sut.IssueParentFragment(issueId, fragment);
        Assert.Equal(expected, result);
    }

    [Theory]
    [MemberData(nameof(GetFragmentTestData))]
    public void GetFragment_ForAllTypes_ReturnsCorrectUrl(Type type, string id, string expected) 
    {
        var result = fixture.Sut.GetFragment(type, id);
        Assert.Equal(expected, result);
    }

    [Theory]
    [MemberData(nameof(CreateEntityTestData))]
    public void CreateEntity_ForAllTypes_ReturnsCorrectUrl(Type type, string ownerId, string expected) 
    {
        var result = fixture.Sut.CreateEntityFragment(type, ownerId);
        Assert.Equal(expected, result);
    }

    [Theory]
    [MemberData(nameof(GetListTestData))]
    public void GetList_ForAllTypes_ReturnsCorrectUrl(Type type, string ownerId, string expected) 
    {
        var result = fixture.Sut.GetListFragment(type, ownerId);
        Assert.Equal(expected, result);
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
        Assert.Equal(expected, result);
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
        Assert.Equal(expected, result);
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
        Assert.Equal(expected, result);
    }

    [Theory]
    [MemberData(nameof(GetListWithNoIdsTestData))]
    public void GetListFragment_WithNoIds_ReturnsDefaultUrl(Type type, string expected) 
    {
        var result = fixture.Sut.GetListFragment(type, new RequestOptions());
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [ClassData(typeof(RedmineTypeTestData))]
    public void GetListFragment_ForAllTypes_ReturnsCorrectUrl(Type type, string parentId, string expected)
    {
        var result = fixture.Sut.GetListFragment(type, parentId);
        Assert.Equal(expected, result);
    }

    [Theory]
    [MemberData(nameof(GetListEntityRequestOptionTestData))]
    public void GetListFragment_WithEmptyOptions_ReturnsCorrectUrl(Type type, RequestOptions requestOptions, string expected)
    {
        var result = fixture.Sut.GetListFragment(type, requestOptions);
        Assert.Equal(expected, result);
    }

    [Theory]
    [ClassData(typeof(RedmineTypeTestData))]
    public void GetListFragment_WithNullOptions_ReturnsCorrectUrl(Type type, string parentId, string expected)
    {
        var result = fixture.Sut.GetListFragment(type, parentId);
        Assert.Equal(expected, result);
    }

    [Theory]
    [MemberData(nameof(GetListWithNullRequestOptionsTestData))]
    public void GetListFragment_WithNullRequestOptions_ReturnsDefaultUrl(Type type, string expected) 
    {
        var result = fixture.Sut.GetListFragment(type, (RequestOptions)null);
        Assert.Equal(expected, result);
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
        Assert.Equal(expected, result);
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
        Assert.Equal("issues.json", result);
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
                "projects/project1/versions.json" 
            },
            { 
                typeof(IssueCategory), 
                "project2", 
                "issue2", 
                "projects/project2/issue_categories.json" 
            }
        };
    }
    
    public class RedmineTypeTestData : TheoryData<Type, string, string>
    {
        public RedmineTypeTestData()
        {
            Add<Issue>(null, "issues.json");
            Add<Project>(null,"projects.json");
            Add<User>(null,"users.json");
            Add<TimeEntry>(null,"time_entries.json");
            Add<CustomField>(null,"custom_fields.json");
            Add<Group>(null,"groups.json");
            Add<News>(null,"news.json");
            Add<Query>(null,"queries.json");
            Add<Role>(null,"roles.json");
            Add<IssueStatus>(null,"issue_statuses.json");
            Add<Tracker>(null,"trackers.json");
            Add<IssuePriority>(null,"enumerations/issue_priorities.json");
            Add<TimeEntryActivity>(null,"enumerations/time_entry_activities.json");
            Add<Version>("1","projects/1/versions.json");
            Add<IssueCategory>("1","projects/1/issue_categories.json");
            Add<ProjectMembership>("1","projects/1/memberships.json");
            Add<IssueRelation>("1","issues/1/relations.json");
            Add<Attachment>(null,"attachments.json");
            Add<IssueCustomField>(null,"custom_fields.json");
            Add<Journal>(null,"journals.json");
            Add<Search>(null,"search.json");
            Add<Watcher>(null,"watchers.json");
        }

        private void Add<T>(string parentId, string expected) where T : class, new()
        {
            AddRow(typeof(T), parentId, expected);
        }
    }
    
    public static TheoryData<Type, string, string> GetFragmentTestData()
    {
        return new TheoryData<Type, string, string>
        {
            { typeof(Attachment), "1", "attachments/1.json" },
            { typeof(CustomField), "2", "custom_fields/2.json" },
            { typeof(Group), "3", "groups/3.json" },
            { typeof(Issue), "4", "issues/4.json" },
            { typeof(IssueCategory), "5", "issue_categories/5.json" },
            { typeof(IssueCustomField), "6", "custom_fields/6.json" },
            { typeof(IssuePriority), "7", "enumerations/issue_priorities/7.json" },
            { typeof(IssueRelation), "8", "relations/8.json" },
            { typeof(IssueStatus), "9", "issue_statuses/9.json" },
            { typeof(Journal), "10", "journals/10.json" },
            { typeof(News), "11", "news/11.json" },
            { typeof(Project), "12", "projects/12.json" },
            { typeof(ProjectMembership), "13", "memberships/13.json" },
            { typeof(Query), "14", "queries/14.json" },
            { typeof(Role), "15", "roles/15.json" },
            { typeof(Search), "16", "search/16.json" },
            { typeof(TimeEntry), "17", "time_entries/17.json" },
            { typeof(TimeEntryActivity), "18", "enumerations/time_entry_activities/18.json" },
            { typeof(Tracker), "19", "trackers/19.json" },
            { typeof(User), "20", "users/20.json" },
            { typeof(Version), "21", "versions/21.json" },
            { typeof(Watcher), "22", "watchers/22.json" }
        };
    }

    public static TheoryData<Type, string, string> CreateEntityTestData()
    {
        return new TheoryData<Type, string, string>
        {
            { typeof(Version), "project1", "projects/project1/versions.json" },
            { typeof(IssueCategory), "project1", "projects/project1/issue_categories.json" },
            { typeof(ProjectMembership), "project1", "projects/project1/memberships.json" },
            
            { typeof(IssueRelation), "issue1", "issues/issue1/relations.json" },
            
            { typeof(File), "project1", "projects/project1/files.json" },
            { typeof(Upload), null, "uploads.json" },
            { typeof(Attachment), "issue1", "/attachments/issues/issue1.json" },
            
            { typeof(Issue), null, "issues.json" },
            { typeof(Project), null, "projects.json" },
            { typeof(User), null, "users.json" },
            { typeof(TimeEntry), null, "time_entries.json" },
            { typeof(News), null, "news.json" },
            { typeof(Query), null, "queries.json" },
            { typeof(Role), null, "roles.json" },
            { typeof(Group), null, "groups.json" },
            { typeof(CustomField), null, "custom_fields.json" },
            { typeof(IssueStatus), null, "issue_statuses.json" },
            { typeof(Tracker), null, "trackers.json" },
            { typeof(IssuePriority), null, "enumerations/issue_priorities.json" },
            { typeof(TimeEntryActivity), null, "enumerations/time_entry_activities.json" }
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
            { typeof(Version), rqWithProjectId, "projects/project1/versions.json" },
            { typeof(IssueCategory), rqWithProjectId, "projects/project1/issue_categories.json" },
            { typeof(ProjectMembership), rqWithProjectId, "projects/project1/memberships.json" },
            
            { typeof(IssueRelation), rqWithPIssueId, "issues/issue1/relations.json" },
            
            { typeof(File), rqWithProjectId, "projects/project1/files.json" },
            { typeof(Attachment), rqWithPIssueId, "attachments.json" },
            
            { typeof(Issue), null, "issues.json" },
            { typeof(Project), null, "projects.json" },
            { typeof(User), null, "users.json" },
            { typeof(TimeEntry), null, "time_entries.json" },
            { typeof(News), null, "news.json" },
            { typeof(Query), null, "queries.json" },
            { typeof(Role), null, "roles.json" },
            { typeof(Group), null, "groups.json" },
            { typeof(CustomField), null, "custom_fields.json" },
            { typeof(IssueStatus), null, "issue_statuses.json" },
            { typeof(Tracker), null, "trackers.json" },
            { typeof(IssuePriority), null, "enumerations/issue_priorities.json" },
            { typeof(TimeEntryActivity), null, "enumerations/time_entry_activities.json" }
        };
    }

    public static TheoryData<Type, string, string> GetListTestData()
    {
        return new TheoryData<Type, string, string>
        {
            { typeof(Version), "project1", "projects/project1/versions.json" },
            { typeof(IssueCategory), "project1", "projects/project1/issue_categories.json" },
            { typeof(ProjectMembership), "project1", "projects/project1/memberships.json" },
            
            { typeof(IssueRelation), "issue1", "issues/issue1/relations.json" },
            
            { typeof(File), "project1", "projects/project1/files.json" },
            
            { typeof(Issue), null, "issues.json" },
            { typeof(Project), null, "projects.json" },
            { typeof(User), null, "users.json" },
            { typeof(TimeEntry), null, "time_entries.json" },
            { typeof(News), null, "news.json" },
            { typeof(Query), null, "queries.json" },
            { typeof(Role), null, "roles.json" },
            { typeof(Group), null, "groups.json" },
            { typeof(CustomField), null, "custom_fields.json" },
            { typeof(IssueStatus), null, "issue_statuses.json" },
            { typeof(Tracker), null, "trackers.json" },
            { typeof(IssuePriority), null, "enumerations/issue_priorities.json" },
            { typeof(TimeEntryActivity), null, "enumerations/time_entry_activities.json" }
        };
    }
    
    public static TheoryData<Type, string, string> GetListWithIssueIdTestData()
    {
        return new TheoryData<Type, string, string>
        {
            { typeof(IssueRelation), "issue1", "issues/issue1/relations.json" },
        };
    }
    
    public static TheoryData<Type, string, string> GetListWithProjectIdTestData()
    {
        return new TheoryData<Type, string, string>
        {
            { typeof(Version), "1", "projects/1/versions.json" },
            { typeof(IssueCategory), "1", "projects/1/issue_categories.json" },
            { typeof(ProjectMembership), "1", "projects/1/memberships.json" },
            { typeof(File), "1", "projects/1/files.json" },
        };
    }
    
    public static TheoryData<Type, string> GetListWithNullRequestOptionsTestData()
    {
        return new TheoryData<Type, string>
        {
            { typeof(Issue), "issues.json" },
            { typeof(Project), "projects.json" },
            { typeof(User), "users.json" }
        };
    }
    
    public static TheoryData<Type, string> GetListWithEmptyQueryStringTestData()
    {
        return new TheoryData<Type, string>
        {
            { typeof(Issue), "issues.json" },
            { typeof(Project), "projects.json" },
            { typeof(User), "users.json" }
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
            { typeof(Issue), "issues.json" },
            { typeof(Project), "projects.json" },
            { typeof(User), "users.json" },
            { typeof(TimeEntry), "time_entries.json" },
            { typeof(CustomField), "custom_fields.json" }
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
    
    public static TheoryData<string, Func<string, string>, string> AttachmentOperationsData()
    {
        var fixture = new RedmineApiUrlsFixture();
        return new TheoryData<string, Func<string, string>, string>
        {
            { 
                "123", 
                id => fixture.Sut.AttachmentUpdate(id),
                "attachments/issues/123.json" 
            },
            { 
                "456", 
                id => fixture.Sut.IssueWatcherAdd(id),
                "issues/456/watchers.json" 
            }
        };
    }
    
    public static TheoryData<string, Func<string, string>, string> ProjectOperationsData()
    {
        var fixture = new RedmineApiUrlsFixture();
        return new TheoryData<string, Func<string, string>, string>
        {
            { 
                "test-project", 
                id => fixture.Sut.ProjectClose(id), 
                "projects/test-project/close.json" 
            },
            { 
                "test-project", 
                id => fixture.Sut.ProjectReopen(id), 
                "projects/test-project/reopen.json" 
            },
            { 
                "test-project", 
                id => fixture.Sut.ProjectArchive(id), 
                "projects/test-project/archive.json" 
            },
            { 
                "test-project", 
                id => fixture.Sut.ProjectUnarchive(id), 
                "projects/test-project/unarchive.json" 
            }
        };
    }
    
    public static TheoryData<string, string, Func<string, string, string>, string> WikiOperationsData()
    {
        var fixture = new RedmineApiUrlsFixture();
        return new TheoryData<string, string, Func<string, string, string>, string>
        {
            { 
                "project1", 
                "page1", 
                (id, page) => fixture.Sut.ProjectWikiPage(id, page),
                "projects/project1/wiki/page1.json" 
            },
            { 
                "project1", 
                "page1", 
                (id, page) => fixture.Sut.ProjectWikiPageCreate(id, page),
                "projects/project1/wiki/page1.json" 
            }
        };
    }

}