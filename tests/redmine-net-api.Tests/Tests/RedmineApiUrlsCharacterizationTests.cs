using System;
using System.Collections.Specialized;
using Redmine.Net.Api;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Types;
using Xunit;
using File = Redmine.Net.Api.Types.File;
using Version = Redmine.Net.Api.Types.Version;

namespace Padi.DotNet.RedmineAPI.Tests.Tests;

public sealed class RedmineApiUrlsCharacterizationTests
{
    private static readonly RedmineApiUrls Xml = new(RedmineConstants.XML);
    private static readonly RedmineApiUrls Json = new(RedmineConstants.JSON);

    public static TheoryData<Type, string> TypeFragments => new()
    {
        { typeof(Attachment), "attachments" },
        { typeof(CustomField), "custom_fields" },
        { typeof(DocumentCategory), "enumerations/document_categories" },
        { typeof(Group), "groups" },
        { typeof(Issue), "issues" },
        { typeof(IssueCategory), "issue_categories" },
        { typeof(IssueCustomField), "custom_fields" },
        { typeof(IssuePriority), "enumerations/issue_priorities" },
        { typeof(IssueRelation), "relations" },
        { typeof(IssueStatus), "issue_statuses" },
        { typeof(Journal), "journals" },
        { typeof(News), "news" },
        { typeof(Project), "projects" },
        { typeof(ProjectMembership), "memberships" },
        { typeof(Query), "queries" },
        { typeof(Role), "roles" },
        { typeof(Search), "search" },
        { typeof(TimeEntry), "time_entries" },
        { typeof(TimeEntryActivity), "enumerations/time_entry_activities" },
        { typeof(Tracker), "trackers" },
        { typeof(User), "users" },
        { typeof(Version), "versions" },
        { typeof(Watcher), "watchers" },
    };

    [Theory]
    [MemberData(nameof(TypeFragments))]
    public void Get_Update_Delete_Fragments_Use_Type_Fragment_And_Id(Type type, string fragment)
    {
        Assert.Equal($"{fragment}/my-id_1.xml", Xml.GetFragment(type, "my-id_1"));
        Assert.Equal($"{fragment}/my-id_1.xml", Xml.UpdateFragment(type, "my-id_1"));
        Assert.Equal($"{fragment}/my-id_1.xml", Xml.DeleteFragment(type, "my-id_1"));
        Assert.Equal($"{fragment}/1.json", Json.GetFragment(type, "1"));
    }

    public static TheoryData<Type, string, string> CreateFragments => new()
    {
        { typeof(Issue), null, "issues.xml" },
        { typeof(Project), "ignored", "projects.xml" },
        { typeof(User), null, "users.xml" },
        { typeof(Version), "p-1", "projects/p-1/versions.xml" },
        { typeof(IssueCategory), "p-1", "projects/p-1/issue_categories.xml" },
        { typeof(ProjectMembership), "p-1", "projects/p-1/memberships.xml" },
        { typeof(IssueRelation), "12", "issues/12/relations.xml" },
        { typeof(File), "p-1", "projects/p-1/files.xml" },
        { typeof(Upload), null, "uploads.xml" },
        { typeof(Upload), "a.txt", "uploads.xml?filename=a.txt" },
        { typeof(Attachment), "12", "attachments/issues/12.xml" },
        { typeof(Attachments), "12", "attachments/issues/12.xml" },
    };

    [Theory]
    [MemberData(nameof(CreateFragments))]
    public void CreateEntityFragment_Characterization(Type type, string ownerId, string expected)
    {
        Assert.Equal(expected, Xml.CreateEntityFragment(type, ownerId));
    }

    public static TheoryData<Type, string, string> ListFragments => new()
    {
        { typeof(Issue), null, "issues.xml" },
        { typeof(IssueStatus), null, "issue_statuses.xml" },
        { typeof(Attachment), "12", "attachments.xml" },
        { typeof(Upload), null, null },
        { typeof(Version), "p-1", "projects/p-1/versions.xml" },
        { typeof(IssueCategory), "p-1", "projects/p-1/issue_categories.xml" },
        { typeof(ProjectMembership), "p-1", "projects/p-1/memberships.xml" },
        { typeof(IssueRelation), "12", "issues/12/relations.xml" },
        { typeof(File), "p-1", "projects/p-1/files.xml" },
    };

    [Theory]
    [MemberData(nameof(ListFragments))]
    public void GetListFragment_Characterization(Type type, string ownerId, string expected)
    {
        if (expected == null)
        {
            Assert.Throws<Redmine.Net.Api.Exceptions.RedmineException>(() => Xml.GetListFragment(type, ownerId));
            return;
        }

        Assert.Equal(expected, Xml.GetListFragment(type, ownerId));
    }

    [Fact]
    public void Owner_Id_Is_Read_From_Query_String_Project_Id_First()
    {
        var options = new RequestOptions { QueryString = new NameValueCollection { { RedmineKeys.ISSUE_ID, "12" }, { RedmineKeys.PROJECT_ID, "p-1" } } };

        Assert.Equal("projects/p-1/versions.xml", Xml.GetListFragment(typeof(Version), options));
        Assert.Equal("projects/p-1/versions.xml", Xml.CreateEntityFragment(typeof(Version), options));
        Assert.Equal("issues/p-1/relations.xml", Xml.GetListFragment(typeof(IssueRelation), options));
    }

    [Fact]
    public void Owner_Fragments_Characterization()
    {
        Assert.Equal("projects/p-1/files.xml", Xml.ProjectFilesFragment("p-1"));
        Assert.Equal("attachments/issues/12.xml", Xml.IssueAttachmentFragment("12"));
        Assert.Equal("projects/p-1/news.xml", Xml.ProjectParentFragment("p-1", "news"));
        Assert.Equal("issues/12/watchers.xml", Xml.IssueParentFragment("12", "watchers"));
        Assert.Equal("attachments/issues/12.xml", Xml.PatchFragment(typeof(Attachment), "12"));
        Assert.Equal("attachments/issues/12.xml", Xml.PatchFragment(typeof(Attachments), "12"));
        Assert.Equal("uploads.xml", Xml.UploadFragment());
        Assert.Equal("uploads.xml?filename=a%20b%26c.txt", Xml.UploadFragment("a b&c.txt"));
    }

    [Fact]
    public void Extension_Fragments_Characterization()
    {
        Assert.Equal("my/account.xml", Xml.MyAccount());
        Assert.Equal("users/current.xml", Xml.CurrentUser());
        Assert.Equal("projects/p-1/close.xml", Xml.ProjectClose("p-1"));
        Assert.Equal("projects/p-1/reopen.xml", Xml.ProjectReopen("p-1"));
        Assert.Equal("projects/p-1/archive.xml", Xml.ProjectArchive("p-1"));
        Assert.Equal("projects/p-1/unarchive.xml", Xml.ProjectUnarchive("p-1"));
        Assert.Equal("projects/p-1/repository/repo_1/revisions/abc123/issues.xml", Xml.ProjectRepositoryAddRelatedIssue("p-1", "repo_1", "abc123"));
        Assert.Equal("projects/p-1/repository/repo_1/revisions/abc123/issues/12.xml", Xml.ProjectRepositoryRemoveRelatedIssue("p-1", "repo_1", "abc123", "12"));
        Assert.Equal("projects/p-1/news.xml", Xml.ProjectNews("p-1"));
        Assert.Equal("projects/p-1/memberships.xml", Xml.ProjectMemberships("p-1"));
        Assert.Equal("projects/p-1/issue_categories.xml", Xml.ProjectIssueCategories("p-1"));
        Assert.Equal("projects/p-1/wiki/index.xml", Xml.ProjectWikiIndex("p-1"));
        Assert.Equal("projects/p-1/wiki/index.xml", Xml.ProjectWikis("p-1"));
        Assert.Equal("projects/p-1/wiki/Page_1.xml", Xml.ProjectWikiPage("p-1", "Page_1"));
        Assert.Equal("projects/p-1/wiki/Page_1/3.xml", Xml.ProjectWikiPageVersion("p-1", "Page_1", "3"));
        Assert.Equal("projects/p-1/wiki/Page_1.xml", Xml.ProjectWikiPageCreate("p-1", "Page_1"));
        Assert.Equal("projects/p-1/wiki/Page_1.xml", Xml.ProjectWikiPageUpdate("p-1", "Page_1"));
        Assert.Equal("projects/p-1/wiki/Page_1.xml", Xml.ProjectWikiPageDelete("p-1", "Page_1"));
        Assert.Equal("issues/12/watchers.xml", Xml.IssueWatcherAdd("12"));
        Assert.Equal("issues/12/watchers/5.xml", Xml.IssueWatcherRemove("12", "5"));
        Assert.Equal("groups/3/users.xml", Xml.GroupUserAdd("3"));
        Assert.Equal("groups/3/users/5.xml", Xml.GroupUserRemove("3", "5"));
        Assert.Equal("attachments/issues/12.xml", Xml.AttachmentUpdate("12"));
        Assert.Equal("uploads.xml", Xml.Uploads());
        Assert.Equal("projects/p.1~x/wiki/Page.v2~x.json", Json.ProjectWikiPage("p.1~x", "Page.v2~x"));
    }
}
