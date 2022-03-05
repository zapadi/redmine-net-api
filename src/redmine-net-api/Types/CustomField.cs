/*
   Copyright 2011 - 2022 Adrian Popescu

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
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [XmlRoot(RedmineKeys.CUSTOM_FIELD)]
    public sealed class CustomField : IdentifiableName, IEquatable<CustomField>
    {
        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public string CustomizedType { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public string FieldFormat { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public string Regexp { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public int? MinLength { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public int? MaxLength { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsRequired { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsFilter { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Searchable { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Multiple { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public string DefaultValue { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Visible { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public IList<CustomFieldPossibleValue> PossibleValues { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public IList<TrackerCustomField> Trackers { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public IList<CustomFieldRole> Roles { get; internal set; }
        #endregion

        #region Implementation of IXmlSerializable 
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
                    case RedmineKeys.CUSTOMIZED_TYPE: CustomizedType = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.DEFAULT_VALUE: DefaultValue = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.FIELD_FORMAT: FieldFormat = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.IS_FILTER: IsFilter = reader.ReadElementContentAsBoolean(); break;
                    case RedmineKeys.IS_REQUIRED: IsRequired = reader.ReadElementContentAsBoolean(); break;
                    case RedmineKeys.MAX_LENGTH: MaxLength = reader.ReadElementContentAsNullableInt(); break;
                    case RedmineKeys.MIN_LENGTH: MinLength = reader.ReadElementContentAsNullableInt(); break;
                    case RedmineKeys.MULTIPLE: Multiple = reader.ReadElementContentAsBoolean(); break;
                    case RedmineKeys.NAME: Name = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.POSSIBLE_VALUES: PossibleValues = reader.ReadElementContentAsCollection<CustomFieldPossibleValue>(); break;
                    case RedmineKeys.REGEXP: Regexp = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.ROLES: Roles = reader.ReadElementContentAsCollection<CustomFieldRole>(); break;
                    case RedmineKeys.SEARCHABLE: Searchable = reader.ReadElementContentAsBoolean(); break;
                    case RedmineKeys.TRACKERS: Trackers = reader.ReadElementContentAsCollection<TrackerCustomField>(); break;
                    case RedmineKeys.VISIBLE: Visible = reader.ReadElementContentAsBoolean(); break;
                    default: reader.Read(); break;
                }
            }
        }

        #endregion

        #region Implementation of IJsonSerialization
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public override void ReadJson(JsonReader reader)
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndObject)
                {
                    return;
                }

                if (reader.TokenType != JsonToken.PropertyName)
                {
                    continue;
                }

                switch (reader.Value)
                {
                    case RedmineKeys.ID: Id = reader.ReadAsInt(); break;
                    case RedmineKeys.CUSTOMIZED_TYPE: CustomizedType = reader.ReadAsString(); break;
                    case RedmineKeys.DEFAULT_VALUE: DefaultValue = reader.ReadAsString(); break;
                    case RedmineKeys.FIELD_FORMAT: FieldFormat = reader.ReadAsString(); break;
                    case RedmineKeys.IS_FILTER: IsFilter = reader.ReadAsBool(); break;
                    case RedmineKeys.IS_REQUIRED: IsRequired = reader.ReadAsBool(); break;
                    case RedmineKeys.MAX_LENGTH: MaxLength = reader.ReadAsInt32(); break;
                    case RedmineKeys.MIN_LENGTH: MinLength = reader.ReadAsInt32(); break;
                    case RedmineKeys.MULTIPLE: Multiple = reader.ReadAsBool(); break;
                    case RedmineKeys.NAME: Name = reader.ReadAsString(); break;
                    case RedmineKeys.POSSIBLE_VALUES: PossibleValues = reader.ReadAsCollection<CustomFieldPossibleValue>(); break;
                    case RedmineKeys.REGEXP: Regexp = reader.ReadAsString(); break;
                    case RedmineKeys.ROLES: Roles = reader.ReadAsCollection<CustomFieldRole>(); break;
                    case RedmineKeys.SEARCHABLE: Searchable = reader.ReadAsBool(); break;
                    case RedmineKeys.TRACKERS: Trackers = reader.ReadAsCollection<TrackerCustomField>(); break;
                    case RedmineKeys.VISIBLE: Visible = reader.ReadAsBool(); break;
                    default: reader.Read(); break;
                }
            }
        }

        #endregion

        #region Implementation of IEquatable<CustomField>
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
                && string.Equals(CustomizedType,other.CustomizedType, StringComparison.OrdinalIgnoreCase)
                && string.Equals(DefaultValue,other.DefaultValue, StringComparison.OrdinalIgnoreCase)
                && string.Equals(FieldFormat,other.FieldFormat, StringComparison.OrdinalIgnoreCase)
                && MaxLength == other.MaxLength
                && MinLength == other.MinLength
                && string.Equals(Name,other.Name, StringComparison.OrdinalIgnoreCase)
                && string.Equals(Regexp,other.Regexp, StringComparison.OrdinalIgnoreCase)
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
                hashCode = HashCodeHelper.GetHashCode(Id, hashCode);
                hashCode = HashCodeHelper.GetHashCode(IsFilter, hashCode);
                hashCode = HashCodeHelper.GetHashCode(IsRequired, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Multiple, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Searchable, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Visible, hashCode);
                hashCode = HashCodeHelper.GetHashCode(CustomizedType, hashCode);
                hashCode = HashCodeHelper.GetHashCode(DefaultValue, hashCode);
                hashCode = HashCodeHelper.GetHashCode(FieldFormat, hashCode);
                hashCode = HashCodeHelper.GetHashCode(MaxLength, hashCode);
                hashCode = HashCodeHelper.GetHashCode(MinLength, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Name, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Regexp, hashCode);
                hashCode = HashCodeHelper.GetHashCode(PossibleValues, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Roles, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Trackers, hashCode);
                return hashCode;
            }
        }
        #endregion

        private string DebuggerDisplay =>
            $@"[{nameof(CustomField)}: {ToString()}
, CustomizedType={CustomizedType}
, FieldFormat={FieldFormat}
, Regexp={Regexp}
, MinLength={MinLength?.ToString(CultureInfo.InvariantCulture)}
, MaxLength={MaxLength?.ToString(CultureInfo.InvariantCulture)}
, IsRequired={IsRequired.ToString(CultureInfo.InvariantCulture)}
, IsFilter={IsFilter.ToString(CultureInfo.InvariantCulture)}
, Searchable={Searchable.ToString(CultureInfo.InvariantCulture)}
, Multiple={Multiple.ToString(CultureInfo.InvariantCulture)}
, DefaultValue={DefaultValue}
, Visible={Visible.ToString(CultureInfo.InvariantCulture)}
, PossibleValues={PossibleValues.Dump()}
, Trackers={Trackers.Dump()}
, Roles={Roles.Dump()}]";
    }
}