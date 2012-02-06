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
    [XmlRoot("issue_category")]
    public class IssueCategory : Identifiable<IssueCategory>, IEquatable<IssueCategory>, IXmlSerializable
    {
        [XmlElement("project ")]
        public IdentifiableName Project { get; set; }

        [XmlElement("assigned_to")]
        public IdentifiableName AsignTo { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        public bool Equals(IssueCategory other)
        {
            if (other == null) return false;
            return (Id == other.Id && Project == other.Project && AsignTo == other.AsignTo && Name == other.Name);
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
                    case "id": Id = reader.ReadElementContentAsInt(); break;

                    case "project": Project = new IdentifiableName(reader); break;

                    case "assigned_to": AsignTo = new IdentifiableName(reader); break;

                    case "name": Name = reader.ReadElementContentAsString(); break;

                    default:
                        reader.Read();
                        break;
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteIdIfNotNull(Project, "project_id");
            writer.WriteElementString("name", Name);
            writer.WriteIdIfNotNull(AsignTo, "assigned_to_id");
        }
    }
}