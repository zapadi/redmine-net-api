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
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Serialization;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Available as of 1.1 :
    /// include: fetch associated data (optional). 
    /// Possible values: children, attachments, relations, changesets and journals. To fetch multiple associations use comma (e.g ?include=relations,journals). 
    /// See Issue journals for more information.
    /// </remarks>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    [XmlRoot(RedmineKeys.ISSUE)]
    public sealed class Issue : 
        Identifiable<Issue>
        ,ICloneable<Issue>
    {
        #region Properties
        /// <summary>
        /// Gets or sets the project.
        /// </summary>
        /// <value>The project.</value>
        public IdentifiableName Project { get; set; }

        /// <summary>
        /// Gets or sets the tracker.
        /// </summary>
        /// <value>The tracker.</value>
        public IdentifiableName Tracker { get; set; }

        /// <summary>
        /// Gets or sets the status.Possible values: open, closed, * to get open and closed issues, status id
        /// </summary>
        /// <value>The status.</value>
        public IssueStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>The priority.</value>
        public IdentifiableName Priority { get; set; }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        /// <value>The author.</value>
        public IdentifiableName Author { get; set; }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        /// <value>The category.</value>
        public IdentifiableName Category { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>The subject.</value>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>The start date.</value>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the due date.
        /// </summary>
        /// <value>The due date.</value>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Gets or sets the done ratio.
        /// </summary>
        /// <value>The done ratio.</value>
        public float? DoneRatio { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [private notes].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [private notes]; otherwise, <c>false</c>.
        /// </value>
        public bool PrivateNotes { get; set; }

        /// <summary>
        /// Gets or sets the estimated hours.
        /// </summary>
        /// <value>The estimated hours.</value>
        public float? EstimatedHours { get; set; }

        /// <summary>
        /// Gets or sets the hours spent on the issue.
        /// </summary>
        /// <value>The hours spent on the issue.</value>
        public float? SpentHours { get; set; }

        /// <summary>
        /// Gets or sets the custom fields.
        /// </summary>
        /// <value>The custom fields.</value>
        public IList<IssueCustomField> CustomFields { get; set; }

        /// <summary>
        /// Gets or sets the created on.
        /// </summary>
        /// <value>The created on.</value>
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the updated on.
        /// </summary>
        /// <value>The updated on.</value>
        public DateTime? UpdatedOn { get; internal set; }

        /// <summary>
        /// Gets or sets the closed on.
        /// </summary>
        /// <value>The closed on.</value>
        public DateTime? ClosedOn { get; internal set; }

        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user to assign the issue to (currently no mechanism to assign by name).
        /// </summary>
        /// <value>
        /// The assigned to.
        /// </value>
        public IdentifiableName AssignedTo { get; set; }

        /// <summary>
        /// Gets or sets the parent issue id. Only when a new issue is created this property shall be used.
        /// </summary>
        /// <value>
        /// The parent issue id.
        /// </value>
        public IdentifiableName ParentIssue { get; set; }

        /// <summary>
        /// Gets or sets the fixed version.
        /// </summary>
        /// <value>
        /// The fixed version.
        /// </value>
        public IdentifiableName FixedVersion { get; set; }

        /// <summary>
        /// indicate whether the issue is private or not
        /// </summary>
        /// <value>
        /// <c>true</c> if this issue is private; otherwise, <c>false</c>.
        /// </value>
        public bool IsPrivate { get; set; }

        /// <summary>
        /// Returns the sum of spent hours of the task and all the sub tasks.
        /// </summary>
        /// <remarks>Availability starting with redmine version 3.3</remarks>
        public float? TotalSpentHours { get; set; }

        /// <summary>
        /// Returns the sum of estimated hours of task and all the sub tasks.
        /// </summary>
        /// <remarks>Availability starting with redmine version 3.3</remarks>
        public float? TotalEstimatedHours { get; set; }

        /// <summary>
        /// Gets or sets the journals.
        /// </summary>
        /// <value>
        /// The journals.
        /// </value>
        public IList<Journal> Journals { get; set; }

        /// <summary>
        /// Gets or sets the change sets.
        /// </summary>
        /// <value>
        /// The change sets.
        /// </value>
        public IList<ChangeSet> ChangeSets { get; set; }

        /// <summary>
        /// Gets or sets the attachments.
        /// </summary>
        /// <value>
        /// The attachments.
        /// </value>
        public IList<Attachment> Attachments { get; set; }

        /// <summary>
        /// Gets or sets the issue relations.
        /// </summary>
        /// <value>
        /// The issue relations.
        /// </value>
        public IList<IssueRelation> Relations { get; set; }

        /// <summary>
        /// Gets or sets the issue children.
        /// </summary>
        /// <value>
        /// The issue children.
        /// NOTE: Only Id, tracker and subject are filled.
        /// </value>
        public IList<IssueChild> Children { get; set; }

        /// <summary>
        /// Gets or sets the attachments.
        /// </summary>
        /// <value>
        /// The attachment.
        /// </value>
        public IList<Upload> Uploads { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IList<Watcher> Watchers { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public List<IssueAllowedStatus> AllowedStatuses { get; set; }
        #endregion

        #region Implementation of IXmlSerialization
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public override void ReadXml(XmlReader reader)
        {
            reader.Read();

            while (!reader.EOF)
            {
                if (reader.IsEmptyElement && !reader.HasAttributes)
                {
                    reader.Read();
                    continue;
                }

                switch (reader.Name)
                {
                    case RedmineKeys.ID: Id = reader.ReadElementContentAsInt(); break;
                    case RedmineKeys.ALLOWED_STATUSES: AllowedStatuses = reader.ReadElementContentAsCollection<IssueAllowedStatus>(); break;
                    case RedmineKeys.ASSIGNED_TO: AssignedTo = new IdentifiableName(reader); break;
                    case RedmineKeys.ATTACHMENTS: Attachments = reader.ReadElementContentAsCollection<Attachment>(); break;
                    case RedmineKeys.AUTHOR: Author = new IdentifiableName(reader); break;
                    case RedmineKeys.CATEGORY: Category = new IdentifiableName(reader); break;
                    case RedmineKeys.CHANGE_SETS: ChangeSets = reader.ReadElementContentAsCollection<ChangeSet>(); break;
                    case RedmineKeys.CHILDREN: Children = reader.ReadElementContentAsCollection<IssueChild>(); break;
                    case RedmineKeys.CLOSED_ON: ClosedOn = reader.ReadElementContentAsNullableDateTime(); break;
                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                    case RedmineKeys.CUSTOM_FIELDS: CustomFields = reader.ReadElementContentAsCollection<IssueCustomField>(); break;
                    case RedmineKeys.DESCRIPTION: Description = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.DONE_RATIO: DoneRatio = reader.ReadElementContentAsNullableFloat(); break;
                    case RedmineKeys.DUE_DATE: DueDate = reader.ReadElementContentAsNullableDateTime(); break;
                    case RedmineKeys.ESTIMATED_HOURS: EstimatedHours = reader.ReadElementContentAsNullableFloat(); break;
                    case RedmineKeys.FIXED_VERSION: FixedVersion = new IdentifiableName(reader); break;
                    case RedmineKeys.IS_PRIVATE: IsPrivate = reader.ReadElementContentAsBoolean(); break;
                    case RedmineKeys.JOURNALS: Journals = reader.ReadElementContentAsCollection<Journal>(); break;
                    case RedmineKeys.NOTES: Notes = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.PARENT: ParentIssue = new IdentifiableName(reader); break;
                    case RedmineKeys.PRIORITY: Priority = new IdentifiableName(reader); break;
                    case RedmineKeys.PRIVATE_NOTES: PrivateNotes = reader.ReadElementContentAsBoolean(); break;
                    case RedmineKeys.PROJECT: Project = new IdentifiableName(reader); break;
                    case RedmineKeys.RELATIONS: Relations = reader.ReadElementContentAsCollection<IssueRelation>(); break;
                    case RedmineKeys.SPENT_HOURS: SpentHours = reader.ReadElementContentAsNullableFloat(); break;
                    case RedmineKeys.START_DATE: StartDate = reader.ReadElementContentAsNullableDateTime(); break;
                    case RedmineKeys.STATUS: Status = new IssueStatus(reader); break;
                    case RedmineKeys.SUBJECT: Subject = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.TOTAL_ESTIMATED_HOURS: TotalEstimatedHours = reader.ReadElementContentAsNullableFloat(); break;
                    case RedmineKeys.TOTAL_SPENT_HOURS: TotalSpentHours = reader.ReadElementContentAsNullableFloat(); break;
                    case RedmineKeys.TRACKER: Tracker = new IdentifiableName(reader); break;
                    case RedmineKeys.UPDATED_ON: UpdatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                    case RedmineKeys.WATCHERS: Watchers = reader.ReadElementContentAsCollection<Watcher>(); break;
                    default: reader.Read(); break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString(RedmineKeys.SUBJECT, Subject);
            writer.WriteElementString(RedmineKeys.NOTES, Notes);

            if (Id != 0)
            {
                writer.WriteBoolean(RedmineKeys.PRIVATE_NOTES, PrivateNotes);
            }

            writer.WriteElementString(RedmineKeys.DESCRIPTION, Description);
            writer.WriteElementString(RedmineKeys.IS_PRIVATE, IsPrivate.ToString(CultureInfo.InvariantCulture).ToLowerInv());

            writer.WriteIdIfNotNull(RedmineKeys.PROJECT_ID, Project);
            writer.WriteIdIfNotNull(RedmineKeys.PRIORITY_ID, Priority);
            writer.WriteIdIfNotNull(RedmineKeys.STATUS_ID, Status);
            writer.WriteIdIfNotNull(RedmineKeys.CATEGORY_ID, Category);
            writer.WriteIdIfNotNull(RedmineKeys.TRACKER_ID, Tracker);
            writer.WriteIdIfNotNull(RedmineKeys.ASSIGNED_TO_ID, AssignedTo);
            writer.WriteIdIfNotNull(RedmineKeys.PARENT_ISSUE_ID, ParentIssue);
            writer.WriteIdIfNotNull(RedmineKeys.FIXED_VERSION_ID, FixedVersion);

            writer.WriteValueOrEmpty(RedmineKeys.ESTIMATED_HOURS, EstimatedHours);
            writer.WriteIfNotDefaultOrNull(RedmineKeys.DONE_RATIO, DoneRatio);

            writer.WriteDateOrEmpty(RedmineKeys.START_DATE, StartDate);
            writer.WriteDateOrEmpty(RedmineKeys.DUE_DATE, DueDate);
            writer.WriteDateOrEmpty(RedmineKeys.UPDATED_ON, UpdatedOn);

            writer.WriteArray(RedmineKeys.UPLOADS, Uploads);
            writer.WriteArray(RedmineKeys.CUSTOM_FIELDS, CustomFields);

            writer.WriteListElements(RedmineKeys.WATCHER_USER_IDS, (IEnumerable<IValue>)Watchers);
        }
        #endregion

        #region Implementation of IJsonSerializable
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public override void ReadJson(JsonReader reader)
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndObject)
                {
                    return;
                }

                if (reader.TokenType != JsonToken.PropertyName)
                {
                    continue;
                }

                switch (reader.Value)
                {
                    case RedmineKeys.ID: Id = reader.ReadAsInt32().GetValueOrDefault(); break;
                    case RedmineKeys.ALLOWED_STATUSES: AllowedStatuses = reader.ReadAsCollection<IssueAllowedStatus>(); break;
                    case RedmineKeys.ASSIGNED_TO: AssignedTo = new IdentifiableName(reader); break;
                    case RedmineKeys.ATTACHMENTS: Attachments = reader.ReadAsCollection<Attachment>(); break;
                    case RedmineKeys.AUTHOR: Author = new IdentifiableName(reader); break;
                    case RedmineKeys.CATEGORY: Category = new IdentifiableName(reader); break;
                    case RedmineKeys.CHANGE_SETS: ChangeSets = reader.ReadAsCollection<ChangeSet>(); break;
                    case RedmineKeys.CHILDREN: Children = reader.ReadAsCollection<IssueChild>(); break;
                    case RedmineKeys.CLOSED_ON: ClosedOn = reader.ReadAsDateTime(); break;
                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadAsDateTime(); break;
                    case RedmineKeys.CUSTOM_FIELDS: CustomFields = reader.ReadAsCollection<IssueCustomField>(); break;
                    case RedmineKeys.DESCRIPTION: Description = reader.ReadAsString(); break;
                    case RedmineKeys.DONE_RATIO: DoneRatio = (float?)reader.ReadAsDouble(); break;
                    case RedmineKeys.DUE_DATE: DueDate = reader.ReadAsDateTime(); break;
                    case RedmineKeys.ESTIMATED_HOURS: EstimatedHours = (float?)reader.ReadAsDouble(); break;
                    case RedmineKeys.FIXED_VERSION: FixedVersion = new IdentifiableName(reader); break;
                    case RedmineKeys.IS_PRIVATE: IsPrivate = reader.ReadAsBoolean().GetValueOrDefault(); break;
                    case RedmineKeys.JOURNALS: Journals = reader.ReadAsCollection<Journal>(); break;
                    case RedmineKeys.NOTES: Notes = reader.ReadAsString(); break;
                    case RedmineKeys.PARENT: ParentIssue = new IdentifiableName(reader); break;
                    case RedmineKeys.PRIORITY: Priority = new IdentifiableName(reader); break;
                    case RedmineKeys.PRIVATE_NOTES: PrivateNotes = reader.ReadAsBoolean().GetValueOrDefault(); break;
                    case RedmineKeys.PROJECT: Project = new IdentifiableName(reader); break;
                    case RedmineKeys.RELATIONS: Relations = reader.ReadAsCollection<IssueRelation>(); break;
                    case RedmineKeys.SPENT_HOURS: SpentHours = (float?)reader.ReadAsDouble(); break;
                    case RedmineKeys.START_DATE: StartDate = reader.ReadAsDateTime(); break;
                    case RedmineKeys.STATUS: Status = new IssueStatus(reader); break;
                    case RedmineKeys.SUBJECT: Subject = reader.ReadAsString(); break;
                    case RedmineKeys.TOTAL_ESTIMATED_HOURS: TotalEstimatedHours = (float?)reader.ReadAsDouble(); break;
                    case RedmineKeys.TOTAL_SPENT_HOURS: TotalSpentHours = (float?)reader.ReadAsDouble(); break;
                    case RedmineKeys.TRACKER: Tracker = new IdentifiableName(reader); break;
                    case RedmineKeys.UPDATED_ON: UpdatedOn = reader.ReadAsDateTime(); break;
                    case RedmineKeys.WATCHERS: Watchers = reader.ReadAsCollection<Watcher>(); break;
                    default: reader.Read(); break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteJson(JsonWriter writer)
        {
            using (new JsonObject(writer, RedmineKeys.ISSUE))
            {
                writer.WriteProperty(RedmineKeys.SUBJECT, Subject);
                writer.WriteProperty(RedmineKeys.DESCRIPTION, Description);
                writer.WriteProperty(RedmineKeys.NOTES, Notes);

                if (Id != 0)
                {
                    writer.WriteBoolean(RedmineKeys.PRIVATE_NOTES, PrivateNotes);
                }

                writer.WriteBoolean(RedmineKeys.IS_PRIVATE, IsPrivate);
                writer.WriteIdIfNotNull(RedmineKeys.PROJECT_ID, Project);
                writer.WriteIdIfNotNull(RedmineKeys.PRIORITY_ID, Priority);
                writer.WriteIdIfNotNull(RedmineKeys.STATUS_ID, Status);
                writer.WriteIdIfNotNull(RedmineKeys.CATEGORY_ID, Category);
                writer.WriteIdIfNotNull(RedmineKeys.TRACKER_ID, Tracker);
                writer.WriteIdIfNotNull(RedmineKeys.ASSIGNED_TO_ID, AssignedTo);
                writer.WriteIdIfNotNull(RedmineKeys.FIXED_VERSION_ID, FixedVersion);
                writer.WriteValueOrEmpty(RedmineKeys.ESTIMATED_HOURS, EstimatedHours);

                writer.WriteIdOrEmpty(RedmineKeys.PARENT_ISSUE_ID, ParentIssue);
                writer.WriteDateOrEmpty(RedmineKeys.START_DATE, StartDate);
                writer.WriteDateOrEmpty(RedmineKeys.DUE_DATE, DueDate);
                writer.WriteDateOrEmpty(RedmineKeys.UPDATED_ON, UpdatedOn);

                if (DoneRatio != null)
                {
                    writer.WriteProperty(RedmineKeys.DONE_RATIO, DoneRatio.Value.ToString(CultureInfo.InvariantCulture));
                }

                if (SpentHours != null)
                {
                    writer.WriteProperty(RedmineKeys.SPENT_HOURS, SpentHours.Value.ToString(CultureInfo.InvariantCulture));
                }

                writer.WriteArray(RedmineKeys.UPLOADS, Uploads);
                writer.WriteArray(RedmineKeys.CUSTOM_FIELDS, CustomFields);

                writer.WriteRepeatableElement(RedmineKeys.WATCHER_USER_IDS, (IEnumerable<IValue>)Watchers);
            }
        }
        #endregion

        #region Implementation of IEquatable<Tracker>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(Issue other)
        {
            if (other == null) return false;
            return Id == other.Id
                && Project == other.Project
                && Tracker == other.Tracker
                && Status == other.Status
                && Priority == other.Priority
                && Author == other.Author
                && Category == other.Category
                && string.Equals(Subject, other.Subject, StringComparison.Ordinal)
                && string.Equals(Description, other.Description, StringComparison.Ordinal)
                && StartDate == other.StartDate
                && DueDate == other.DueDate
                && DoneRatio == other.DoneRatio
                && EstimatedHours == other.EstimatedHours
                && SpentHours == other.SpentHours
                && CreatedOn == other.CreatedOn
                && UpdatedOn == other.UpdatedOn
                && AssignedTo == other.AssignedTo
                && FixedVersion == other.FixedVersion
                && string.Equals(Notes, other.Notes, StringComparison.Ordinal)
                && ClosedOn == other.ClosedOn
                && PrivateNotes == other.PrivateNotes
                && (Attachments?.Equals<Attachment>(other.Attachments) ?? other.Attachments == null)
                && (CustomFields?.Equals<IssueCustomField>(other.CustomFields) ?? other.CustomFields == null)
                && (ChangeSets?.Equals<ChangeSet>(other.ChangeSets) ?? other.ChangeSets == null)
                && (Children?.Equals<IssueChild>(other.Children) ?? other.Children == null)
                && (Journals?.Equals<Journal>(other.Journals) ?? other.Journals == null)
                && (Relations?.Equals<IssueRelation>(other.Relations) ?? other.Relations == null)
                && (Watchers?.Equals<Watcher>(other.Watchers) ?? other.Watchers == null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as Issue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hashCode = base.GetHashCode();

            hashCode = HashCodeHelper.GetHashCode(Project, hashCode);
          
            hashCode = HashCodeHelper.GetHashCode(Tracker, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Status, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Priority, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Author, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Category, hashCode);
            
            hashCode = HashCodeHelper.GetHashCode(Subject, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Description, hashCode);
            hashCode = HashCodeHelper.GetHashCode(StartDate, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Project, hashCode);
            hashCode = HashCodeHelper.GetHashCode(DueDate, hashCode);
            hashCode = HashCodeHelper.GetHashCode(DoneRatio, hashCode);
            hashCode = HashCodeHelper.GetHashCode(PrivateNotes, hashCode);
            hashCode = HashCodeHelper.GetHashCode(EstimatedHours, hashCode);
            hashCode = HashCodeHelper.GetHashCode(SpentHours, hashCode);
            hashCode = HashCodeHelper.GetHashCode(CreatedOn, hashCode);
            hashCode = HashCodeHelper.GetHashCode(UpdatedOn, hashCode);

            hashCode = HashCodeHelper.GetHashCode(Notes, hashCode);
            hashCode = HashCodeHelper.GetHashCode(AssignedTo, hashCode);
            hashCode = HashCodeHelper.GetHashCode(ParentIssue, hashCode);
            hashCode = HashCodeHelper.GetHashCode(FixedVersion, hashCode);
            hashCode = HashCodeHelper.GetHashCode(IsPrivate, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Journals, hashCode);
            hashCode = HashCodeHelper.GetHashCode(CustomFields, hashCode);

            hashCode = HashCodeHelper.GetHashCode(ChangeSets, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Attachments, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Relations, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Children, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Uploads, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Watchers, hashCode);

            return hashCode;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Issue left, Issue right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Issue left, Issue right)
        {
            return !Equals(left, right);
        }
        #endregion

        #region Implementation of IClonable<T>
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public new Issue Clone(bool resetId)
        {
            var issue = new Issue
            {
                Project = Project?.Clone(false),
                Tracker = Tracker?.Clone(false),
                Status = Status?.Clone(false),
                Priority = Priority?.Clone(false),
                Author = Author?.Clone(false),
                Category = Category?.Clone(false),
                Subject = Subject,
                Description = Description,
                StartDate = StartDate,
                DueDate = DueDate,
                DoneRatio = DoneRatio,
                IsPrivate = IsPrivate,
                EstimatedHours = EstimatedHours,
                TotalEstimatedHours = TotalEstimatedHours,
                SpentHours = SpentHours,
                TotalSpentHours = TotalSpentHours,
                AssignedTo = AssignedTo?.Clone(false),
                FixedVersion = FixedVersion?.Clone(false),
                Notes = Notes,
                PrivateNotes = PrivateNotes,
                CreatedOn = CreatedOn,
                UpdatedOn = UpdatedOn,
                ClosedOn = ClosedOn,
                ParentIssue = ParentIssue?.Clone(false),
                CustomFields = CustomFields?.Clone(false),
                Journals = Journals?.Clone(false),
                Attachments = Attachments?.Clone(false),
                Relations = Relations?.Clone(false),
                Children = Children?.Clone(false),
                Watchers = Watchers?.Clone(false),
                Uploads = Uploads?.Clone(false),
            };

            return issue;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IdentifiableName AsParent()
        {
            return IdentifiableName.Create<IdentifiableName>(Id);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string DebuggerDisplay => $"[Issue:Id={Id.ToInvariantString()}, Status={Status?.Name}, Priority={Priority?.Name}, DoneRatio={DoneRatio?.ToString("F", CultureInfo.InvariantCulture)},IsPrivate={IsPrivate.ToInvariantString()}]";
    }
}