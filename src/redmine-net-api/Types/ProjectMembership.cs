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
    /// Availability 1.4
    /// </summary>
    /// <remarks>
    /// POST - Adds a project member.
    /// GET - Returns the membership of given :id.
    /// PUT - Updates the membership of given :id. Only the roles can be updated, the project and the user of a membership are read-only.
    /// DELETE - Deletes a memberships. Memberships inherited from a group membership can not be deleted. You must delete the group membership.
    /// </remarks>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [XmlRoot(RedmineKeys.MEMBERSHIP)]
    public sealed class ProjectMembership : Identifiable<ProjectMembership>
    {
        #region Properties
        /// <summary>
        /// Gets or sets the project.
        /// </summary>
        /// <value>The project.</value>
        public IdentifiableName Project { get; internal set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        public IdentifiableName User { get; set; }

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        /// <value>
        /// The group.
        /// </value>
        public IdentifiableName Group { get; internal set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public IList<MembershipRole> Roles { get; set; }
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
                    case RedmineKeys.GROUP: Group = new IdentifiableName(reader); break;
                    case RedmineKeys.PROJECT: Project = new IdentifiableName(reader); break;
                    case RedmineKeys.ROLES: Roles = reader.ReadElementContentAsCollection<MembershipRole>(); break;
                    case RedmineKeys.USER: User = new IdentifiableName(reader); break;
                    default: reader.Read(); break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteIdIfNotNull(RedmineKeys.USER_ID, User);
            writer.WriteArray(RedmineKeys.ROLE_IDS, Roles, typeof(MembershipRole), RedmineKeys.ROLE_ID);
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
                    case RedmineKeys.GROUP: Group = new IdentifiableName(reader); break;
                    case RedmineKeys.PROJECT: Project = new IdentifiableName(reader); break;
                    case RedmineKeys.ROLES: Roles = reader.ReadAsCollection<MembershipRole>(); break;
                    case RedmineKeys.USER: User = new IdentifiableName(reader); break;
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
            using (new JsonObject(writer, RedmineKeys.MEMBERSHIP))
            {
                writer.WriteIdIfNotNull(RedmineKeys.USER_ID, User);
                writer.WriteRepeatableElement(RedmineKeys.ROLE_IDS, (IEnumerable<IValue>)Roles);
            }
        }
        #endregion

        #region Implementation of IEquatable<ProjectMembership>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(ProjectMembership other)
        {
            if (other == null) return false;
            return Id == other.Id
                && Project.Equals(other.Project)
                && Roles.Equals<MembershipRole>(other.Roles)
                && (User != null ? User.Equals(other.User) : other.User == null)
                && (Group != null ? Group.Equals(other.Group) : other.Group == null);
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
                hashCode = HashCodeHelper.GetHashCode(Project, hashCode);
                hashCode = HashCodeHelper.GetHashCode(User, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Group, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Roles, hashCode);
                return hashCode;
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string DebuggerDisplay => $"[{nameof(ProjectMembership)}: {ToString()}, Project={Project}, User={User}, Group={Group}, Roles={Roles.Dump()}]";

    }
}