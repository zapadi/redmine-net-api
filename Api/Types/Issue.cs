/*
   Copyright 2011 Dorin Huzum, Adrian Popescu.

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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    [Serializable]
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
        /// Gets or sets the status.
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
        /// Gets or sets the custom fields.
        /// </summary>
        /// <value>The custom fields.</value>
        [XmlArray("custom_fields")]
        [XmlArrayItem("custom_field")]
        public IList<CustomField> CustomFields { get; set; }

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

        [XmlElement("assigned_to")]
        public IdentifiableName AssignedTo { get; set; }

        public XmlSchema GetSchema()
        {
            return null;
        }

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
                    case "id":
                        Id = reader.ReadElementContentAsInt();
                        break;

                    case "project":
                        Project = new IdentifiableName(reader);
                        break;

                    case "tracker":
                        Tracker = new IdentifiableName(reader);
                        break;

                    case "status":
                        Status = new IdentifiableName(reader);
                        break;

                    case "priority":
                        Priority = new IdentifiableName(reader);
                        break;

                    case "author":
                        Author = new IdentifiableName(reader);
                        break;

                    case "assigned_to":
                        AssignedTo = new IdentifiableName(reader);
                        break;

                    case "category":
                        Category = new IdentifiableName(reader);
                        break;

                    case "subject":
                        Subject = reader.ReadElementContentAsString();
                        break;

                    case "description":
                        Description = reader.ReadElementContentAsString();
                        break;

                    case "start_date":
                        StartDate = reader.ReadElementContentAsNullableDateTime();
                        break;

                    case "due_date":
                        DueDate = reader.ReadElementContentAsNullableDateTime();
                        break;

                    case "done_ratio":
                        DoneRatio = reader.ReadElementContentAsNullableFloat();
                        break;

                    case "estimated_hours":
                        EstimatedHours = reader.ReadElementContentAsNullableFloat();
                        break;

                    case "created_on":
                        CreatedOn = reader.ReadElementContentAsNullableDateTime();
                        break;

                    case "updated_on":
                        UpdatedOn = reader.ReadElementContentAsNullableDateTime();
                        break;

                    case "custom_fields":
                        CustomFields = reader.ReadElementContentAsCollection<CustomField>();
                        break;

                    default:
                        reader.Read();
                        break;
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("subject", Subject);
            writer.WriteElementString("description", Description);
            writer.WriteIdIfNotNull(Project, "project_id");
            writer.WriteIdIfNotNull(Priority, "priority_id");
            writer.WriteIdIfNotNull(Status, "status_id");
            writer.WriteIdIfNotNull(Category, "category_id");
            writer.WriteIdIfNotNull(Tracker, "tracker_id");
            writer.WriteIdIfNotNull(AssignedTo, "assigned_to_id");
        }

        public object Clone()
        {
            var issue = new Issue {AssignedTo = AssignedTo,Author = Author, Category = Category, CustomFields = CustomFields, Description = Description, DoneRatio = DoneRatio, DueDate = DueDate, EstimatedHours = EstimatedHours, Priority = Priority, StartDate = StartDate, Status = Status, Subject = Subject, Tracker = Tracker, Project = Project};
            return issue;
        }

        public bool Equals(Issue other)
        {
            if (other == null) return false;
            return (Id == other.Id && Project == other.Project && Tracker == other.Tracker && Status == other.Status && Priority == other.Priority 
                && Author == other.Author && Category == other.Category && Subject == other.Subject && Description == other.Description && StartDate == other.StartDate
                && DueDate == other.DueDate && DoneRatio == other.DoneRatio && EstimatedHours == other.EstimatedHours && CustomFields == other.CustomFields 
                && CreatedOn == other.CreatedOn && UpdatedOn == other.UpdatedOn && AssignedTo == other.AssignedTo
                );
        }
    }
}