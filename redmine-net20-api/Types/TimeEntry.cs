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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;


namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// Availability 1.1
    /// </summary>
    [XmlRoot(RedmineKeys.TIME_ENTRY)]
    public class TimeEntry : Identifiable<TimeEntry>, ICloneable, IEquatable<TimeEntry>, IXmlSerializable
    {
        private string comments;

        /// <summary>
        /// Gets or sets the issue id to log time on.
        /// </summary>
        /// <value>The issue id.</value>
        [XmlAttribute(RedmineKeys.ISSUE)]
        public IdentifiableName Issue { get; set; }

        /// <summary>
        /// Gets or sets the project id to log time on.
        /// </summary>
        /// <value>The project id.</value>
        [XmlAttribute(RedmineKeys.PROJECT)]
        public IdentifiableName Project { get; set; }

        /// <summary>
        /// Gets or sets the date the time was spent (default to the current date).
        /// </summary>
        /// <value>The spent on.</value>
        [XmlAttribute(RedmineKeys.SPENT_ON)]
        public DateTime? SpentOn { get; set; }

        /// <summary>
        /// Gets or sets the number of spent hours.
        /// </summary>
        /// <value>The hours.</value>
        [XmlAttribute(RedmineKeys.HOURS)]
        public decimal Hours { get; set; }

        /// <summary>
        /// Gets or sets the activity id of the time activity. This parameter is required unless a default activity is defined in Redmine..
        /// </summary>
        /// <value>The activity id.</value>
        [XmlAttribute(RedmineKeys.ACTIVITY)]
        public IdentifiableName Activity { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        [XmlAttribute(RedmineKeys.USER)]
        public IdentifiableName User { get; set; }

        /// <summary>
        /// Gets or sets the short description for the entry (255 characters max).
        /// </summary>
        /// <value>The comments.</value>
        [XmlAttribute(RedmineKeys.COMMENTS)]
        public String Comments
        {
            get { return comments; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (value.Length > 255)
                    {
                        value = value.Substring(0, 255);
                    }
                }
                comments = value;
            }
        }

        /// <summary>
        /// Gets or sets the created on.
        /// </summary>
        /// <value>The created on.</value>
        [XmlElement(RedmineKeys.CREATED_ON)]
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the updated on.
        /// </summary>
        /// <value>The updated on.</value>
        [XmlElement(RedmineKeys.UPDATED_ON)]
        public DateTime? UpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets the custom fields.
        /// </summary>
        /// <value>The custom fields.</value>
        [XmlArray(RedmineKeys.CUSTOM_FIELDS)]
        [XmlArrayItem(RedmineKeys.CUSTOM_FIELD)]
        public IList<IssueCustomField> CustomFields { get; set; }

        public object Clone()
        {
            var timeEntry = new TimeEntry { Activity = Activity, Comments = Comments, Hours = Hours, Issue = Issue, Project = Project, SpentOn = SpentOn, User = User, CustomFields = CustomFields };
            return timeEntry;
        }

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
                    case RedmineKeys.ID: Id = reader.ReadElementContentAsInt(); break;

                    case RedmineKeys.ISSUE_ID: Issue = new IdentifiableName(reader); break;

                    case RedmineKeys.ISSUE: Issue = new IdentifiableName(reader); break;

                    case RedmineKeys.PROJECT_ID: Project = new IdentifiableName(reader); break;

                    case RedmineKeys.PROJECT: Project = new IdentifiableName(reader); break;

                    case RedmineKeys.SPENT_ON: SpentOn = reader.ReadElementContentAsNullableDateTime(); break;

                    case RedmineKeys.USER: User = new IdentifiableName(reader); break;

                    case RedmineKeys.HOURS: Hours = reader.ReadElementContentAsDecimal(); break;

                    case RedmineKeys.ACTIVITY_ID: Activity = new IdentifiableName(reader); break;

                    case RedmineKeys.ACTIVITY: Activity = new IdentifiableName(reader); break;

                    case RedmineKeys.COMMENTS: Comments = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;

                    case RedmineKeys.UPDATED_ON: UpdatedOn = reader.ReadElementContentAsNullableDateTime(); break;

                    case RedmineKeys.CUSTOM_FIELDS: CustomFields = reader.ReadElementContentAsCollection<IssueCustomField>(); break;

                    default: reader.Read(); break;
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteIdIfNotNull(Issue, RedmineKeys.ISSUE_ID);
            writer.WriteIdIfNotNull(Project, RedmineKeys.PROJECT_ID);
            if (!SpentOn.HasValue) SpentOn = DateTime.Now;
            writer.WriteDateOrEmpty(SpentOn, RedmineKeys.SPENT_ON);
            writer.WriteValueOrEmpty<decimal>(Hours, RedmineKeys.HOURS);
            writer.WriteIdIfNotNull(Activity, RedmineKeys.ACTIVITY_ID);
            writer.WriteElementString(RedmineKeys.COMMENTS, Comments);
        }

        public bool Equals(TimeEntry other)
        {
            if (other == null) return false;
            return (Id == other.Id
                && Issue == other.Issue
                && Project == other.Project
                && SpentOn == other.SpentOn
                && Hours == other.Hours
                && Activity == other.Activity
                && Comments == other.Comments
                && User == other.User
                && CreatedOn == other.CreatedOn
                && UpdatedOn == other.UpdatedOn
                && Equals(CustomFields, other.CustomFields));
        }
    }
}