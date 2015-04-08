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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    [XmlRoot("changeset")]
    public class ChangeSet : IXmlSerializable, IEquatable<ChangeSet>
    {
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("revision")]
        public int Revision { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("user")]
        public IdentifiableName User { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("comments")]
        public string Comments { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("committed_on")]
        public DateTime? CommittedOn { get; set; }

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

                Revision = reader.ReadAttributeAsInt("revision");

                switch (reader.Name)
                {
                    case "user": User = new IdentifiableName(reader); break;

                    case "comments": Comments = reader.ReadElementContentAsString(); break;

                    case "committed_on": CommittedOn = reader.ReadElementContentAsNullableDateTime(); break;

                    default: reader.Read(); break;
                }
            }
        }

        public void WriteXml(XmlWriter writer) { }

        public bool Equals(ChangeSet other)
        {
            if (other == null) return false;

            return Revision == other.Revision && User == other.User && Comments == other.Comments && CommittedOn == other.CommittedOn;
        }
    }
}