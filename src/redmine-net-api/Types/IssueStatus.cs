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
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Serialization.Json.Extensions;
using Redmine.Net.Api.Serialization.Xml.Extensions;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// Availability 1.3
    /// </summary>
    [DebuggerDisplay($"{{{nameof(DebuggerDisplay)},nq}}")]
    [XmlRoot(RedmineKeys.ISSUE_STATUS)]
    public sealed class IssueStatus : IdentifiableName, IEquatable<IssueStatus>, ICloneable<IssueStatus>
    {
        /// <summary>
        /// 
        /// </summary>
        public IssueStatus()
        {
            
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public IssueStatus(int id)
        {
            Id = id;
        }
        
        /// <summary>
        /// 
        /// </summary>
        internal IssueStatus(int id, string name, bool isDefault = false, bool isClosed = false)
        {
            Id = id;
            Name = name;
            IsClosed = isClosed;
            IsDefault = isDefault;
        }
        
        internal IssueStatus(XmlReader reader)
        {
            Initialize(reader);
        }
        
        internal IssueStatus(JsonReader reader)
        {
            Initialize(reader);
        }

        private void Initialize(XmlReader reader)
        {
            ReadXml(reader);
        }

        private void Initialize(JsonReader reader)
        {
            ReadJson(reader);
        }
        
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
            if (reader.HasAttributes && reader.Name == "status")
            {
                Id = reader.ReadAttributeAsInt(RedmineKeys.ID);
                IsClosed = reader.ReadAttributeAsBoolean(RedmineKeys.IS_CLOSED);
                IsDefault = reader.ReadAttributeAsBoolean(RedmineKeys.IS_DEFAULT);
                Name = reader.GetAttribute(RedmineKeys.NAME);
                reader.Read();
                return;
            }
            
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
            return base.Equals(other)
                   && IsClosed == other.IsClosed 
                   && IsDefault == other.IsDefault;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resetId"></param>
        /// <returns></returns>
        public new IssueStatus Clone(bool resetId)
        {
            return new IssueStatus
            {
                Id = Id,
                Name = Name,
                IsClosed = IsClosed,
                IsDefault = IsDefault
            };
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
                var hashCode = base.GetHashCode();
                hashCode = HashCodeHelper.GetHashCode(IsClosed, hashCode);
                hashCode = HashCodeHelper.GetHashCode(IsDefault, hashCode);
                return hashCode;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(IssueStatus left, IssueStatus right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(IssueStatus left, IssueStatus right)
        {
            return !Equals(left, right);
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string DebuggerDisplay => $"[IssueStatus: Id={Id.ToInvariantString()}, Name={Name}, IsDefault={IsDefault.ToInvariantString()}, IsClosed={IsClosed.ToInvariantString()}]";

    }
}