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

using System;
using System.Collections.Specialized;
using Redmine.Net.Api.Extensions;

namespace Redmine.Net.Api
{
    /// <summary>
    /// 
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class SearchFilterBuilder
    {
        /// <summary>
        /// search scope condition
        /// </summary>
        /// <returns></returns>
        public SearchScope? Scope
        {
            get => _scope;
            set
            {
                _scope = value;
                if (_scope != null)
                {
                    _internalScope = _scope switch
                    {
                        SearchScope.All => "all",
                        SearchScope.MyProject => "my_project",
                        SearchScope.SubProjects => "subprojects",
                        _ => throw new ArgumentOutOfRangeException(nameof(value))
                    };
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool? AllWords { get; set; } 
            
        /// <summary>
        /// 
        /// </summary>
        public bool? TitlesOnly { get; set; }
            
        /// <summary>
        /// 
        /// </summary>
        public bool? IncludeIssues{ get; set; }
            
        /// <summary>
        /// 
        /// </summary>
        public bool? IncludeNews{ get; set; }
            
        /// <summary>
        /// 
        /// </summary>
        public bool? IncludeDocuments{ get; set; }
            
        /// <summary>
        /// 
        /// </summary>
        public bool? IncludeChangeSets{ get; set; }
            
        /// <summary>
        /// 
        /// </summary>
        public bool? IncludeWikiPages{ get; set; }
            
        /// <summary>
        /// 
        /// </summary>
        public bool? IncludeMessages{ get; set; }
            
        /// <summary>
        /// 
        /// </summary>
        public bool? IncludeProjects{ get; set; }
            
        /// <summary>
        /// filtered by open issues.
        /// </summary>
        public bool? OpenIssues{ get; set; }

        /// <summary>
        /// </summary>
        public SearchAttachment? Attachments
        {
            get => _attachments;
            set
            {
                _attachments = value;
                if (_attachments != null)
                {
                    _internalAttachments = _attachments switch
                    {
                        SearchAttachment.OnlyInAttachment => "only",
                        SearchAttachment.InDescription => "0",
                        SearchAttachment.InDescriptionAndAttachment => "1",
                        _ => throw new ArgumentOutOfRangeException(nameof(Attachments))
                    };
                }
            }
        }

        private string _internalScope;
        private string _internalAttachments;
        private SearchAttachment? _attachments;
        private SearchScope? _scope;

        /// <summary>
        /// 
        /// </summary>
        public NameValueCollection Build(NameValueCollection sb)
        {
            sb.AddIfNotNull(RedmineKeys.SCOPE,_internalScope);
            sb.AddIfNotNull(RedmineKeys.PROJECTS, IncludeProjects);
            sb.AddIfNotNull(RedmineKeys.OPEN_ISSUES, OpenIssues);
            sb.AddIfNotNull(RedmineKeys.MESSAGES, IncludeMessages);
            sb.AddIfNotNull(RedmineKeys.WIKI_PAGES, IncludeWikiPages);
            sb.AddIfNotNull(RedmineKeys.CHANGE_SETS, IncludeChangeSets);
            sb.AddIfNotNull(RedmineKeys.DOCUMENTS, IncludeDocuments);
            sb.AddIfNotNull(RedmineKeys.NEWS, IncludeNews);
            sb.AddIfNotNull(RedmineKeys.ISSUES, IncludeIssues);
            sb.AddIfNotNull(RedmineKeys.TITLES_ONLY, TitlesOnly);
            sb.AddIfNotNull(RedmineKeys.ALL_WORDS, AllWords);
            sb.AddIfNotNull(RedmineKeys.ATTACHMENTS, _internalAttachments);
                
            return sb;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum SearchScope
    {
        /// <summary>
        /// all projects
        /// </summary>
        All,
        /// <summary>
        /// assigned projects
        /// </summary>
        MyProject,
        /// <summary>
        /// include subproject
        /// </summary>
        SubProjects
    }

    /// <summary>
    /// 
    /// </summary>
    public enum SearchAttachment
    {
        /// <summary>
        /// search only in description
        /// </summary>
        InDescription = 0,
        /// <summary>
        /// search by description and attachment
        /// </summary>
        InDescriptionAndAttachment,
        /// <summary>
        /// search only in attachment
        /// </summary>
        OnlyInAttachment
    }
}