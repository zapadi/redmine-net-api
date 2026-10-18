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
using System.Collections.Generic;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;
using Version = Redmine.Net.Api.Types.Version;

namespace Redmine.Net.Api.Net
{
    internal sealed class RedmineApiUrls
    {
        public string Format { get; init; }

        private enum UrlOwner
        {
            None,
            Project,
            Issue,
        }

        private sealed class TypeUrl(string fragment, UrlOwner owner)
        {
            public string Fragment { get; } = fragment;
            public UrlOwner Owner { get; } = owner;
        }

        private static readonly Dictionary<Type, TypeUrl> TypeUrls = new Dictionary<Type, TypeUrl>()
        {
            {typeof(Attachment), new TypeUrl(RedmineKeys.ATTACHMENTS, UrlOwner.None)},
            {typeof(CustomField), new TypeUrl(RedmineKeys.CUSTOM_FIELDS, UrlOwner.None)},
            {typeof(DocumentCategory), new TypeUrl(RedmineKeys.ENUMERATION_DOCUMENT_CATEGORIES, UrlOwner.None)},
            {typeof(Group), new TypeUrl(RedmineKeys.GROUPS, UrlOwner.None)},
            {typeof(Issue), new TypeUrl(RedmineKeys.ISSUES, UrlOwner.None)},
            {typeof(IssueCategory), new TypeUrl(RedmineKeys.ISSUE_CATEGORIES, UrlOwner.Project)},
            {typeof(IssueCustomField), new TypeUrl(RedmineKeys.CUSTOM_FIELDS, UrlOwner.None)},
            {typeof(IssuePriority), new TypeUrl(RedmineKeys.ENUMERATION_ISSUE_PRIORITIES, UrlOwner.None)},
            {typeof(IssueRelation), new TypeUrl(RedmineKeys.RELATIONS, UrlOwner.Issue)},
            {typeof(IssueStatus), new TypeUrl(RedmineKeys.ISSUE_STATUSES, UrlOwner.None)},
            {typeof(Journal), new TypeUrl(RedmineKeys.JOURNALS, UrlOwner.None)},
            {typeof(News), new TypeUrl(RedmineKeys.NEWS, UrlOwner.None)},
            {typeof(Project), new TypeUrl(RedmineKeys.PROJECTS, UrlOwner.None)},
            {typeof(ProjectMembership), new TypeUrl(RedmineKeys.MEMBERSHIPS, UrlOwner.Project)},
            {typeof(Query), new TypeUrl(RedmineKeys.QUERIES, UrlOwner.None)},
            {typeof(Role), new TypeUrl(RedmineKeys.ROLES, UrlOwner.None)},
            {typeof(Search), new TypeUrl(RedmineKeys.SEARCH, UrlOwner.None)},
            {typeof(TimeEntry), new TypeUrl(RedmineKeys.TIME_ENTRIES, UrlOwner.None)},
            {typeof(TimeEntryActivity), new TypeUrl(RedmineKeys.ENUMERATION_TIME_ENTRY_ACTIVITIES, UrlOwner.None)},
            {typeof(Tracker), new TypeUrl(RedmineKeys.TRACKERS, UrlOwner.None)},
            {typeof(User), new TypeUrl(RedmineKeys.USERS, UrlOwner.None)},
            {typeof(Version), new TypeUrl(RedmineKeys.VERSIONS, UrlOwner.Project)},
            {typeof(Watcher), new TypeUrl(RedmineKeys.WATCHERS, UrlOwner.None)},
        };

        public RedmineApiUrls(string format)
        {
            Format = format;
        }
        
        public string ProjectFilesFragment(string projectId)
        {
            if (string.IsNullOrEmpty(projectId))
            {
                throw new RedmineException("The owner id(project id) is mandatory!");
            }

            return $"{RedmineKeys.PROJECTS}/{Escape(projectId)}/{RedmineKeys.FILES}.{Format}";
        }

        public string IssueAttachmentFragment(string issueId)
        {
            if (issueId.IsNullOrWhiteSpace())
            {
                throw new RedmineException("The issue id is mandatory!");
            }

            return $"{RedmineKeys.ATTACHMENTS}/{RedmineKeys.ISSUES}/{Escape(issueId)}.{Format}";
        }
        
        public string ProjectParentFragment(string projectId, string mapTypeFragment)
        {
            if (string.IsNullOrEmpty(projectId))
            {
                throw new RedmineException("The owner project id is mandatory!");
            }

            return $"{RedmineKeys.PROJECTS}/{Escape(projectId)}/{mapTypeFragment}.{Format}";
        }

        public string IssueParentFragment(string issueId, string mapTypeFragment)
        {
            if (string.IsNullOrEmpty(issueId))
            {
                throw new RedmineException("The owner issue id is mandatory!");
            }

            return $"{RedmineKeys.ISSUES}/{Escape(issueId)}/{mapTypeFragment}.{Format}";
        }
        
        public static string TypeFragment(Dictionary<Type, string> mapTypeUrlFragments, Type type)
        {
            if (!mapTypeUrlFragments.TryGetValue(type, out var fragment))
            {
                throw new RedmineException($"There is no uri fragment defined for type {type.Name}");
            }

            return fragment;
        }

        private static TypeUrl GetTypeUrl(Type type)
        {
            if (!TypeUrls.TryGetValue(type, out var typeUrl))
            {
                throw new RedmineException($"There is no uri fragment defined for type {type.Name}");
            }

            return typeUrl;
        }

        private string OwnedListFragment(Type type, string ownerId)
        {
            if (type == typeof(File))
            {
                return ProjectFilesFragment(ownerId);
            }

            var typeUrl = GetTypeUrl(type);

            return typeUrl.Owner switch
            {
                UrlOwner.Project => ProjectParentFragment(ownerId, typeUrl.Fragment),
                UrlOwner.Issue => IssueParentFragment(ownerId, typeUrl.Fragment),
                _ => $"{typeUrl.Fragment}.{Format}"
            };
        }

        public string CreateEntityFragment<T>(string ownerId = null)
        {
            var type = typeof(T);

            return CreateEntityFragment(type, ownerId);
        }
        public string CreateEntityFragment<T>(RequestOptions requestOptions)
        {
            var type = typeof(T);

            return CreateEntityFragment(type, requestOptions);
        }
        internal string CreateEntityFragment(Type type, RequestOptions requestOptions)
        {
            string ownerId = null;
            if (requestOptions is { QueryString: not null })
            {
                ownerId = requestOptions.QueryString.Get(RedmineKeys.PROJECT_ID) ??
                          requestOptions.QueryString.Get(RedmineKeys.ISSUE_ID);
            }

            return CreateEntityFragment(type, ownerId);
        }
        internal string CreateEntityFragment(Type type, string ownerId = null)
        {
            if (type == typeof(Upload))
            {
                return UploadFragment(ownerId);
            }

            if (type == typeof(Attachment) || type == typeof(Attachments))
            {
                return IssueAttachmentFragment(ownerId);
            }

            return OwnedListFragment(type, ownerId);
        }

        public string GetFragment<T>(string id) where T : class, new()
        {
            var type = typeof(T);

            return GetFragment(type, id);
        }
        internal string GetFragment(Type type, string id)
        {
            return $"{GetTypeUrl(type).Fragment}/{Escape(id)}.{Format}";
        }

        public string PatchFragment<T>(string ownerId)
        {
            var type = typeof(T);

            return PatchFragment(type, ownerId);
        }
        internal string PatchFragment(Type type, string ownerId)
        {
            if (type == typeof(Attachment) || type == typeof(Attachments))
            {
                return IssueAttachmentFragment(ownerId);
            }

            throw new RedmineException($"No endpoint defined for type {type} for PATCH operation.");
        }

        public string DeleteFragment<T>(string id)
        {
            var type = typeof(T);

            return DeleteFragment(type, id);
        }
        internal string DeleteFragment(Type type, string id)
        {
            return $"{GetTypeUrl(type).Fragment}/{Escape(id)}.{Format}";
        }

        public string UpdateFragment<T>(string id)
        {
            var type = typeof(T);

            return UpdateFragment(type, id);
        }
        internal string UpdateFragment(Type type, string id)
        {
            return $"{GetTypeUrl(type).Fragment}/{Escape(id)}.{Format}";
        }

        public string GetListFragment<T>(string ownerId = null) where T : class, new()
        {
            var type = typeof(T);

            return GetListFragment(type, ownerId);
        }
        
        public string GetListFragment<T>(RequestOptions requestOptions) where T : class, new()
        {
            var type = typeof(T);
            
            return GetListFragment(type, requestOptions);
        }
        
        internal string GetListFragment(Type type, RequestOptions requestOptions)
        {
            string ownerId = null;
            if (requestOptions is { QueryString: not null })
            {
                ownerId = requestOptions.QueryString.Get(RedmineKeys.PROJECT_ID) ??
                          requestOptions.QueryString.Get(RedmineKeys.ISSUE_ID);
            }
            
            return GetListFragment(type, ownerId);
        }

        internal string GetListFragment(Type type, string ownerId = null)
        {
            return OwnedListFragment(type, ownerId);
        }

        internal static string Escape(string value)
        {
            return string.IsNullOrEmpty(value) ? value ?? string.Empty : Uri.EscapeDataString(value);
        }

        public string UploadFragment(string fileName = null)
        {
            return !fileName.IsNullOrWhiteSpace() 
                ? $"{RedmineKeys.UPLOADS}.{Format}?filename={Uri.EscapeDataString(fileName)}" 
                : $"{RedmineKeys.UPLOADS}.{Format}";
        }
    }
}