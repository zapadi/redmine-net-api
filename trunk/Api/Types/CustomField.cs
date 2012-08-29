/*
   Copyright 2011 Adrian Popescu, Dorin Huzum.

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
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    [XmlRoot("custom_field")]
    public class CustomField : IdentifiableName, IEquatable<CustomField>
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [XmlText]
        public string Value { get; set; }

        [XmlAttribute("multiple")]
        public bool Multiple { get; set; }

        public override void ReadXml(XmlReader reader)
        {
            Id = reader.ReadAttributeAsInt("id");
            Name = reader.GetAttribute("name");
            Multiple = reader.ReadAttributeAsBoolean("multiple");

            try
            {
                Value = Multiple ? reader.ReadElementContentAsString() : reader.ReadElementString();
            }
            catch (Exception ex)
            {
                try
                {
                    Value = reader.ReadElementContentAsString();
                }
                catch (Exception ex2)
                {
                    Trace.TraceError("Redmine.NET: Could not load custom field value.");
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("id", Id.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("value", Value);
        }

        public bool Equals(CustomField other)
        {
            if (other == null) return false;
            return (Id == other.Id && Name == other.Name && Multiple == other.Multiple && Value == other.Value);
        }
    }
}