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
    /// Availability 1.3
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [XmlRoot(RedmineKeys.ISSUE_CATEGORY)]
    public sealed class IssueCategory : Identifiable<IssueCategory>
    {
        #region Properties
        /// <summary>
        /// Gets or sets the project.
        /// </summary>
        /// <value>
        /// The project.
        /// </value>
        public IdentifiableName Project { get; set; }

        /// <summary>
        /// Gets or sets the asign to.
        /// </summary>
        /// <value>
        /// The asign to.
        /// </value>
        public IdentifiableName AssignTo { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
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
                    case RedmineKeys.ASSIGNED_TO: AssignTo = new IdentifiableName(reader); break;
                    case RedmineKeys.NAME: Name = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.PROJECT: Project = new IdentifiableName(reader); break;
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
            writer.WriteIdIfNotNull(RedmineKeys.PROJECT_ID, Project);
            writer.WriteElementString(RedmineKeys.NAME, Name);
            writer.WriteIdIfNotNull(RedmineKeys.ASSIGNED_TO_ID, AssignTo);
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
                    case RedmineKeys.ASSIGNED_TO: AssignTo = new IdentifiableName(reader); break;
                    case RedmineKeys.NAME: Name = reader.ReadAsString(); break;
                    case RedmineKeys.PROJECT: Project = new IdentifiableName(reader); break;
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
            using (new JsonObject(writer, RedmineKeys.ISSUE_CATEGORY))
            {
                writer.WriteIdIfNotNull(RedmineKeys.PROJECT_ID, Project);
                writer.WriteProperty(RedmineKeys.NAME, Name);
                writer.WriteIdIfNotNull(RedmineKeys.ASSIGNED_TO_ID, AssignTo);
            }
        }
        #endregion

        #region Implementation of IEquatable<IssueCategory>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(IssueCategory other)
        {
            if (other == null) return false;
            return Id == other.Id && Project == other.Project && AssignTo == other.AssignTo && Name == other.Name;
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
                hashCode = HashCodeHelper.GetHashCode(Project, hashCode);
                hashCode = HashCodeHelper.GetHashCode(AssignTo, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Name, hashCode);
                return hashCode;
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string DebuggerDisplay => $"[{nameof(IssueCategory)}: {ToString()}, Project={Project}, AssignTo={AssignTo}, Name={Name}]";

    }
}