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
    [XmlRoot("detail")]
    public class Detail : IXmlSerializable, IEquatable<Detail>
    {
        /// <summary>
        /// Gets or sets the property.
        /// </summary>
        /// <value>
        /// The property.
        /// </value>
        [XmlAttribute("property")]
        public string Property { get; set; }

        /// <summary>
        /// Gets or sets the status id.
        /// </summary>
        /// <value>
        /// The status id.
        /// </value>
        [XmlAttribute("name")]
        public string StatusId { get; set; }

        /// <summary>
        /// Gets or sets the old value.
        /// </summary>
        /// <value>
        /// The old value.
        /// </value>
        [XmlElement("old_value")]
        public string OldValue { get; set; }

        /// <summary>
        /// Gets or sets the new value.
        /// </summary>
        /// <value>
        /// The new value.
        /// </value>
        [XmlElement("new_value")]
        public string NewValue { get; set; }

        public XmlSchema GetSchema() { return null; }

        public void ReadXml(XmlReader reader)
        {
            Property = reader.GetAttribute("property");
            StatusId = reader.GetAttribute("name");

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
                    case "old_value": OldValue = reader.ReadElementContentAsString(); break;

                    case "new_value": NewValue = reader.ReadElementContentAsString(); break;

                    default: reader.Read(); break;
                }
            }
        }

        public void WriteXml(XmlWriter writer) { }

        public bool Equals(Detail other)
        {
            if (other == null) return false;
            return Property == other.Property && StatusId == other.StatusId && OldValue == other.OldValue && NewValue == other.NewValue;
        }
    }
}