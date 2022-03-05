/*
   Copyright 2011 - 2022 Adrian Popescu

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

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;
using File = Redmine.Net.Api.Types.File;
using Version = Redmine.Net.Api.Types.Version;

namespace Redmine.Net.Api.Internals
{
    /// <summary>
    /// </summary>
    internal static class UrlHelper
    {
        /// <summary>
        /// </summary>
        private const string REQUEST_FORMAT = "{0}/{1}/{2}.{3}";

        /// <summary>
        /// </summary>
        private const string FORMAT = "{0}/{1}.{2}";

        /// <summary>
        /// </summary>
        private const string WIKI_INDEX_FORMAT = "{0}/projects/{1}/wiki/index.{2}";

        /// <summary>
        /// </summary>
        private const string WIKI_PAGE_FORMAT = "{0}/projects/{1}/wiki/{2}.{3}";

        /// <summary>
        /// </summary>
        private const string WIKI_VERSION_FORMAT = "{0}/projects/{1}/wiki/{2}/{3}.{4}";

        /// <summary>
        /// </summary>
        private const string ENTITY_WITH_PARENT_FORMAT = "{0}/{1}/{2}/{3}.{4}";

        /// <summary>
        /// </summary>
        private const string ATTACHMENT_UPDATE_FORMAT = "{0}/attachments/issues/{1}.{2}";

        /// <summary>
        /// 
        /// </summary>
        private const string FILE_URL_FORMAT = "{0}/projects/{1}/files.{2}";

        private const string MY_ACCOUNT_FORMAT = "{0}/my/account.{1}";
    

    /// <summary>
        /// </summary>
        private const string CURRENT_USER_URI = "current";
        /// <summary>
        ///     Gets the upload URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        public static string GetUploadUrl<T>(RedmineManager redmineManager, string id)
            where T : class, new()
        {
            var type = typeof(T);

            if (!RedmineManager.Suffixes.ContainsKey(type)) throw new KeyNotFoundException(type.Name);

            return string.Format(CultureInfo.InvariantCulture,REQUEST_FORMAT, redmineManager.Host, RedmineManager.Suffixes[type], id,
                redmineManager.Format);
        }

        /// <summary>
        ///     Gets the create URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="ownerId">The owner identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        /// <exception cref="Redmine.Net.Api.Exceptions.RedmineException">
        ///     The owner id(project id) is mandatory!
        ///     or
        ///     The owner id(issue id) is mandatory!
        /// </exception>
        public static string GetCreateUrl<T>(RedmineManager redmineManager, string ownerId) where T : class, new()
        {
            var type = typeof(T);

            if (!RedmineManager.Suffixes.ContainsKey(type)) throw new KeyNotFoundException(type.Name);

            if (type == typeof(Version) || type == typeof(IssueCategory) || type == typeof(ProjectMembership))
            {
                if (string.IsNullOrEmpty(ownerId)) throw new RedmineException("The owner id(project id) is mandatory!");
                return string.Format(CultureInfo.InvariantCulture,ENTITY_WITH_PARENT_FORMAT, redmineManager.Host, RedmineKeys.PROJECTS,
                    ownerId, RedmineManager.Suffixes[type], redmineManager.Format);
            }
            if (type == typeof(IssueRelation))
            {
                if (string.IsNullOrEmpty(ownerId)) throw new RedmineException("The owner id(issue id) is mandatory!");
                return string.Format(CultureInfo.InvariantCulture,ENTITY_WITH_PARENT_FORMAT, redmineManager.Host, RedmineKeys.ISSUES,
                    ownerId, RedmineManager.Suffixes[type], redmineManager.Format);
            }

            if (type == typeof(File))
            {
                if (string.IsNullOrEmpty(ownerId))
                {
                    throw new RedmineException("The owner id(project id) is mandatory!");
                }
                return string.Format(CultureInfo.InvariantCulture,FILE_URL_FORMAT, redmineManager.Host, ownerId, redmineManager.Format);
            }

            return string.Format(CultureInfo.InvariantCulture,FORMAT, redmineManager.Host, RedmineManager.Suffixes[type],
                redmineManager.Format);
        }

        /// <summary>
        /// Gets the delete URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        public static string GetDeleteUrl<T>(RedmineManager redmineManager, string id) where T : class, new()
        {
            var type = typeof(T);

            if (!RedmineManager.Suffixes.ContainsKey(type)) throw new KeyNotFoundException(type.Name);

            return string.Format(CultureInfo.InvariantCulture,REQUEST_FORMAT, redmineManager.Host, RedmineManager.Suffixes[type], id,
                redmineManager.Format);
        }

        /// <summary>
        /// Gets the get URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        public static string GetGetUrl<T>(RedmineManager redmineManager, string id) where T : class, new()
        {
            var type = typeof(T);

            if (!RedmineManager.Suffixes.ContainsKey(type)) throw new KeyNotFoundException(type.Name);

            return string.Format(CultureInfo.InvariantCulture,REQUEST_FORMAT, redmineManager.Host, RedmineManager.Suffixes[type], id,
                redmineManager.Format);
        }

        /// <summary>
        /// Gets the list URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        /// <exception cref="Redmine.Net.Api.Exceptions.RedmineException">
        /// The project id is mandatory! \nCheck if you have included the parameter project_id to parameters.
        /// or
        /// The issue id is mandatory! \nCheck if you have included the parameter issue_id to parameters
        /// </exception>
        public static string GetListUrl<T>(RedmineManager redmineManager, NameValueCollection parameters)
            where T : class, new()
        {
            var type = typeof(T);

            if (!RedmineManager.Suffixes.ContainsKey(type)) throw new KeyNotFoundException(type.Name);

            if (type == typeof(Version) || type == typeof(IssueCategory) || type == typeof(ProjectMembership))
            {
                var projectId = parameters.GetParameterValue(RedmineKeys.PROJECT_ID);
                if (string.IsNullOrEmpty(projectId))
                    throw new RedmineException("The project id is mandatory! \nCheck if you have included the parameter project_id to parameters.");

                return string.Format(CultureInfo.InvariantCulture,ENTITY_WITH_PARENT_FORMAT, redmineManager.Host, RedmineKeys.PROJECTS,
                    projectId, RedmineManager.Suffixes[type], redmineManager.Format);
            }
            if (type == typeof(IssueRelation))
            {
                var issueId = parameters.GetParameterValue(RedmineKeys.ISSUE_ID);
                if (string.IsNullOrEmpty(issueId))
                    throw new RedmineException("The issue id is mandatory! \nCheck if you have included the parameter issue_id to parameters");

                return string.Format(CultureInfo.InvariantCulture,ENTITY_WITH_PARENT_FORMAT, redmineManager.Host, RedmineKeys.ISSUES,
                    issueId, RedmineManager.Suffixes[type], redmineManager.Format);
            }

            if (type == typeof(File))
            {
                var projectId = parameters.GetParameterValue(RedmineKeys.PROJECT_ID);
                if (string.IsNullOrEmpty(projectId))
                {
                    throw new RedmineException("The project id is mandatory! \nCheck if you have included the parameter project_id to parameters.");
                }
                return string.Format(CultureInfo.InvariantCulture,FILE_URL_FORMAT, redmineManager.Host, projectId, redmineManager.Format);
            }
            
            return string.Format(CultureInfo.InvariantCulture,FORMAT, redmineManager.Host, RedmineManager.Suffixes[type],
                redmineManager.Format);
        }

        /// <summary>
        /// Gets the wikis URL.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <returns></returns>
        public static string GetWikisUrl(RedmineManager redmineManager, string projectId)
        {
            return string.Format(CultureInfo.InvariantCulture,WIKI_INDEX_FORMAT, redmineManager.Host, projectId,
                redmineManager.Format);
        }

        /// <summary>
        /// Gets the wiki page URL.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        public static string GetWikiPageUrl(RedmineManager redmineManager, string projectId, string pageName, uint version = 0)
        {
            var uri = version == 0
                ? string.Format(CultureInfo.InvariantCulture,WIKI_PAGE_FORMAT, redmineManager.Host, projectId, pageName,
                    redmineManager.Format)
                : string.Format(CultureInfo.InvariantCulture,WIKI_VERSION_FORMAT, redmineManager.Host, projectId, pageName, version.ToString(CultureInfo.InvariantCulture),
                    redmineManager.Format);
            return uri;
        }

        /// <summary>
        /// Gets the add user to group URL.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <returns></returns>
        public static string GetAddUserToGroupUrl(RedmineManager redmineManager, int groupId)
        {
            return string.Format(CultureInfo.InvariantCulture,REQUEST_FORMAT, redmineManager.Host,
                RedmineManager.Suffixes[typeof(Group)],
                $"{groupId.ToString(CultureInfo.InvariantCulture)}/users", redmineManager.Format);
        }

        /// <summary>
        /// Gets the remove user from group URL.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public static string GetRemoveUserFromGroupUrl(RedmineManager redmineManager, int groupId, int userId)
        {
            return string.Format(CultureInfo.InvariantCulture,REQUEST_FORMAT, redmineManager.Host,
                RedmineManager.Suffixes[typeof(Group)],
                $"{groupId.ToString(CultureInfo.InvariantCulture)}/users/{userId.ToString(CultureInfo.InvariantCulture)}", redmineManager.Format);
        }

        /// <summary>
        /// Gets the upload file URL.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <returns></returns>
        public static string GetUploadFileUrl(RedmineManager redmineManager)
        {
            return string.Format(CultureInfo.InvariantCulture,FORMAT, redmineManager.Host, RedmineKeys.UPLOADS,
                redmineManager.Format);
        }

        /// <summary>
        /// Gets the current user URL.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <returns></returns>
        public static string GetCurrentUserUrl(RedmineManager redmineManager)
        {
            return string.Format(CultureInfo.InvariantCulture,REQUEST_FORMAT, redmineManager.Host,
                RedmineManager.Suffixes[typeof(User)], CURRENT_USER_URI,
                redmineManager.Format);
        }

        public static string GetMyAccountUrl(RedmineManager redmineManager)
        {
            return string.Format(CultureInfo.InvariantCulture,MY_ACCOUNT_FORMAT, redmineManager.Host, redmineManager.Format);
        }

        /// <summary>
        /// Gets the wiki create or updater URL.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <returns></returns>
        public static string GetWikiCreateOrUpdaterUrl(RedmineManager redmineManager, string projectId, string pageName)
        {
            return string.Format(CultureInfo.InvariantCulture,WIKI_PAGE_FORMAT, redmineManager.Host, projectId, pageName,
                redmineManager.Format);
        }

        /// <summary>
        /// Gets the delete wiki URL.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="projectId">The project identifier.</param>
        /// <param name="pageName">Name of the page.</param>
        /// <returns></returns>
        public static string GetDeleteWikiUrl(RedmineManager redmineManager, string projectId, string pageName)
        {
            return string.Format(CultureInfo.InvariantCulture,WIKI_PAGE_FORMAT, redmineManager.Host, projectId, pageName,
                redmineManager.Format);
        }

        /// <summary>
        /// Gets the add watcher URL.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="issueId">The issue identifier.</param>
        /// <returns></returns>
        public static string GetAddWatcherUrl(RedmineManager redmineManager, int issueId)
        {
            return string.Format(CultureInfo.InvariantCulture,REQUEST_FORMAT, redmineManager.Host,
                RedmineManager.Suffixes[typeof(Issue)], $"{issueId.ToString(CultureInfo.InvariantCulture)}/watchers",
                redmineManager.Format);
        }

        /// <summary>
        /// Gets the remove watcher URL.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="issueId">The issue identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public static string GetRemoveWatcherUrl(RedmineManager redmineManager, int issueId, int userId)
        {
            return string.Format(CultureInfo.InvariantCulture,REQUEST_FORMAT, redmineManager.Host,
                RedmineManager.Suffixes[typeof(Issue)], $"{issueId.ToString(CultureInfo.InvariantCulture)}/watchers/{userId.ToString(CultureInfo.InvariantCulture)}",
                redmineManager.Format);
        }

        /// <summary>
        /// Gets the attachment update URL.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="issueId">The issue identifier.</param>
        /// <returns></returns>
        public static string GetAttachmentUpdateUrl(RedmineManager redmineManager, int issueId)
        {
            return string.Format(CultureInfo.InvariantCulture,
                ATTACHMENT_UPDATE_FORMAT,
                redmineManager.Host,
                issueId.ToString(CultureInfo.InvariantCulture),
                redmineManager.Format);
        }
    }
}