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
    [XmlRoot("custom_field")]
    public class CustomField : IXmlSerializable, IEquatable<CustomField>
    {
        [XmlElement("id")]
        public int Id { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("customized_type")]
        public string CustomizedType { get; set; }

        [XmlElement("field_format")]
        public string FieldFormat { get; set; }

        [XmlElement("regexp")]
        public string Regexp { get; set; }

        [XmlElement("min_length")]
        public int? MinLength { get; set; }

        [XmlElement("max_length")]
        public int? MaxLength { get; set; }

        [XmlElement("is_required")]
        public bool IsRequired { get; set; }

        [XmlElement("is_filter")]
        public bool IsFilter { get; set; }

        [XmlElement("searchable")]
        public bool Searchable { get; set; }

        [XmlElement("multiple")]
        public bool Multiple { get; set; }

        [XmlElement("default_value")]
        public string DefaultValue { get; set; }

        [XmlElement("visible")]
        public bool Visible { get; set; }

        [XmlArray("possible_values")]
        [XmlArrayItem("possible_value")]
        public IList<CustomFieldPossibleValue> PossibleValues { get; set; }

        [XmlArray("trackers")]
        [XmlArrayItem("tracker")]
        public IList<TrackerCustomField> Trackers { get; set; }

        [XmlArray("roles")]
        [XmlArrayItem("role")]
        public IList<Role> Roles { get; set; }

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

                    case "name": Name = reader.ReadElementContentAsString(); break;

                    case "customized_type": CustomizedType = reader.ReadElementContentAsString(); break;

                    case "field_format": FieldFormat = reader.ReadElementContentAsString(); break;

                    case "regexp": Regexp = reader.ReadElementContentAsString(); break;

                    case "min_length": MinLength = reader.ReadElementContentAsNullableInt(); break;

                    case "max_length": MaxLength = reader.ReadElementContentAsNullableInt(); break;

                    case "is_required": IsRequired = reader.ReadElementContentAsBoolean(); break;

                    case "is_filter": IsFilter = reader.ReadElementContentAsBoolean(); break;

                    case "searchable": Searchable = reader.ReadElementContentAsBoolean(); break;

                    case "visible": Visible = reader.ReadElementContentAsBoolean(); break;

                    case "default_value": DefaultValue = reader.ReadElementContentAsString(); break;

                    case "multiple": Multiple = reader.ReadElementContentAsBoolean(); break;

                    case "trackers":
                        Trackers = reader.ReadElementContentAsCollection<TrackerCustomField>();
                        break;

                    case "roles":
                        Roles = reader.ReadElementContentAsCollection<Role>();
                        break;
                    case "possible_values": PossibleValues = reader.ReadElementContentAsCollection<CustomFieldPossibleValue>(); break;

                    default: reader.Read(); break;
                }
            }
        }

        public void WriteXml(XmlWriter writer) { }

        public bool Equals(CustomField other)
        {
            if (other == null) return false;
            return (Id == other.Id && Name == other.Name && Multiple == other.Multiple && IsFilter == other.IsFilter && IsRequired == other.IsRequired
                && Searchable == other.Searchable && Visible == other.Visible
                && CustomizedType == other.CustomizedType && DefaultValue == other.DefaultValue
                && FieldFormat == other.FieldFormat && MaxLength == other.MaxLength && MinLength == other.MinLength && Regexp == other.Regexp
                && Equals(Roles, other.Roles)
                && Equals(Trackers, other.Trackers)
                && Equals(PossibleValues, other.PossibleValues));
        }
    }
}