using System;
using System.Collections.Specialized;
using System.Globalization;
using Redmine.Net.Api.Extensions;

namespace Redmine.Net.Api
{
    /// <summary>
    /// 
    /// </summary>
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
                    switch (_scope)
                    {
                        case SearchScope.All:
                            _internalScope = "all";
                            break;
                        case SearchScope.MyProject:
                            _internalScope = "my_project";
                            break;
                        case SearchScope.SubProjects:
                            _internalScope = "subprojects";
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
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
                    switch (_attachments)
                    {
                        case SearchAttachment.OnlyInAttachment:
                            _internalAttachments = "only";
                            break;

                        case SearchAttachment.InDescription:
                            _internalAttachments = "0";
                            break;
                        case SearchAttachment.InDescriptionAndAttachment:
                            _internalAttachments = "1";
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
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
            AddIfNotNull(sb,"scope",_internalScope);
            AddIfNotNull(sb,"projects",IncludeProjects);
            AddIfNotNull(sb,"open_issues",OpenIssues);
            AddIfNotNull(sb,"messages",IncludeMessages);
            AddIfNotNull(sb,"wiki_pages",IncludeWikiPages);
            AddIfNotNull(sb,"changesets",IncludeChangeSets);
            AddIfNotNull(sb,"documents",IncludeDocuments);
            AddIfNotNull(sb,"news",IncludeNews);
            AddIfNotNull(sb,"issues",IncludeIssues);
            AddIfNotNull(sb,"titles_only",TitlesOnly);
            AddIfNotNull(sb,"all_words", AllWords);
            AddIfNotNull(sb,"attachments", _internalAttachments);
                
            return sb;
        }

        private static void AddIfNotNull(NameValueCollection nameValueCollection, string key, bool? value)
        {
            if (value.HasValue)
            {
                nameValueCollection.Add(key, value.Value.ToString(CultureInfo.InvariantCulture));
            }
        }
        
        private static void AddIfNotNull(NameValueCollection nameValueCollection, string key, string value)
        {
            if (!value.IsNullOrWhiteSpace())
            {
                nameValueCollection.Add(key, value);
            }
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