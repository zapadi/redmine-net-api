/*
   Copyright 2011 - 2025 Adrian Popescu

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System.Collections.Generic;
using System.Text;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;

namespace Redmine.Net.Api.Net;

internal static class RedmineApiUrlsExtensions
{
    public static string MyAccount(this RedmineApiUrls redmineApiUrls)
    {
        return $"{RedmineKeys.MY}/{RedmineKeys.ACCOUNT}.{redmineApiUrls.Format}";
    }

    public static string CurrentUser(this RedmineApiUrls redmineApiUrls)
    {
        return $"{RedmineKeys.USERS}/{RedmineKeys.CURRENT}.{redmineApiUrls.Format}";
    }

    public static string ProjectClose(this RedmineApiUrls redmineApiUrls, string projectIdentifier)
    {
        return $"{RedmineKeys.PROJECTS}/{RedmineApiUrls.Escape(projectIdentifier)}/{RedmineKeys.CLOSE}.{redmineApiUrls.Format}";
    }
    
    public static string ProjectReopen(this RedmineApiUrls redmineApiUrls, string projectIdentifier)
    {
        return $"{RedmineKeys.PROJECTS}/{RedmineApiUrls.Escape(projectIdentifier)}/{RedmineKeys.REOPEN}.{redmineApiUrls.Format}";
    }
    
    public static string ProjectArchive(this RedmineApiUrls redmineApiUrls, string projectIdentifier)
    {
        return $"{RedmineKeys.PROJECTS}/{RedmineApiUrls.Escape(projectIdentifier)}/{RedmineKeys.ARCHIVE}.{redmineApiUrls.Format}";
    }
    
    public static string ProjectUnarchive(this RedmineApiUrls redmineApiUrls, string projectIdentifier)
    {
        return $"{RedmineKeys.PROJECTS}/{RedmineApiUrls.Escape(projectIdentifier)}/{RedmineKeys.UNARCHIVE}.{redmineApiUrls.Format}";
    }
    
    public static string ProjectRepositoryAddRelatedIssue(this RedmineApiUrls redmineApiUrls, string projectIdentifier, string repositoryIdentifier, string revision)
    {
        return $"{RedmineKeys.PROJECTS}/{RedmineApiUrls.Escape(projectIdentifier)}/{RedmineKeys.REPOSITORY}/{RedmineApiUrls.Escape(repositoryIdentifier)}/{RedmineKeys.REVISIONS}/{RedmineApiUrls.Escape(revision)}/{RedmineKeys.ISSUES}.{redmineApiUrls.Format}";
    }
    
    public static string ProjectRepositoryRemoveRelatedIssue(this RedmineApiUrls redmineApiUrls, string projectIdentifier, string repositoryIdentifier, string revision, string issueIdentifier)
    {
        return $"{RedmineKeys.PROJECTS}/{RedmineApiUrls.Escape(projectIdentifier)}/{RedmineKeys.REPOSITORY}/{RedmineApiUrls.Escape(repositoryIdentifier)}/{RedmineKeys.REVISIONS}/{RedmineApiUrls.Escape(revision)}/{RedmineKeys.ISSUES}/{RedmineApiUrls.Escape(issueIdentifier)}.{redmineApiUrls.Format}";
    }
    
    public static string ProjectNews(this RedmineApiUrls redmineApiUrls, string projectIdentifier)
    {
        if (projectIdentifier.IsNullOrWhiteSpace())
        {
            throw new RedmineException($"Argument '{nameof(projectIdentifier)}' is null or whitespace");
        }

        return $"{RedmineKeys.PROJECTS}/{RedmineApiUrls.Escape(projectIdentifier)}/{RedmineKeys.NEWS}.{redmineApiUrls.Format}";
    }

    public static string ProjectMemberships(this RedmineApiUrls redmineApiUrls, string projectIdentifier)
    {
        if (projectIdentifier.IsNullOrWhiteSpace())
        {
            throw new RedmineException($"Argument '{nameof(projectIdentifier)}' is null or whitespace");
        }

        return $"{RedmineKeys.PROJECTS}/{RedmineApiUrls.Escape(projectIdentifier)}/{RedmineKeys.MEMBERSHIPS}.{redmineApiUrls.Format}";
    }
    
    public static string ProjectIssueCategories(this RedmineApiUrls redmineApiUrls, string projectIdentifier)
    {
        return $"{RedmineKeys.PROJECTS}/{RedmineApiUrls.Escape(projectIdentifier)}/{RedmineKeys.ISSUE_CATEGORIES}.{redmineApiUrls.Format}";
    }

    public static string ProjectWikiIndex(this RedmineApiUrls redmineApiUrls, string projectId)
    {
        return $"{RedmineKeys.PROJECTS}/{RedmineApiUrls.Escape(projectId)}/{RedmineKeys.WIKI}/{RedmineKeys.INDEX}.{redmineApiUrls.Format}";
    }

    public static string ProjectWikiPage(this RedmineApiUrls redmineApiUrls, string projectId, string wikiPageName)
    {
        return $"{RedmineKeys.PROJECTS}/{RedmineApiUrls.Escape(projectId)}/{RedmineKeys.WIKI}/{RedmineApiUrls.Escape(wikiPageName)}.{redmineApiUrls.Format}";
    }

    public static string ProjectWikiPageVersion(this RedmineApiUrls redmineApiUrls, string projectId, string wikiPageName, string version)
    {
        return $"{RedmineKeys.PROJECTS}/{RedmineApiUrls.Escape(projectId)}/{RedmineKeys.WIKI}/{RedmineApiUrls.Escape(wikiPageName)}/{RedmineApiUrls.Escape(version)}.{redmineApiUrls.Format}";
    }

    public static string ProjectWikiPageCreate(this RedmineApiUrls redmineApiUrls, string projectId, string wikiPageName)
    {
        return $"{RedmineKeys.PROJECTS}/{RedmineApiUrls.Escape(projectId)}/{RedmineKeys.WIKI}/{RedmineApiUrls.Escape(wikiPageName)}.{redmineApiUrls.Format}";
    }

    public static string ProjectWikiPageUpdate(this RedmineApiUrls redmineApiUrls, string projectId, string wikiPageName)
    {
        return $"{RedmineKeys.PROJECTS}/{RedmineApiUrls.Escape(projectId)}/{RedmineKeys.WIKI}/{RedmineApiUrls.Escape(wikiPageName)}.{redmineApiUrls.Format}";
    }

    public static string ProjectWikiPageDelete(this RedmineApiUrls redmineApiUrls, string projectId, string wikiPageName)
    {
        return $"{RedmineKeys.PROJECTS}/{RedmineApiUrls.Escape(projectId)}/{RedmineKeys.WIKI}/{RedmineApiUrls.Escape(wikiPageName)}.{redmineApiUrls.Format}";
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

        return $"{RedmineKeys.ISSUES}/{RedmineApiUrls.Escape(issueIdentifier)}/{RedmineKeys.WATCHERS}.{redmineApiUrls.Format}";
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

        return $"{RedmineKeys.ISSUES}/{RedmineApiUrls.Escape(issueIdentifier)}/{RedmineKeys.WATCHERS}/{RedmineApiUrls.Escape(userId)}.{redmineApiUrls.Format}";
    }

    public static string GroupUserAdd(this RedmineApiUrls redmineApiUrls, string groupIdentifier)
    {
        if (groupIdentifier.IsNullOrWhiteSpace())
        {
            throw new RedmineException($"Argument '{nameof(groupIdentifier)}' is null or whitespace");
        }

        return $"{RedmineKeys.GROUPS}/{RedmineApiUrls.Escape(groupIdentifier)}/{RedmineKeys.USERS}.{redmineApiUrls.Format}";
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

        return $"{RedmineKeys.GROUPS}/{RedmineApiUrls.Escape(groupIdentifier)}/{RedmineKeys.USERS}/{RedmineApiUrls.Escape(userId)}.{redmineApiUrls.Format}";
    }

    public static string GroupUsersRemove(this RedmineApiUrls redmineApiUrls, string groupIdentifier, IEnumerable<int> userIds)
    {
        var uri = new StringBuilder(GroupUserAdd(redmineApiUrls, groupIdentifier));
        var separator = '?';

        foreach (var userId in userIds)
        {
            uri.Append(separator).Append(RedmineKeys.USER_IDS).Append("[]=").Append(userId.ToInvariantString());
            separator = '&';
        }

        return uri.ToString();
    }

    public static string AttachmentUpdate(this RedmineApiUrls redmineApiUrls, string issueIdentifier)
    {
        if (issueIdentifier.IsNullOrWhiteSpace())
        {
            throw new RedmineException($"Argument '{nameof(issueIdentifier)}' is null or whitespace");
        }

        return $"{RedmineKeys.ATTACHMENTS}/{RedmineKeys.ISSUES}/{RedmineApiUrls.Escape(issueIdentifier)}.{redmineApiUrls.Format}";
    }

    public static string Uploads(this RedmineApiUrls redmineApiUrls)
    {
        return $"{RedmineKeys.UPLOADS}.{redmineApiUrls.Format}";
    }
}