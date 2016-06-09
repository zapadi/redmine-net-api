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
    public class CustomField : IdentifiableName, IEquatable<CustomField>
    {
        /// <summary>
        /// 
        /// </summary>
        [XmlElement(RedmineKeys.CUSTOMIZED_TYPE)]
        public string CustomizedType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(RedmineKeys.FIELD_FORMAT)]
        public string FieldFormat { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(RedmineKeys.REGEXP)]
        public string Regexp { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(RedmineKeys.MIN_LENGTH)]
        public int? MinLength { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(RedmineKeys.MAX_LENGTH)]
        public int? MaxLength { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(RedmineKeys.IS_REQUIRED)]
        public bool IsRequired { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(RedmineKeys.IS_FILTER)]
        public bool IsFilter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(RedmineKeys.SEARCHABLE)]
        public bool Searchable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(RedmineKeys.MULTIPLE)]
        public bool Multiple { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(RedmineKeys.DEFAULT_VALUE)]
        public string DefaultValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement(RedmineKeys.VISIBLE)]
        public bool Visible { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlArray(RedmineKeys.POSSIBLE_VALUES)]
        [XmlArrayItem(RedmineKeys.POSSIBLE_VALUE)]
        public IList<CustomFieldPossibleValue> PossibleValues { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlArray(RedmineKeys.TRACKERS)]
        [XmlArrayItem(RedmineKeys.TRACKER)]
        public IList<TrackerCustomField> Trackers { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlArray(RedmineKeys.ROLES)]
        [XmlArrayItem(RedmineKeys.ROLE)]
        public IList<CustomFieldRole> Roles { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public override void ReadXml(XmlReader reader)
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

                    case RedmineKeys.CUSTOMIZED_TYPE: CustomizedType = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.FIELD_FORMAT: FieldFormat = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.REGEXP: Regexp = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.MIN_LENGTH: MinLength = reader.ReadElementContentAsNullableInt(); break;

                    case RedmineKeys.MAX_LENGTH: MaxLength = reader.ReadElementContentAsNullableInt(); break;

                    case RedmineKeys.IS_REQUIRED: IsRequired = reader.ReadElementContentAsBoolean(); break;

                    case RedmineKeys.IS_FILTER: IsFilter = reader.ReadElementContentAsBoolean(); break;

                    case RedmineKeys.SEARCHABLE: Searchable = reader.ReadElementContentAsBoolean(); break;

                    case RedmineKeys.VISIBLE: Visible = reader.ReadElementContentAsBoolean(); break;

                    case RedmineKeys.DEFAULT_VALUE: DefaultValue = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.MULTIPLE: Multiple = reader.ReadElementContentAsBoolean(); break;

                    case RedmineKeys.TRACKERS: Trackers = reader.ReadElementContentAsCollection<TrackerCustomField>(); break;

                    case RedmineKeys.ROLES: Roles = reader.ReadElementContentAsCollection<CustomFieldRole>(); break;

                    case RedmineKeys.POSSIBLE_VALUES: PossibleValues = reader.ReadElementContentAsCollection<CustomFieldPossibleValue>(); break;

                    default: reader.Read(); break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteXml(XmlWriter writer) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(CustomField other)
        {
            if (other == null) return false;

            return Id == other.Id
                && IsFilter == other.IsFilter
                && IsRequired == other.IsRequired
                && Multiple == other.Multiple
                && Searchable == other.Searchable
                && Visible == other.Visible
                && CustomizedType.Equals(other.CustomizedType)
                && DefaultValue.Equals(other.DefaultValue)
                && FieldFormat.Equals(other.FieldFormat)
                && MaxLength == other.MaxLength
                && MinLength == other.MinLength
                && Name.Equals(other.Name)
                && Regexp.Equals(other.Regexp)
                && PossibleValues.Equals(other.PossibleValues)
                && Roles.Equals(other.Roles)
                && Trackers.Equals(other.Trackers);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as CustomField);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 13;
                hashCode = HashCodeHelper.GetHashCode(Id,hashCode);
				hashCode = HashCodeHelper.GetHashCode(IsFilter,hashCode);
				hashCode = HashCodeHelper.GetHashCode(IsRequired,hashCode);
				hashCode = HashCodeHelper.GetHashCode(Multiple,hashCode);
				hashCode = HashCodeHelper.GetHashCode(Searchable,hashCode);
				hashCode = HashCodeHelper.GetHashCode(Visible,hashCode);
				hashCode = HashCodeHelper.GetHashCode(CustomizedType,hashCode);
				hashCode = HashCodeHelper.GetHashCode(DefaultValue,hashCode);
				hashCode = HashCodeHelper.GetHashCode(FieldFormat,hashCode);
				hashCode = HashCodeHelper.GetHashCode(MaxLength,hashCode);
				hashCode = HashCodeHelper.GetHashCode(MinLength,hashCode);
				hashCode = HashCodeHelper.GetHashCode(Name,hashCode);
				hashCode = HashCodeHelper.GetHashCode(Regexp,hashCode);
				hashCode = HashCodeHelper.GetHashCode(PossibleValues,hashCode);
				hashCode = HashCodeHelper.GetHashCode(Roles,hashCode);
				hashCode = HashCodeHelper.GetHashCode(Trackers,hashCode);
                return hashCode;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public override string ToString ()
		{
			return string.Format ("[CustomField: Id={0}, Name={1}, CustomizedType={2}, FieldFormat={3}, Regexp={4}, MinLength={5}, MaxLength={6}, IsRequired={7}, IsFilter={8}, Searchable={9}, Multiple={10}, DefaultValue={11}, Visible={12}, PossibleValues={13}, Trackers={14}, Roles={15}]",
				Id, Name, CustomizedType, FieldFormat, Regexp, MinLength, MaxLength, IsRequired, IsFilter, Searchable, Multiple, DefaultValue, Visible, PossibleValues, Trackers, Roles);
		}
    }
}