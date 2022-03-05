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
    /// Availability 1.3
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [XmlRoot(RedmineKeys.ISSUE_STATUS)]
    public sealed class IssueStatus : IdentifiableName, IEquatable<IssueStatus>
    {
        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether IssueStatus is default.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if IssueStatus is default; otherwise, <c>false</c>.
        /// </value>
        public bool IsDefault { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether IssueStatus is closed.
        /// </summary>
        /// <value><c>true</c> if IssueStatus is closed; otherwise, <c>false</c>.</value>
        public bool IsClosed { get; internal set; }
        #endregion

        #region Implementation of IXmlSerialization
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
                    case RedmineKeys.IS_CLOSED: IsClosed = reader.ReadElementContentAsBoolean(); break;
                    case RedmineKeys.IS_DEFAULT: IsDefault = reader.ReadElementContentAsBoolean(); break;
                    case RedmineKeys.NAME: Name = reader.ReadElementContentAsString(); break;
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
                    case RedmineKeys.IS_CLOSED: IsClosed = reader.ReadAsBool(); break;
                    case RedmineKeys.IS_DEFAULT: IsDefault = reader.ReadAsBool(); break;
                    case RedmineKeys.NAME: Name = reader.ReadAsString(); break;
                    default: reader.Read(); break;
                }
            }
        }
        #endregion

        #region Implementation of IEquatable<IssueStatus>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IssueStatus other)
        {
            if (other == null) return false;
            return Id == other.Id && Name == other.Name && IsClosed == other.IsClosed && IsDefault == other.IsDefault;
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
            return Equals(obj as IssueStatus);
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
                hashCode = HashCodeHelper.GetHashCode(Name, hashCode);
                hashCode = HashCodeHelper.GetHashCode(IsClosed, hashCode);
                hashCode = HashCodeHelper.GetHashCode(IsDefault, hashCode);
                return hashCode;
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string DebuggerDisplay => $"[{nameof(IssueStatus)}: {ToString()}, IsDefault={IsDefault.ToString(CultureInfo.InvariantCulture)}, IsClosed={IsClosed.ToString(CultureInfo.InvariantCulture)}]";

    }
}