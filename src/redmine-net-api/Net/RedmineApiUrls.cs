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

        private static readonly Dictionary<Type, string> TypeUrlFragments = new Dictionary<Type, string>()
        {
            {typeof(Attachment), RedmineKeys.ATTACHMENTS},
            {typeof(CustomField), RedmineKeys.CUSTOM_FIELDS},
            {typeof(DocumentCategory), RedmineKeys.ENUMERATION_DOCUMENT_CATEGORIES},
            {typeof(Group), RedmineKeys.GROUPS},
            {typeof(Issue), RedmineKeys.ISSUES},
            {typeof(IssueCategory), RedmineKeys.ISSUE_CATEGORIES},
            {typeof(IssueCustomField), RedmineKeys.CUSTOM_FIELDS},
            {typeof(IssuePriority), RedmineKeys.ENUMERATION_ISSUE_PRIORITIES},
            {typeof(IssueRelation), RedmineKeys.RELATIONS},
            {typeof(IssueStatus), RedmineKeys.ISSUE_STATUSES},
            {typeof(Journal), RedmineKeys.JOURNALS},
            {typeof(News), RedmineKeys.NEWS},
            {typeof(Project), RedmineKeys.PROJECTS},
            {typeof(ProjectMembership), RedmineKeys.MEMBERSHIPS},
            {typeof(Query), RedmineKeys.QUERIES},
            {typeof(Role), RedmineKeys.ROLES},
            {typeof(Search), RedmineKeys.SEARCH},
            {typeof(TimeEntry), RedmineKeys.TIME_ENTRIES},
            {typeof(TimeEntryActivity), RedmineKeys.ENUMERATION_TIME_ENTRY_ACTIVITIES},
            {typeof(Tracker), RedmineKeys.TRACKERS},
            {typeof(User), RedmineKeys.USERS},
            {typeof(Version), RedmineKeys.VERSIONS},
            {typeof(Watcher), RedmineKeys.WATCHERS},
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

            return $"{RedmineKeys.PROJECTS}/{projectId}/{RedmineKeys.FILES}.{Format}";
        }

        public string IssueAttachmentFragment(string issueId)
        {
            if (issueId.IsNullOrWhiteSpace())
            {
                throw new RedmineException("The issue id is mandatory!");
            }

            return $"/{RedmineKeys.ATTACHMENTS}/{RedmineKeys.ISSUES}/{issueId}.{Format}";
        }
        
        public string ProjectParentFragment(string projectId, string mapTypeFragment)
        {
            if (string.IsNullOrEmpty(projectId))
            {
                throw new RedmineException("The owner project id is mandatory!");
            }

            return $"{RedmineKeys.PROJECTS}/{projectId}/{mapTypeFragment}.{Format}";
        }

        public string IssueParentFragment(string issueId, string mapTypeFragment)
        {
            if (string.IsNullOrEmpty(issueId))
            {
                throw new RedmineException("The owner issue id is mandatory!");
            }

            return $"{RedmineKeys.ISSUES}/{issueId}/{mapTypeFragment}.{Format}";
        }
        
        public static string TypeFragment(Dictionary<Type, string> mapTypeUrlFragments, Type type)
        {
            if (!mapTypeUrlFragments.TryGetValue(type, out var fragment))
            {
                throw new RedmineException($"There is no uri fragment defined for type {type.Name}");
            }

            return fragment;
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
            if (type == typeof(Version) || type == typeof(IssueCategory) || type == typeof(ProjectMembership))
            {
                return ProjectParentFragment(ownerId, TypeUrlFragments[type]);
            }

            if (type == typeof(IssueRelation))
            {
                return IssueParentFragment(ownerId, TypeUrlFragments[type]);
            }

            if (type == typeof(File))
            {
                return ProjectFilesFragment(ownerId);
            }

            if (type == typeof(Upload))
            {
                return $"{RedmineKeys.UPLOADS}.{Format}";
            }

            if (type == typeof(Attachment) || type == typeof(Attachments))
            {
                return IssueAttachmentFragment(ownerId);
            }

            return $"{TypeFragment(TypeUrlFragments, type)}.{Format}";
        }

        public string GetFragment<T>(string id) where T : class, new()
        {
            var type = typeof(T);

            return GetFragment(type, id);
        }
        internal string GetFragment(Type type, string id)
        {
            return $"{TypeFragment(TypeUrlFragments, type)}/{id}.{Format}";
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
            return $"{TypeFragment(TypeUrlFragments, type)}/{id}.{Format}";
        }

        public string UpdateFragment<T>(string id)
        {
            var type = typeof(T);

            return UpdateFragment(type, id);
        }
        internal string UpdateFragment(Type type, string id)
        {
            return $"{TypeFragment(TypeUrlFragments, type)}/{id}.{Format}";
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
            if (type == typeof(Version) || type == typeof(IssueCategory) || type == typeof(ProjectMembership))
            {
                return ProjectParentFragment(ownerId, TypeUrlFragments[type]);
            }

            if (type == typeof(IssueRelation))
            {
                return IssueParentFragment(ownerId, TypeUrlFragments[type]);
            }

            if (type == typeof(File))
            {
                return ProjectFilesFragment(ownerId);
            }

            return $"{TypeFragment(TypeUrlFragments, type)}.{Format}";
        }

        public string UploadFragment(string fileName = null)
        {
            return !fileName.IsNullOrWhiteSpace() 
                ? $"{RedmineKeys.UPLOADS}.{Format}?filename={Uri.EscapeDataString(fileName)}" 
                : $"{RedmineKeys.UPLOADS}.{Format}";
        }
    }
}