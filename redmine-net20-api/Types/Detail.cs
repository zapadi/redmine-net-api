/*
   Copyright 2011 - 2015 Adrian Popescu.

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
    [XmlRoot(RedmineKeys.DETAIL)]
    public class Detail : IXmlSerializable, IEquatable<Detail>
    {
        /// <summary>
        /// Gets or sets the property.
        /// </summary>
        /// <value>
        /// The property.
        /// </value>
        [XmlAttribute(RedmineKeys.PROPERTY)]
        public string Property { get; set; }

        /// <summary>
        /// Gets or sets the status id.
        /// </summary>
        /// <value>
        /// The status id.
        /// </value>
        [XmlAttribute(RedmineKeys.STATUS_ID)]
        public string StatusId { get; set; }

        /// <summary>
        /// Gets or sets the old value.
        /// </summary>
        /// <value>
        /// The old value.
        /// </value>
        [XmlElement(RedmineKeys.OLD_VALUE)]
        public string OldValue { get; set; }

        /// <summary>
        /// Gets or sets the new value.
        /// </summary>
        /// <value>
        /// The new value.
        /// </value>
        [XmlElement(RedmineKeys.NEW_VALUE)]
        public string NewValue { get; set; }

        public XmlSchema GetSchema() { return null; }

        public void ReadXml(XmlReader reader)
        {
            Property = reader.GetAttribute(RedmineKeys.PROPERTY);
            StatusId = reader.GetAttribute(RedmineKeys.STATUS_ID);

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
                    case RedmineKeys.OLD_VALUE: OldValue = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.NEW_VALUE: NewValue = reader.ReadElementContentAsString(); break;

                    default: reader.Read(); break;
                }
            }
        }

        public void WriteXml(XmlWriter writer) { }

        public bool Equals(Detail other)
        {
            if (other == null) return false;
            return Property.Equals(other.Property)
                && StatusId.Equals(other.StatusId)
                && OldValue.Equals(other.OldValue)
                && NewValue.Equals(other.NewValue);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as Detail);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 13;
                hashCode = Utils.GetHashCode(Property, hashCode);
                hashCode = Utils.GetHashCode(StatusId, hashCode);
                hashCode = Utils.GetHashCode(OldValue, hashCode);
                hashCode = Utils.GetHashCode(NewValue, hashCode);

                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format("Property: {0}, StatusId: {1}, OldValue: {2}, NewValue: {3}", Property, StatusId, OldValue, NewValue);
        }
    }
}