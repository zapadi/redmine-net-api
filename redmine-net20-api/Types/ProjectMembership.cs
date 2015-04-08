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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// Availability 1.4
    /// POST - Adds a project member.
    /// GET - Returns the membership of given :id.
    /// PUT - Updates the membership of given :id. Only the roles can be updated, the project and the user of a membership are read-only.
    /// DELETE - Deletes a memberships. Memberships inherited from a group membership can not be deleted. You must delete the group membership.
    /// </summary>
    [XmlRoot("membership")]
    public class ProjectMembership : Identifiable<ProjectMembership>, IEquatable<ProjectMembership>, IXmlSerializable
    {
        /// <summary>
        /// Gets or sets the project.
        /// </summary>
        /// <value>The project.</value>
        [XmlElement("project")]
        public IdentifiableName Project { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        [XmlElement("user")]
        public IdentifiableName User { get; set; }

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        /// <value>
        /// The group.
        /// </value>
        [XmlElement("group")]
        public IdentifiableName Group { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [XmlArray("roles")]
        [XmlArrayItem("role")]
        public List<MembershipRole> Roles { get; set; }

        public bool Equals(ProjectMembership other)
        {
            if (other == null) return false;
            return (Id == other.Id && Project == other.Project && Roles == other.Roles && User == other.User && Group == other.Group);
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
                    case "id": Id = reader.ReadElementContentAsInt(); break;

                    case "project": Project = new IdentifiableName(reader); break;

                    case "user": User = new IdentifiableName(reader); break;

                    case "group": Group = new IdentifiableName(reader); break;

                    case "roles": Roles = reader.ReadElementContentAsCollection<MembershipRole>(); break;

                    default: reader.Read(); break;
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            if (User != null) writer.WriteElementString("user_id", User.Id.ToString());

            writer.WriteStartElement("role_ids");
            writer.WriteAttributeString("type", "array");
            foreach (var role in Roles)
            {
                new XmlSerializer(role.GetType(),new XmlAttributeOverrides(),null,new XmlRootAttribute("role_id"),"").Serialize(writer, role);
            }
            writer.WriteEndElement();
        }
    }
}