using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.JSonConverters2
{
    public class IssueConverter : IJsonConverter
    {
        public void Serialize(JsonWriter writer, object obj, JsonSerializer serializer)
        {
            if (obj == null) return;

            var issue = (Issue) obj;

            writer.WriteStartObject();
            writer.WriteValue(RedmineKeys.ISSUE);

            writer.WriteProperty(RedmineKeys.SUBJECT, issue.Subject);
            writer.WriteProperty(RedmineKeys.DESCRIPTION, issue.Description);
            writer.WriteProperty(RedmineKeys.NOTES, issue.Notes);

            if (issue.Id != 0)
            {
                writer.WriteProperty(RedmineKeys.PRIVATE_NOTES, issue.PrivateNotes);
            }

            writer.WriteProperty(RedmineKeys.IS_PRIVATE, issue.IsPrivate);

            writer.WriteIdIfNotNull(RedmineKeys.PROJECT_ID, issue.Project);
            writer.WriteIdIfNotNull(RedmineKeys.PRIORITY_ID, issue.Priority);
            writer.WriteIdIfNotNull(RedmineKeys.STATUS_ID, issue.Status);
            writer.WriteIdIfNotNull(RedmineKeys.CATEGORY_ID, issue.Category);
            writer.WriteIdIfNotNull(RedmineKeys.TRACKER_ID, issue.Tracker);
            writer.WriteIdIfNotNull(RedmineKeys.ASSIGNED_TO_ID, issue.AssignedTo);
            writer.WriteIdIfNotNull(RedmineKeys.FIXED_VERSION_ID, issue.FixedVersion);
            writer.WriteValueOrEmpty(RedmineKeys.ESTIMATED_HOURS, issue.EstimatedHours);

            writer.WriteIdOrEmpty(RedmineKeys.PARENT_ISSUE_ID, issue.ParentIssue);
            writer.WriteDateOrEmpty(RedmineKeys.START_DATE, issue.StartDate);
            writer.WriteDateOrEmpty(RedmineKeys.DUE_DATE, issue.DueDate);
            writer.WriteDateOrEmpty(RedmineKeys.UPDATED_ON, issue.DueDate);

            if (issue.DoneRatio != null)
                writer.WriteProperty(RedmineKeys.DONE_RATIO, issue.DoneRatio.Value.ToString(CultureInfo.InvariantCulture));

            if (issue.SpentHours != null)
                writer.WriteProperty(RedmineKeys.SPENT_HOURS, issue.SpentHours.Value.ToString(CultureInfo.InvariantCulture));

            writer.WriteArray(RedmineKeys.UPLOADS, issue.Uploads, new UploadConverter(), serializer);
            writer.WriteArray(RedmineKeys.CUSTOM_FIELDS, issue.CustomFields, new IssueCustomFieldConverter(), serializer);

            writer.WriteIdsArray(RedmineKeys.WATCHER_USER_IDS, issue.Watchers);

            writer.WriteEndObject();
        }

        public object Deserialize(JObject obj, JsonSerializer serializer)
        {
            if (obj == null) return null;

            var issue = new Issue
            {
                Id = obj.Value<int>(RedmineKeys.ID),
                Description = obj.Value<string>(RedmineKeys.DESCRIPTION),

                Project = obj.GetValueAsIdentifiableName(RedmineKeys.PROJECT),
                Tracker = obj.GetValueAsIdentifiableName(RedmineKeys.TRACKER),
                Status = obj.GetValueAsIdentifiableName(RedmineKeys.STATUS),

                CreatedOn = obj.Value<DateTime?>(RedmineKeys.CREATED_ON),
                UpdatedOn = obj.Value<DateTime?>(RedmineKeys.UPDATED_ON),
                ClosedOn = obj.Value<DateTime?>(RedmineKeys.CLOSED_ON),

                Priority = obj.GetValueAsIdentifiableName(RedmineKeys.PRIORITY),
                Author = obj.GetValueAsIdentifiableName(RedmineKeys.AUTHOR),
                AssignedTo = obj.GetValueAsIdentifiableName(RedmineKeys.ASSIGNED_TO),
                Category = obj.GetValueAsIdentifiableName(RedmineKeys.CATEGORY),
                FixedVersion = obj.GetValueAsIdentifiableName(RedmineKeys.FIXED_VERSION),

                Subject = obj.Value<string>(RedmineKeys.SUBJECT),
                Notes = obj.Value<string>(RedmineKeys.NOTES),
                IsPrivate = obj.Value<bool>(RedmineKeys.IS_PRIVATE),
                StartDate = obj.Value<DateTime?>(RedmineKeys.START_DATE),
                DueDate = obj.Value<DateTime?>(RedmineKeys.DUE_DATE),
                SpentHours = obj.Value<float>(RedmineKeys.SPENT_HOURS),
                DoneRatio = obj.Value<float>(RedmineKeys.DONE_RATIO),
                EstimatedHours = obj.Value<float>(RedmineKeys.ESTIMATED_HOURS),

                ParentIssue = obj.GetValueAsIdentifiableName(RedmineKeys.PARENT),
                CustomFields = obj.GetValueAsCollection<IssueCustomField>(RedmineKeys.CUSTOM_FIELDS),
                Attachments = obj.GetValueAsCollection<Attachment>(RedmineKeys.ATTACHMENTS),
                Relations = obj.GetValueAsCollection<IssueRelation>(RedmineKeys.RELATIONS),
                Journals = obj.GetValueAsCollection<Journal>(RedmineKeys.JOURNALS),
                Changesets = obj.GetValueAsCollection<ChangeSet>(RedmineKeys.CHANGESETS),
                Watchers = obj.GetValueAsCollection<Watcher>(RedmineKeys.WATCHERS),
                Children = obj.GetValueAsCollection<IssueChild>(RedmineKeys.CHILDREN)
            };

            return issue;
        }
    }
}
