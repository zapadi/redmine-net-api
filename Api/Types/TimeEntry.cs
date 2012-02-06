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
        [XmlAttribute("issue")]
        public IdentifiableName Issue { get; set; }

        /// <summary>
        /// Gets or sets the project id.
        /// </summary>
        /// <value>The project id.</value>
        [XmlAttribute("project")]
        public IdentifiableName Project { get; set; }

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
        public decimal Hours { get; set; }

        /// <summary>
        /// Gets or sets the activity id.
        /// </summary>
        /// <value>The activity id.</value>
        [XmlAttribute("activity")]
        public IdentifiableName Activity { get; set; }

        [XmlAttribute("user")]
        public IdentifiableName User { get; set; }

        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        /// <value>The comments.</value>
        [XmlAttribute("comments")]
        public String Comments { get; set; }

        public object Clone()
        {
            var timeEntry = new TimeEntry { Activity = Activity, Comments = Comments, Hours = Hours, Issue = Issue, Project = Project, SpentOn = SpentOn, User = User};
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

                    case "issue_id": Issue = new IdentifiableName(reader); break;

                    case "project_id": Project = new IdentifiableName(reader); break;

                    case "spent_on": SpentOn = reader.ReadElementContentAsNullableDateTime(); break;

                    case "user": User = new IdentifiableName(reader); break;

                    case "hours": Hours = reader.ReadElementContentAsDecimal(); break;

                    case "activity_id": Activity = new IdentifiableName(reader); break;

                    case "comments": Comments = reader.ReadElementContentAsString(); break;

                    default:
                        reader.Read();
                        break;
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteIdIfNotNull(Issue, "issue_id");
            writer.WriteIdIfNotNull(Project, "project_id");
            writer.WriteIfNotDefaultOrNull(SpentOn, "spent_on");
            writer.WriteElementString("hours", Hours.ToString());
            writer.WriteIdIfNotNull(Activity, "activity_id");
            writer.WriteElementString("comments", Comments);
        }

        public bool Equals(TimeEntry other)
        {
            if (other == null) return false;
            return (Id == other.Id && Issue == other.Issue && Project == other.Project && SpentOn == other.SpentOn && Hours == other.Hours && Activity == other.Activity && Comments == other.Comments && User == other.User);
        }
    }
}