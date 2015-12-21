/*
   Copyright 2011 - 2015 Adrian Popescu, Dorin Huzum.

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
using System.Globalization;
using System.Linq;
using System.Web.Script.Serialization;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.JSonConverters
{
    internal class IssueConverter : JavaScriptConverter
    {
        #region Overrides of JavaScriptConverter

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary != null)
            {
                var issue = new Issue();

                issue.Id = dictionary.GetValue<int>(RedmineKeys.ID);
                issue.Description = dictionary.GetValue<string>(RedmineKeys.DESCRIPTION);
                issue.Project = dictionary.GetValueAsIdentifiableName(RedmineKeys.PROJECT);
                issue.Tracker = dictionary.GetValueAsIdentifiableName(RedmineKeys.TRACKER);
                issue.Status = dictionary.GetValueAsIdentifiableName(RedmineKeys.STATUS);
                issue.CreatedOn = dictionary.GetValue<DateTime?>(RedmineKeys.CREATED_ON);
                issue.UpdatedOn = dictionary.GetValue<DateTime?>(RedmineKeys.UPDATED_ON);
                issue.ClosedOn = dictionary.GetValue<DateTime?>(RedmineKeys.CLOSED_ON);
                issue.Priority = dictionary.GetValueAsIdentifiableName(RedmineKeys.PRIORITY);
                issue.Author = dictionary.GetValueAsIdentifiableName(RedmineKeys.AUTHOR);
                issue.AssignedTo = dictionary.GetValueAsIdentifiableName(RedmineKeys.ASSIGNED_TO);
                issue.Category = dictionary.GetValueAsIdentifiableName(RedmineKeys.CATEGORY);
                issue.FixedVersion = dictionary.GetValueAsIdentifiableName(RedmineKeys.FIXED_VERSION);
                issue.Subject = dictionary.GetValue<string>(RedmineKeys.SUBJECT);
                issue.Notes = dictionary.GetValue<string>(RedmineKeys.NOTES);
                issue.IsPrivate = dictionary.GetValue<bool>(RedmineKeys.IS_PRIVATE);
                issue.StartDate = dictionary.GetValue<DateTime?>(RedmineKeys.START_DATE);
                issue.DueDate = dictionary.GetValue<DateTime?>(RedmineKeys.DUE_DATE);
                issue.DoneRatio = dictionary.GetValue<float>(RedmineKeys.DONE_RATIO);
                issue.EstimatedHours = dictionary.GetValue<float>(RedmineKeys.ESTIMATED_HOURS);
                issue.ParentIssue = dictionary.GetValueAsIdentifiableName(RedmineKeys.PARENT);

                issue.CustomFields = dictionary.GetValueAsCollection<IssueCustomField>(RedmineKeys.CUSTOM_FIELDS);
                issue.Attachments = dictionary.GetValueAsCollection<Attachment>(RedmineKeys.ATTACHMENTS);
                issue.Relations = dictionary.GetValueAsCollection<IssueRelation>(RedmineKeys.RELATIONS);
                issue.Journals = dictionary.GetValueAsCollection<Journal>(RedmineKeys.JOURNALS);
                issue.Changesets = dictionary.GetValueAsCollection<ChangeSet>(RedmineKeys.CHANGESETS);
                issue.Watchers = dictionary.GetValueAsCollection<Watcher>(RedmineKeys.WATCHERS);
                issue.Children = dictionary.GetValueAsCollection<IssueChild>(RedmineKeys.CHILDREN);
                return issue;
            }

            return null;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var entity = obj as Issue;
            var result = new Dictionary<string, object>();

            if (entity != null)
            {
                result.Add(RedmineKeys.SUBJECT, entity.Subject);
                result.Add(RedmineKeys.DESCRIPTION, entity.Description);
                result.Add(RedmineKeys.NOTES, entity.Notes);
                if (entity.Id != 0)
                {
                    result.Add(RedmineKeys.PRIVATE_NOTES, entity.PrivateNotes);
                }
                result.Add(RedmineKeys.IS_PRIVATE, entity.IsPrivate);
                result.WriteIdIfNotNull(entity.Project, RedmineKeys.PROJECT_ID);
                result.WriteIdIfNotNull(entity.Priority, RedmineKeys.PRIORITY_ID);
                result.WriteIdIfNotNull(entity.Status, RedmineKeys.STATUS_ID);
                result.WriteIdIfNotNull(entity.Category, RedmineKeys.CATEGORY_ID);
                result.WriteIdIfNotNull(entity.Tracker, RedmineKeys.TRACKER_ID);
                result.WriteIdIfNotNull(entity.AssignedTo, RedmineKeys.ASSIGNED_TO_ID);
                result.WriteIdIfNotNull(entity.FixedVersion, RedmineKeys.FIXED_VERSION_ID);
                result.WriteValueOrEmpty(entity.EstimatedHours, RedmineKeys.ESTIMATED_HOURS);

                result.WriteIdOrEmpty(entity.ParentIssue, RedmineKeys.PARENT_ISSUE_ID);
                result.WriteDateOrEmpty(entity.StartDate, RedmineKeys.START_DATE);
                result.WriteDateOrEmpty(entity.DueDate, RedmineKeys.DUE_DATE);
                result.WriteDateOrEmpty(entity.DueDate, RedmineKeys.UPDATED_ON);
                if (entity.DoneRatio != null)
                    result.Add(RedmineKeys.DONE_RATIO, entity.DoneRatio.ToString());

                result.WriteArray(RedmineKeys.UPLOADS, entity.Uploads, new UploadConverter(), serializer);
                result.WriteArray(RedmineKeys.CUSTOM_FIELDS, entity.CustomFields, new IssueCustomFieldConverter(), serializer);

                result.WriteIdsArray(RedmineKeys.WATCHER_USER_IDS, entity.Watchers);
                
                var root = new Dictionary<string, object>();
                root[RedmineKeys.ISSUE] = result;
                return root;
            }

            return result;
        }

        public override IEnumerable<Type> SupportedTypes { get { return new List<Type>(new[] { typeof(Issue) }); } }

        #endregion
    }
}
