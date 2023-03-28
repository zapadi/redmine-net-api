using System;
using System.Collections.Specialized;
using System.Globalization;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Serialization;
using Redmine.Net.Api.Types;
using Version = Redmine.Net.Api.Types.Version;

namespace Redmine.Net.Api.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class RedmineManagerExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectIdentifier"></param>
        /// <param name="nameValueCollection"></param>
        /// <returns></returns>
        public static PagedResults<News> GetProjectNews(this RedmineManager redmineManager, string projectIdentifier, NameValueCollection nameValueCollection)
        {
            if (projectIdentifier.IsNullOrWhiteSpace())
            {
                throw new RedmineException("Argument 'projectIdentifier' is null");
            }
            
            return WebApiHelper.ExecuteDownloadList<News>(redmineManager, Uri.EscapeUriString($"{redmineManager.Host}/project/{projectIdentifier}/news.{redmineManager.Format}"), nameValueCollection);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="redmineManager"></param>
        /// <param name="projectIdentifier"></param>
        /// <param name="news"></param>
        /// <returns></returns>
        /// <exception cref="RedmineException"></exception>
        public static News AddProjectNews(this RedmineManager redmineManager, string projectIdentifier, News news)
        {
            if (projectIdentifier.IsNullOrWhiteSpace())
            {
                throw new RedmineException("Argument 'projectIdentifier' is null");
            }
            
            if (news == null)
            {
                throw new RedmineException("Argument news is null");
            }

            if (news.Title.IsNullOrWhiteSpace())
            {
                throw new RedmineException("Title cannot be blank");
            }
            
            var data = redmineManager.Serializer.Serialize(news);
            
            return WebApiHelper.ExecuteUpload<News>(redmineManager, Uri.EscapeUriString($"{redmineManager.Host}/project/{projectIdentifier}/news.{redmineManager.Format}"), HttpVerbs.POST, data);
        }
        
        public static PagedResults<ProjectMembership> GetProjectMemberships(this RedmineManager redmineManager, string projectIdentifier, NameValueCollection nameValueCollection)
        {
            if (projectIdentifier.IsNullOrWhiteSpace())
            {
                throw new RedmineException($"Argument '{nameof(projectIdentifier)}' is null");
            }
            
            return WebApiHelper.ExecuteDownloadList<ProjectMembership>(redmineManager, Uri.EscapeUriString($"{redmineManager.Host}/project/{projectIdentifier}/memberships.{redmineManager.Format}"), nameValueCollection);
        }
    }
}