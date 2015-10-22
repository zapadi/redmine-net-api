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
    /// Availability 1.4
    /// </summary>
    [XmlRoot(RedmineKeys.ROLE)]
    public class Role : IXmlSerializable, IEquatable<Role>
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        [XmlElement(RedmineKeys.ID)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [XmlElement(RedmineKeys.NAME)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the permissions.
        /// </summary>
        /// <value>
        /// The issue relations.
        /// </value>
        [XmlArray(RedmineKeys.PERMISSIONS)]
        [XmlArrayItem(RedmineKeys.PERMISSION)]
        public IList<Permission> Permissions { get; set; }

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

                    case RedmineKeys.NAME: Name = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.PERMISSIONS: Permissions = reader.ReadElementContentAsCollection<Permission>(); break;

                    default: reader.Read(); break;
                }
            }
        }

        public void WriteXml(XmlWriter writer) { }

        public bool Equals(Role other)
        {
            if (other == null) return false;
            return Id == other.Id && Name == other.Name;
        }

        public override string ToString()
        {
            return Id + ", " + Name;
        }
    }
}