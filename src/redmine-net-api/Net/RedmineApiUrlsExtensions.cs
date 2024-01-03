using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;

namespace Redmine.Net.Api.Net;

internal static class RedmineApiUrlsExtensions
{
    public static string MyAccount(this RedmineApiUrls redmineApiUrls)
    {
        return $"my/account.{redmineApiUrls.Format}";
    }

    public static string CurrentUser(this RedmineApiUrls redmineApiUrls)
    {
        return $"{RedmineKeys.CURRENT}.{redmineApiUrls.Format}";
    }

    public static string ProjectNews(this RedmineApiUrls redmineApiUrls, string projectIdentifier)
    {
        if (projectIdentifier.IsNullOrWhiteSpace())
        {
            throw new RedmineException($"Argument '{nameof(projectIdentifier)}' is null or whitespace");
        }

        return $"{RedmineKeys.PROJECT}/{projectIdentifier}/{RedmineKeys.NEWS}.{redmineApiUrls.Format}";
    }

    public static string ProjectMemberships(this RedmineApiUrls redmineApiUrls, string projectIdentifier)
    {
        if (projectIdentifier.IsNullOrWhiteSpace())
        {
            throw new RedmineException($"Argument '{nameof(projectIdentifier)}' is null or whitespace");
        }

        return $"{RedmineKeys.PROJECT}/{projectIdentifier}/{RedmineKeys.MEMBERSHIPS}.{redmineApiUrls.Format}";
    }

    public static string ProjectWikiIndex(this RedmineApiUrls redmineApiUrls, string projectId)
    {
        return $"{RedmineKeys.PROJECTS}/{projectId}/{RedmineKeys.WIKI}/index.{redmineApiUrls.Format}";
    }

    public static string ProjectWikiPage(this RedmineApiUrls redmineApiUrls, string projectId, string wikiPageName)
    {
        return $"{RedmineKeys.PROJECTS}/{projectId}/{RedmineKeys.WIKI}/{wikiPageName}.{redmineApiUrls.Format}";
    }

    public static string ProjectWikiPageVersion(this RedmineApiUrls redmineApiUrls, string projectId, string wikiPageName, string version)
    {
        return $"{RedmineKeys.PROJECTS}/{projectId}/{RedmineKeys.WIKI}/{wikiPageName}/{version}.{redmineApiUrls.Format}";
    }

    public static string ProjectWikiPageCreate(this RedmineApiUrls redmineApiUrls, string projectId, string wikiPageName)
    {
        return $"{RedmineKeys.PROJECTS}/{projectId}/{RedmineKeys.WIKI}/{wikiPageName}.{redmineApiUrls.Format}";
    }

    public static string ProjectWikiPageUpdate(this RedmineApiUrls redmineApiUrls, string projectId, string wikiPageName)
    {
        return $"{RedmineKeys.PROJECTS}/{projectId}/{RedmineKeys.WIKI}/{wikiPageName}.{redmineApiUrls.Format}";
    }

    public static string ProjectWikiPageDelete(this RedmineApiUrls redmineApiUrls, string projectId, string wikiPageName)
    {
        return $"{RedmineKeys.PROJECTS}/{projectId}/{RedmineKeys.WIKI}/{wikiPageName}.{redmineApiUrls.Format}";
    }

    public static string ProjectWikis(this RedmineApiUrls redmineApiUrls, string projectId)
    {
        return ProjectWikiIndex(redmineApiUrls, projectId);
    }

    public static string IssueWatcherAdd(this RedmineApiUrls redmineApiUrls, string issueIdentifier)
    {
        if (issueIdentifier.IsNullOrWhiteSpace())
        {
            throw new RedmineException($"Argument '{nameof(issueIdentifier)}' is null or whitespace");
        }

        return $"{RedmineKeys.ISSUES}/{issueIdentifier}/{RedmineKeys.WATCHERS}.{redmineApiUrls.Format}";
    }

    public static string IssueWatcherRemove(this RedmineApiUrls redmineApiUrls, string issueIdentifier, string userId)
    {
        if (issueIdentifier.IsNullOrWhiteSpace())
        {
            throw new RedmineException($"Argument '{nameof(issueIdentifier)}' is null or whitespace");
        }

        if (userId.IsNullOrWhiteSpace())
        {
            throw new RedmineException($"Argument '{nameof(userId)}' is null or whitespace");
        }

        return $"{RedmineKeys.ISSUES}/{issueIdentifier}/{RedmineKeys.WATCHERS}/{userId}.{redmineApiUrls.Format}";
    }

    public static string GroupUserAdd(this RedmineApiUrls redmineApiUrls, string groupIdentifier)
    {
        if (groupIdentifier.IsNullOrWhiteSpace())
        {
            throw new RedmineException($"Argument '{nameof(groupIdentifier)}' is null or whitespace");
        }

        return $"{RedmineKeys.GROUPS}/{groupIdentifier}/{RedmineKeys.USERS}.{redmineApiUrls.Format}";
    }

    public static string GroupUserRemove(this RedmineApiUrls redmineApiUrls, string groupIdentifier, string userId)
    {
        if (groupIdentifier.IsNullOrWhiteSpace())
        {
            throw new RedmineException($"Argument '{nameof(groupIdentifier)}' is null or whitespace");
        }

        if (userId.IsNullOrWhiteSpace())
        {
            throw new RedmineException($"Argument '{nameof(userId)}' is null or whitespace");
        }

        return $"{RedmineKeys.GROUPS}/{groupIdentifier}/{RedmineKeys.USERS}/{userId}.{redmineApiUrls.Format}";
    }

    public static string AttachmentUpdate(this RedmineApiUrls redmineApiUrls, string issueIdentifier)
    {
        if (issueIdentifier.IsNullOrWhiteSpace())
        {
            throw new RedmineException($"Argument '{nameof(issueIdentifier)}' is null or whitespace");
        }

        return $"{RedmineKeys.ATTACHMENTS}/{RedmineKeys.ISSUES}/{issueIdentifier}.{redmineApiUrls.Format}";
    }

    public static string Uploads(this RedmineApiUrls redmineApiUrls)
    {
        return $"{RedmineKeys.UPLOADS}.{redmineApiUrls.Format}";
    }
}