/*
   Copyright 2011 - 2025 Adrian Popescu

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
    [DebuggerDisplay($"{{{nameof(DebuggerDisplay)},nq}}")]
    [XmlRoot(RedmineKeys.CUSTOM_FIELD)]
    public sealed class CustomField : IdentifiableName, IEquatable<CustomField>
    {
        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public string CustomizedType { get; internal set; }
        
        /// <summary>
        /// Added in Redmine 5.1.0 version
        /// </summary>
        public string Description { get; internal set; }

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
                    case RedmineKeys.DESCRIPTION: Description = reader.ReadElementContentAsString(); break;
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
                    case RedmineKeys.DESCRIPTION: Description = reader.ReadAsString(); break;
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

            var result = base.Equals(other)
                    && string.Equals(CustomizedType, other.CustomizedType, StringComparison.Ordinal)
                    && string.Equals(Description, other.Description, StringComparison.Ordinal)
                    && string.Equals(FieldFormat, other.FieldFormat, StringComparison.Ordinal)
                    && string.Equals(Regexp, other.Regexp, StringComparison.Ordinal)
                    && string.Equals(DefaultValue, other.DefaultValue, StringComparison.Ordinal)
                    && MinLength == other.MinLength
                    && MaxLength == other.MaxLength
                    && IsRequired == other.IsRequired
                    && IsFilter == other.IsFilter
                    && Searchable == other.Searchable
                    && Multiple == other.Multiple
                    && Visible == other.Visible
                    && (PossibleValues?.Equals<CustomFieldPossibleValue>(other.PossibleValues) ?? other.PossibleValues == null)
                    && (Trackers?.Equals<TrackerCustomField>(other.Trackers) ?? other.Trackers == null)
                    && (Roles?.Equals<CustomFieldRole>(other.Roles) ?? other.Roles == null);
            return result;
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
                var hashCode = base.GetHashCode();
                hashCode = HashCodeHelper.GetHashCode(CustomizedType, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Description, hashCode);
                hashCode = HashCodeHelper.GetHashCode(FieldFormat, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Regexp, hashCode);
                hashCode = HashCodeHelper.GetHashCode(MinLength, hashCode);
                hashCode = HashCodeHelper.GetHashCode(MaxLength, hashCode);
                hashCode = HashCodeHelper.GetHashCode(IsRequired, hashCode);
                hashCode = HashCodeHelper.GetHashCode(IsFilter, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Searchable, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Multiple, hashCode);
                hashCode = HashCodeHelper.GetHashCode(DefaultValue, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Visible, hashCode);
                hashCode = HashCodeHelper.GetHashCode(PossibleValues, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Trackers, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Roles, hashCode);
                return hashCode;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(CustomField left, CustomField right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(CustomField left, CustomField right)
        {
            return !Equals(left, right);
        }
        #endregion

        private string DebuggerDisplay => $"[CustomField: Id={Id.ToInvariantString()}, Name={Name}]";
    }
}