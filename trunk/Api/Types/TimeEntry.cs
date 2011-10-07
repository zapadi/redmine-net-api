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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    [Serializable]
    [XmlRoot("time_entry")]
    public class TimeEntry : Identifiable<TimeEntry>, ICloneable, IEquatable<TimeEntry>, IXmlSerializable
    {
        /// <summary>
        /// Gets or sets the issue id.
        /// </summary>
        /// <value>The issue id.</value>
        [XmlAttribute("issue_id")]
        public int IssueId { get; set; }

        /// <summary>
        /// Gets or sets the project id.
        /// </summary>
        /// <value>The project id.</value>
        [XmlAttribute("project_id")]
        public int ProjectId { get; set; }

        /// <summary>
        /// Gets or sets the spent on.
        /// </summary>
        /// <value>The spent on.</value>
        [XmlAttribute("spent_on")]
        public DateTime? SpentOn { get; set; }

        /// <summary>
        /// Gets or sets the hours.
        /// </summary>
        /// <value>The hours.</value>
        [XmlAttribute("hours")]
        public int Hours { get; set; }

        /// <summary>
        /// Gets or sets the activity id.
        /// </summary>
        /// <value>The activity id.</value>
        [XmlAttribute("activity_id")]
        public int ActivityId { get; set; }

        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        /// <value>The comments.</value>
        [XmlAttribute("comments")]
        public String Comments { get; set; }

        public object Clone()
        {
            var timeEntry = new TimeEntry {ActivityId = ActivityId, Comments = Comments, Hours = Hours, IssueId = IssueId, ProjectId = ProjectId, SpentOn = SpentOn};
            return timeEntry;
        }

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
                    case "id": Id = reader.ReadElementContentAsInt();
                        break;

                    case "issue_id": IssueId = reader.ReadElementContentAsInt(); break;

                    case "project_id": ProjectId = reader.ReadElementContentAsInt(); break;

                    case "spent_on": SpentOn = reader.ReadElementContentAsNullableDateTime(); break;

                    case "hours": Hours = reader.ReadElementContentAsInt(); break;

                    case "activity_id": ActivityId = reader.ReadElementContentAsInt(); break;

                    case "comments": Comments = reader.ReadElementContentAsString(); break;

                    default:
                        reader.Read();
                        break;
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("issue_id", IssueId.ToString());
            writer.WriteElementString("project_id", ProjectId.ToString());
            writer.WriteIfNotDefaultOrNull(SpentOn, "spent_on");
            writer.WriteElementString("hours", Hours.ToString());
            writer.WriteElementString("activity_id", ActivityId.ToString());
            writer.WriteElementString("comments", Comments);
        }

        public bool Equals(TimeEntry other)
        {
            if (other == null) return false;
            return (Id == other.Id && IssueId == other.IssueId && ProjectId == other.ProjectId && SpentOn == other.SpentOn && Hours == other.Hours && ActivityId == other.ActivityId && Comments == other.Comments);
        }
    }
}