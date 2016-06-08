/*
   Copyright 2011 - 2016 Adrian Popescu.

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
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    [XmlRoot(RedmineKeys.CUSTOM_FIELD)]
    public class IssueCustomField : IdentifiableName, IEquatable<IssueCustomField>, ICloneable
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [XmlArray(RedmineKeys.VALUE)]
        [XmlArrayItem(RedmineKeys.VALUE)]
        public IList<CustomFieldValue> Values { get; set; }

        [XmlAttribute(RedmineKeys.MULTIPLE)]
        public bool Multiple { get; set; }

        public override void ReadXml(XmlReader reader)
        {
            Id = Convert.ToInt32(reader.GetAttribute(RedmineKeys.ID));
            Name = reader.GetAttribute(RedmineKeys.NAME);

            Multiple = reader.ReadAttributeAsBoolean(RedmineKeys.MULTIPLE);
            reader.Read();

            if (string.IsNullOrEmpty(reader.GetAttribute("type")))
            {
                Values = new List<CustomFieldValue> { new CustomFieldValue { Info = reader.ReadElementContentAsString() } };
            }
            else
            {
                var result = reader.ReadElementContentAsCollection<CustomFieldValue>();
                Values = result;
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            if (Values == null) return;
            var itemsCount = Values.Count;

            writer.WriteAttributeString(RedmineKeys.ID, Id.ToString(CultureInfo.InvariantCulture));
            if (itemsCount > 1)
            {
                writer.WriteArrayStringElement(Values, RedmineKeys.VALUE, GetValue);
                //                writer.WriteStartElement(RedmineKeys.VALUE);
                //                writer.WriteAttributeString("type", "array");
                //
                //                foreach (var v in Values) writer.WriteElementString(RedmineKeys.VALUE, v.Info);
                //
                //                writer.WriteEndElement();
            }
            else
            {
                writer.WriteElementString(RedmineKeys.VALUE, itemsCount > 0 ? Values[0].Info : null);
            }
        }

        public bool Equals(IssueCustomField other)
        {
            if (other == null) return false;
            return (Id == other.Id && Name == other.Name && Multiple == other.Multiple && Values.Equals<CustomFieldValue>(other.Values));
        }

        public object Clone()
        {
            var issueCustomField = new IssueCustomField { Multiple = Multiple, Values = Values.Clone<CustomFieldValue>() };
            return issueCustomField;
        }

        public override string ToString()
        {
            return string.Format("[IssueCustomField: {2} Values={0}, Multiple={1}]", Values, Multiple, base.ToString());
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 13;
                hashCode = HashCodeExtensions.GetHashCode(Id, hashCode);
                hashCode = HashCodeExtensions.GetHashCode(Name, hashCode);
                hashCode = HashCodeExtensions.GetHashCode(Values, hashCode);
                hashCode = HashCodeExtensions.GetHashCode(Multiple, hashCode);
                return hashCode;
            }
        }

        public string GetValue(object item)
        {
            return ((CustomFieldValue)item).Info;
        }
    }
}