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
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Serialization;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// Availability 2.1
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [XmlRoot(RedmineKeys.GROUP)]
    public sealed class Group : IdentifiableName, IEquatable<Group>
    {
        /// <summary>
        /// 
        /// </summary>
        public Group() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public Group(string name)
        {
            Name = name;
        }

        #region Properties
        /// <summary>
        /// Represents the group's users.
        /// </summary>
        public IList<GroupUser> Users { get;  set; }

        /// <summary>
        /// Gets or sets the custom fields.
        /// </summary>
        /// <value>The custom fields.</value>
        public IList<IssueCustomField> CustomFields { get; internal set; }

        /// <summary>
        /// Gets or sets the custom fields.
        /// </summary>
        /// <value>The custom fields.</value>
        public IList<Membership> Memberships { get; internal set; }
        #endregion

        #region Implementation of IXmlSerializable

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="System.Xml.XmlReader"/> stream from which the object is deserialized. </param>
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
                    case RedmineKeys.CUSTOM_FIELDS: CustomFields = reader.ReadElementContentAsCollection<IssueCustomField>(); break;
                    case RedmineKeys.MEMBERSHIPS: Memberships = reader.ReadElementContentAsCollection<Membership>(); break;
                    case RedmineKeys.NAME: Name = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.USERS: Users = reader.ReadElementContentAsCollection<GroupUser>(); break;
                    default: reader.Read(); break;
                }
            }
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="System.Xml.XmlWriter"/> stream to which the object is serialized. </param>
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString(RedmineKeys.NAME, Name);
            writer.WriteArrayIds(RedmineKeys.USER_IDS, Users, typeof(int), GetGroupUserId);
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
                    case RedmineKeys.CUSTOM_FIELDS: CustomFields = reader.ReadAsCollection<IssueCustomField>(); break;
                    case RedmineKeys.MEMBERSHIPS: Memberships = reader.ReadAsCollection<Membership>(); break;
                    case RedmineKeys.NAME: Name = reader.ReadAsString(); break;
                    case RedmineKeys.USERS: Users = reader.ReadAsCollection<GroupUser>(); break;
                    default: reader.Read(); break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteJson(JsonWriter writer)
        {
            using (new JsonObject(writer, RedmineKeys.GROUP))
            {
                writer.WriteProperty(RedmineKeys.NAME, Name);
                writer.WriteRepeatableElement(RedmineKeys.USER_IDS, (IEnumerable<IValue>)Users);
            }
        }
        #endregion

        #region Implementation of IEquatable<Group>

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Group other)
        {
            if (other == null) return false;
            return Id == other.Id
                && Name == other.Name
                && (Users != null ? Users.Equals<GroupUser>(other.Users) : other.Users == null)
                && (CustomFields != null ? CustomFields.Equals<IssueCustomField>(other.CustomFields) : other.CustomFields == null)
                && (Memberships != null ? Memberships.Equals<Membership>(other.Memberships) : other.Memberships == null);
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
            return Equals(obj as Group);
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
                hashCode = HashCodeHelper.GetHashCode(Users, hashCode);
                hashCode = HashCodeHelper.GetHashCode(CustomFields, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Memberships, hashCode);
                return hashCode;
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string DebuggerDisplay => $"[{nameof(Group)}: {ToString()}, Users={Users.Dump()}, CustomFields={CustomFields.Dump()}, Memberships={Memberships.Dump()}]";


        /// <summary>
        /// 
        /// </summary>
        /// <param name="gu"></param>
        /// <returns></returns>
        public static int GetGroupUserId(object gu)
        {
            return ((GroupUser)gu).Id;
        }
    }
}