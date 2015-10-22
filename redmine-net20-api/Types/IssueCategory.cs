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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// Availability 1.3
    /// </summary>
    [XmlRoot(RedmineKeys.ISSUE_CATEGORY)]
    public class IssueCategory : Identifiable<IssueCategory>, IEquatable<IssueCategory>, IXmlSerializable
    {
        /// <summary>
        /// Gets or sets the project.
        /// </summary>
        /// <value>
        /// The project.
        /// </value>
        [XmlElement(RedmineKeys.PROJECT)]
        public IdentifiableName Project { get; set; }

        /// <summary>
        /// Gets or sets the asign to.
        /// </summary>
        /// <value>
        /// The asign to.
        /// </value>
        [XmlElement(RedmineKeys.ASSIGNED_TO)]
        public IdentifiableName AsignTo { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [XmlElement(RedmineKeys.NAME)]
        public string Name { get; set; }

        public bool Equals(IssueCategory other)
        {
            if (other == null) return false;
            return (Id == other.Id && Project == other.Project && AsignTo == other.AsignTo && Name == other.Name);
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

                    case RedmineKeys.PROJECT: Project = new IdentifiableName(reader); break;

                    case RedmineKeys.ASSIGNED_TO: AsignTo = new IdentifiableName(reader); break;

                    case RedmineKeys.NAME: Name = reader.ReadElementContentAsString(); break;

                    default: reader.Read(); break;
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteIdIfNotNull(Project, RedmineKeys.PROJECT_ID);
            writer.WriteElementString(RedmineKeys.NAME, Name);
            writer.WriteIdIfNotNull(AsignTo, RedmineKeys.ASSIGNED_TO_ID);
        }
    }
}