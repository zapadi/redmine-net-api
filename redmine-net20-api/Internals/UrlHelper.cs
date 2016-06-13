/*
   Copyright 2011 - 2016 Adrian Popescu.

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
using System.Collections.Specialized;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;
using Redmine.Net.Api.Exceptions;

namespace Redmine.Net.Api.Internals
{
    /// <summary>
    /// 
    /// </summary>
    internal static class UrlHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager"></param>
        /// <param name="id"></param>
        /// <param name="obj"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public static string GetUploadUrl<T>(RedmineManager redmineManager, string id, T obj, string projectId = null) where T : class, new()
        {
            var type = typeof(T);

            if (!RedmineManager.Sufixes.ContainsKey(type)) throw new KeyNotFoundException(type.Name);

            return string.Format(RedmineManager.REQUEST_FORMAT, redmineManager.Host, RedmineManager.Sufixes[type], id, redmineManager.MimeFormat.ToString().ToLower());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public static string GetCreateUrl<T>( RedmineManager redmineManager, string ownerId) where T : class, new()
        {
            var type = typeof(T);

            if (!RedmineManager.Sufixes.ContainsKey(type)) throw new KeyNotFoundException(type.Name);

            if (type == typeof(Version) || type == typeof(IssueCategory) || type == typeof(ProjectMembership))
            {
                if (string.IsNullOrEmpty(ownerId)) throw new RedmineException("The owner id(project id) is mandatory!");
                return string.Format(RedmineManager.ENTITY_WITH_PARENT_FORMAT, redmineManager.Host, RedmineKeys.PROJECTS, ownerId, RedmineManager.Sufixes[type], redmineManager.MimeFormat.ToString().ToLower());
            }
            else
                if (type == typeof(IssueRelation))
                {
                    if (string.IsNullOrEmpty(ownerId)) throw new RedmineException("The owner id(issue id) is mandatory!");
                    return string.Format(RedmineManager.ENTITY_WITH_PARENT_FORMAT, redmineManager.Host, RedmineKeys.ISSUES, ownerId, RedmineManager.Sufixes[type], redmineManager.MimeFormat.ToString().ToLower());
                }

            return string.Format(RedmineManager.FORMAT, redmineManager.Host, RedmineManager.Sufixes[type], redmineManager.MimeFormat.ToString().ToLower());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetDeleteUrl<T>( RedmineManager redmineManager, string id) where T : class, new()
        {
            var type = typeof(T);

            if (!RedmineManager.Sufixes.ContainsKey(type)) throw new KeyNotFoundException(type.Name);

            return string.Format(RedmineManager.REQUEST_FORMAT, redmineManager.Host, RedmineManager.Sufixes[type], id, redmineManager.MimeFormat.ToString().ToLower());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetGetUrl<T>( RedmineManager redmineManager, string id) where T : class, new()
        {
            var type = typeof(T);

            if (!RedmineManager.Sufixes.ContainsKey(type)) throw new KeyNotFoundException(type.Name);

            return string.Format(RedmineManager.REQUEST_FORMAT, redmineManager.Host, RedmineManager.Sufixes[type], id, redmineManager.MimeFormat.ToString().ToLower());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string GetListUrl<T>( RedmineManager redmineManager, NameValueCollection parameters) where T : class, new()
        {
            var type = typeof(T);

            if (!RedmineManager.Sufixes.ContainsKey(type)) throw new KeyNotFoundException(type.Name);

            if (type == typeof(Version) || type == typeof(IssueCategory) || type == typeof(ProjectMembership))
            {
                string projectId = parameters.GetParameterValue(RedmineKeys.PROJECT_ID);
                if (string.IsNullOrEmpty(projectId))
                    throw new RedmineException("The project id is mandatory! \nCheck if you have included the parameter project_id to parameters.");

                return string.Format(RedmineManager.ENTITY_WITH_PARENT_FORMAT, redmineManager.Host, RedmineKeys.PROJECTS, projectId, RedmineManager.Sufixes[type], redmineManager.MimeFormat.ToString().ToLower());
            }
            if (type == typeof(IssueRelation))
            {
                string issueId = parameters.GetParameterValue(RedmineKeys.ISSUE_ID);
                if (string.IsNullOrEmpty(issueId))
                    throw new RedmineException("The issue id is mandatory! \nCheck if you have included the parameter issue_id to parameters");
                return string.Format(RedmineManager.ENTITY_WITH_PARENT_FORMAT, redmineManager.Host, RedmineKeys.ISSUES, issueId, RedmineManager.Sufixes[type], redmineManager.MimeFormat.ToString().ToLower());
            }
            return string.Format(RedmineManager.FORMAT, redmineManager.Host, RedmineManager.Sufixes[type], redmineManager.MimeFormat.ToString().ToLower());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public static string GetWikisUrl( RedmineManager redmineManager, string projectId)
        {
            return string.Format(RedmineManager.WIKI_INDEX_FORMAT, redmineManager.Host, projectId, redmineManager.MimeFormat.ToString().ToLower());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectId"></param>
        /// <param name="parameters"></param>
        /// <param name="pageName"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static string GetWikiPageUrl( RedmineManager redmineManager, string projectId, NameValueCollection parameters, string pageName, uint version = 0)
        {
            string uri = version == 0
                ? string.Format(RedmineManager.WIKI_PAGE_FORMAT, redmineManager.Host, projectId, pageName, redmineManager.MimeFormat.ToString().ToLower())
                : string.Format(RedmineManager.WIKI_VERSION_FORMAT, redmineManager.Host, projectId, pageName, version, redmineManager.MimeFormat.ToString().ToLower());
            return uri;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public static string GetAddUserToGroupUrl( RedmineManager redmineManager, int groupId)
        {
            return string.Format(RedmineManager.REQUEST_FORMAT, redmineManager.Host, RedmineManager.Sufixes[typeof(Group)],
                groupId + "/users", redmineManager.MimeFormat.ToString().ToLower());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string GetRemoveUserFromGroupUrl( RedmineManager redmineManager, int groupId, int userId)
        {
            return string.Format(RedmineManager.REQUEST_FORMAT, redmineManager.Host, RedmineManager.Sufixes[typeof(Group)],
                groupId + "/users/" + userId, redmineManager.MimeFormat.ToString().ToLower());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <returns></returns>
        public static string GetUploadFileUrl( RedmineManager redmineManager)
        {
            return string.Format(RedmineManager.FORMAT, redmineManager.Host, RedmineKeys.UPLOADS, redmineManager.MimeFormat.ToString().ToLower());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <returns></returns>
        public static string GetCurrentUserUrl( RedmineManager redmineManager)
        {
            return string.Format(RedmineManager.REQUEST_FORMAT, redmineManager.Host, RedmineManager.Sufixes[typeof(User)], RedmineManager.CURRENT_USER_URI, redmineManager.MimeFormat.ToString().ToLower());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectId"></param>
        /// <param name="pageName"></param>
        /// <returns></returns>
        public static string GetWikiCreateOrUpdaterUrl( RedmineManager redmineManager, string projectId, string pageName)
        {
            return string.Format(RedmineManager.WIKI_PAGE_FORMAT, redmineManager.Host, projectId, pageName,
                redmineManager.MimeFormat.ToString().ToLower());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectId"></param>
        /// <param name="pageName"></param>
        /// <returns></returns>
        public static string GetDeleteWikirUrl( RedmineManager redmineManager, string projectId, string pageName)
        {
            return string.Format(RedmineManager.WIKI_PAGE_FORMAT, redmineManager.Host, projectId, pageName, redmineManager.MimeFormat.ToString().ToLower());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string GetAddWatcherUrl( RedmineManager redmineManager, int issueId, int userId)
        {
            return string.Format(RedmineManager.REQUEST_FORMAT, redmineManager.Host, RedmineManager.Sufixes[typeof(Issue)], issueId + "/watchers", redmineManager.MimeFormat.ToString().ToLower());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="issueId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string GetRemoveWatcherUrl( RedmineManager redmineManager, int issueId, int userId)
        {
            return string.Format(RedmineManager.REQUEST_FORMAT, redmineManager.Host, RedmineManager.Sufixes[typeof(Issue)], issueId + "/watchers/" + userId, redmineManager.MimeFormat.ToString().ToLower());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="issueId"></param>
        /// <returns></returns>
        public static string GetAttachmentUpdateUrl(RedmineManager redmineManager, int issueId)
        {
            return string.Format(RedmineManager.ATTACHMENT_UPDATE_FORMAT, redmineManager.Host, issueId, redmineManager.MimeFormat.ToString().ToLower());
        }
    }
}