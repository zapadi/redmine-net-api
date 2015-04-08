/*
   Copyright 2011 - 2015 Adrian Popescu

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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// Available as of 1.1 :
    ///include: fetch associated data (optional). 
    ///Possible values: children, attachments, relations, changesets and journals. To fetch multiple associations use comma (e.g ?include=relations,journals). 
    /// See Issue journals for more information.
    /// </summary>
    [XmlRoot("issue")]
    public class Issue : Identifiable<Issue>, IXmlSerializable, IEquatable<Issue>, ICloneable
    {
        /// <summary>
        /// Gets or sets the project.
        /// </summary>
        /// <value>The project.</value>
        [XmlElement("project")]
        public IdentifiableName Project { get; set; }

        /// <summary>
        /// Gets or sets the tracker.
        /// </summary>
        /// <value>The tracker.</value>
        [XmlElement("tracker")]
        public IdentifiableName Tracker { get; set; }

        /// <summary>
        /// Gets or sets the status.Possible values: open, closed, * to get open and closed issues, status id
        /// </summary>
        /// <value>The status.</value>
        [XmlElement("status")]
        public IdentifiableName Status { get; set; }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>The priority.</value>
        [XmlElement("priority")]
        public IdentifiableName Priority { get; set; }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        /// <value>The author.</value>
        [XmlElement("author")]
        public IdentifiableName Author { get; set; }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        /// <value>The category.</value>
        [XmlElement("category")]
        public IdentifiableName Category { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>The subject.</value>
        [XmlElement("subject")]
        public String Subject { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [XmlElement("description")]
        public String Description { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>The start date.</value>
        [XmlElement("start_date")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the due date.
        /// </summary>
        /// <value>The due date.</value>
        [XmlElement("due_date")]
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Gets or sets the done ratio.
        /// </summary>
        /// <value>The done ratio.</value>
        [XmlElement("done_ratio", IsNullable = true)]
        public float? DoneRatio { get; set; }

        /// <summary>
        /// Gets or sets the estimated hours.
        /// </summary>
        /// <value>The estimated hours.</value>
        [XmlElement("estimated_hours", IsNullable = true)]
        public float? EstimatedHours { get; set; }

        /// <summary>
        /// Gets or sets the hours spent on the issue.
        /// </summary>
        /// <value>The hours spent on the issue.</value>
        [XmlElement("spent_hours", IsNullable = true)]
        public float? SpentHours { get; set; }

        /// <summary>
        /// Gets or sets the custom fields.
        /// </summary>
        /// <value>The custom fields.</value>
        [XmlArray("custom_fields")]
        [XmlArrayItem("custom_field")]
        public IList<IssueCustomField> CustomFields { get; set; }

        /// <summary>
        /// Gets or sets the created on.
        /// </summary>
        /// <value>The created on.</value>
        [XmlElement("created_on")]
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the updated on.
        /// </summary>
        /// <value>The updated on.</value>
        [XmlElement("updated_on")]
        public DateTime? UpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets the closed on.
        /// </summary>
        /// <value>The closed on.</value>
        [XmlElement("closed_on")]
        public DateTime? ClosedOn { get; set; }

        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        [XmlElement("notes")]
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user to assign the issue to (currently no mechanism to assign by name).
        /// </summary>
        /// <value>
        /// The assigned to.
        /// </value>
        [XmlElement("assigned_to")]
        public IdentifiableName AssignedTo { get; set; }

        /// <summary>
        /// Gets or sets the parent issue id. Only when a new issue is created this property shall be used.
        /// </summary>
        /// <value>
        /// The parent issue id.
        /// </value>
        [XmlElement("parent")]
        public IdentifiableName ParentIssue { get; set; }

        /// <summary>
        /// Gets or sets the fixed version.
        /// </summary>
        /// <value>
        /// The fixed version.
        /// </value>
        [XmlElement("fixed_version")]
        public IdentifiableName FixedVersion { get; set; }

        /// <summary>
        /// indicate whether the issue is private or not
        /// </summary>
        /// <value>
        /// <c>true</c> if this issue is private; otherwise, <c>false</c>.
        /// </value>
        [XmlElement("is_private")]
        public bool IsPrivate { get; set; }
        /// <summary>
        /// Gets or sets the journals.
        /// </summary>
        /// <value>
        /// The journals.
        /// </value>
        [XmlArray("journals")]
        [XmlArrayItem("journal")]
        public IList<Journal> Journals { get; set; }

        /// <summary>
        /// Gets or sets the changesets.
        /// </summary>
        /// <value>
        /// The changesets.
        /// </value>
        [XmlArray("changesets")]
        [XmlArrayItem("changeset")]
        public IList<ChangeSet> Changesets { get; set; }

        /// <summary>
        /// Gets or sets the attachments.
        /// </summary>
        /// <value>
        /// The attachments.
        /// </value>
        [XmlArray("attachments")]
        [XmlArrayItem("attachment")]
        public IList<Attachment> Attachments { get; set; }

        /// <summary>
        /// Gets or sets the issue relations.
        /// </summary>
        /// <value>
        /// The issue relations.
        /// </value>
        [XmlArray("relations")]
        [XmlArrayItem("relation")]
        public IList<IssueRelation> Relations { get; set; }

        /// <summary>
        /// Gets or sets the issue children.
        /// </summary>
        /// <value>
        /// The issue children.
        /// NOTE: Only Id, tracker and subject are filled.
        /// </value>
        [XmlArray("children")]
        [XmlArrayItem("issue")]
        public IList<IssueChild> Children { get; set; }

        /// <summary>
        /// Gets or sets the attachments.
        /// </summary>
        /// <value>
        /// The attachment.
        /// </value>
        [XmlArray("uploads")]
        [XmlArrayItem("upload")]
        public IList<Upload> Uploads { get; set; }

        [XmlArray("watchers")]
        [XmlArrayItem("watcher")]
        public IList<Watcher> Watchers { get; set; }

        public XmlSchema GetSchema() { return null; }

        public void ReadXml(XmlReader reader)
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
                    case "id": Id = reader.ReadElementContentAsInt(); break;

                    case "project": Project = new IdentifiableName(reader); break;

                    case "tracker": Tracker = new IdentifiableName(reader); break;

                    case "status": Status = new IdentifiableName(reader); break;

                    case "priority": Priority = new IdentifiableName(reader); break;

                    case "author": Author = new IdentifiableName(reader); break;

                    case "assigned_to": AssignedTo = new IdentifiableName(reader); break;

                    case "category": Category = new IdentifiableName(reader); break;

                    case "parent": ParentIssue = new IdentifiableName(reader); break;

                    case "fixed_version": FixedVersion = new IdentifiableName(reader); break;

                    case "subject": Subject = reader.ReadElementContentAsString(); break;

                    case "notes": Notes = reader.ReadElementContentAsString(); break;

                    case "description": Description = reader.ReadElementContentAsString(); break;

                    case "start_date": StartDate = reader.ReadElementContentAsNullableDateTime(); break;

                    case "due_date": DueDate = reader.ReadElementContentAsNullableDateTime(); break;

                    case "done_ratio": DoneRatio = reader.ReadElementContentAsNullableFloat(); break;

                    case "estimated_hours": EstimatedHours = reader.ReadElementContentAsNullableFloat(); break;

                    case "spent_hours" : SpentHours = reader.ReadElementContentAsNullableFloat(); break;

                    case "created_on": CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;

                    case "updated_on": UpdatedOn = reader.ReadElementContentAsNullableDateTime(); break;

                    case "closed_on": ClosedOn = reader.ReadElementContentAsNullableDateTime(); break;

                    case "custom_fields": CustomFields = reader.ReadElementContentAsCollection<IssueCustomField>(); break;

                    case "attachments": Attachments = reader.ReadElementContentAsCollection<Attachment>(); break;

                    case "relations": Relations = reader.ReadElementContentAsCollection<IssueRelation>(); break;

                    case "journals": Journals = reader.ReadElementContentAsCollection<Journal>(); break;

                    case "changesets": Changesets = reader.ReadElementContentAsCollection<ChangeSet>(); break;

                    case "children": Children = reader.ReadElementContentAsCollection<IssueChild>(); break;

                    case "watchers": Watchers = reader.ReadElementContentAsCollection<Watcher>(); break;

                    default: reader.Read(); break;
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("subject", Subject);
            writer.WriteElementString("notes", Notes);
            writer.WriteElementString("description", Description);
            writer.WriteIdIfNotNull(Project, "project_id");
            writer.WriteIdIfNotNull(Priority, "priority_id");
            writer.WriteIdIfNotNull(Status, "status_id");
            writer.WriteIdIfNotNull(Category, "category_id");
            writer.WriteIdIfNotNull(Tracker, "tracker_id");
            writer.WriteIdIfNotNull(AssignedTo, "assigned_to_id");
            writer.WriteElementString("is_private", IsPrivate.ToString());
            writer.WriteElementString("parent_issue_id", ParentIssue == null ? null : ParentIssue.Id.ToString(CultureInfo.InvariantCulture));
            writer.WriteIdIfNotNull(FixedVersion, "fixed_version_id");
            writer.WriteIfNotDefaultOrNull(EstimatedHours, "estimated_hours");
            if (StartDate != null)
                writer.WriteElementString("start_date", StartDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            if (DueDate != null)
                writer.WriteElementString("due_date", DueDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));

            writer.WriteIfNotDefaultOrNull(DoneRatio, "done_ratio");

            if (Uploads != null)
            {
                writer.WriteStartElement("uploads");
                //ajout Pierre Labrie
                writer.WriteAttributeString("type", "array");

                foreach (var u in Uploads)
                {
                    new XmlSerializer(u.GetType()).Serialize(writer, u);
                }
                writer.WriteEndElement();
            }

            if (CustomFields != null)
            {
                writer.WriteStartElement("custom_fields");
                writer.WriteAttributeString("type", "array");
                foreach (var cf in CustomFields)
                {
                    new XmlSerializer(cf.GetType()).Serialize(writer, cf);
                }
                writer.WriteEndElement();
            }

            if (Watchers != null)
            {
                writer.WriteStartElement("watcher_user_ids");
                writer.WriteAttributeString("type", "array");
                foreach (var watcher in Watchers)
                {
                    new XmlSerializer(typeof(int)).Serialize(writer, watcher.Id);
                }
                writer.WriteEndElement();
            }
        }

        public object Clone()
        {
            var issue = new Issue { AssignedTo = AssignedTo, Author = Author, Category = Category, CustomFields = CustomFields, Description = Description, DoneRatio = DoneRatio, DueDate = DueDate, SpentHours = SpentHours, EstimatedHours = EstimatedHours, Priority = Priority, StartDate = StartDate, Status = Status, Subject = Subject, Tracker = Tracker, Project = Project, FixedVersion = FixedVersion, Notes = Notes, Watchers = Watchers};
            return issue;
        }

        public bool Equals(Issue other)
        {
            if (other == null) return false;
            return (Id == other.Id && Project == other.Project && Tracker == other.Tracker && Status == other.Status && Priority == other.Priority
                && Author == other.Author && Category == other.Category && Subject == other.Subject && Description == other.Description && StartDate == other.StartDate
                && DueDate == other.DueDate && DoneRatio == other.DoneRatio && EstimatedHours == other.EstimatedHours && CustomFields == other.CustomFields
                && CreatedOn == other.CreatedOn && UpdatedOn == other.UpdatedOn && AssignedTo == other.AssignedTo && FixedVersion == other.FixedVersion
                && Notes == other.Notes && Watchers == other.Watchers && ClosedOn == other.ClosedOn && SpentHours == other.SpentHours
                );
        }
    }
}