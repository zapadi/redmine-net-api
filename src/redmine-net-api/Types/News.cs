/*
   Copyright 2011 - 2023 Adrian Popescu

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
using Redmine.Net.Api.Serialization;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// Availability 1.1
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [XmlRoot(RedmineKeys.NEWS)]
    public sealed class News : Identifiable<News>
    {
        #region Properties
        /// <summary>
        /// Gets or sets the project.
        /// </summary>
        /// <value>The project.</value>
        public IdentifiableName Project { get; internal set; }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        /// <value>The author.</value>
        public IdentifiableName Author { get; internal set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; internal set; }

        /// <summary>
        /// Gets or sets the summary.
        /// </summary>
        /// <value>The summary.</value>
        public string Summary { get; internal set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; internal set; }

        /// <summary>
        /// Gets or sets the created on.
        /// </summary>
        /// <value>The created on.</value>
        public DateTime? CreatedOn { get; internal set; }
        
        /// <summary>
        /// 
        /// </summary>
        public List<Attachment> Attachments { get; internal set; }
        
        /// <summary>
        /// 
        /// </summary>
        public List<NewsComment> Comments { get; internal set; }
        
        /// <summary>
        /// 
        /// </summary>
        public List<Upload> Uploads { get; set; }
        
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
                    case RedmineKeys.AUTHOR: Author = new IdentifiableName(reader); break;
                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                    case RedmineKeys.DESCRIPTION: Description = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.PROJECT: Project = new IdentifiableName(reader); break;
                    case RedmineKeys.SUMMARY: Summary = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.TITLE: Title = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.ATTACHMENTS: Attachments = reader.ReadElementContentAsCollection<Attachment>();
                        break;
                    case RedmineKeys.COMMENTS: Comments = reader.ReadElementContentAsCollection<NewsComment>();
                        break;
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
            writer.WriteElementString(RedmineKeys.TITLE, Title);
            writer.WriteElementString(RedmineKeys.SUMMARY, Summary);
            writer.WriteElementString(RedmineKeys.DESCRIPTION, Description);
            if (Uploads != null)
            {
                writer.WriteArray(RedmineKeys.UPLOADS, Uploads);
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
                    case RedmineKeys.AUTHOR: Author = new IdentifiableName(reader); break;
                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadAsDateTime(); break;
                    case RedmineKeys.DESCRIPTION: Description = reader.ReadAsString(); break;
                    case RedmineKeys.PROJECT: Project = new IdentifiableName(reader); break;
                    case RedmineKeys.SUMMARY: Summary = reader.ReadAsString(); break;
                    case RedmineKeys.TITLE: Title = reader.ReadAsString(); break;
                    case RedmineKeys.ATTACHMENTS: Attachments = reader.ReadAsCollection<Attachment>();
                        break;
                    case RedmineKeys.COMMENTS: Comments = reader.ReadAsCollection<NewsComment>();
                        break;
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
            using (new JsonObject(writer, RedmineKeys.NEWS))
            {
                writer.WriteProperty(RedmineKeys.TITLE, Title);
                writer.WriteProperty(RedmineKeys.SUMMARY, Summary);
                writer.WriteProperty(RedmineKeys.DESCRIPTION, Description);
                if (Uploads != null)
                {
                    writer.WriteArray(RedmineKeys.UPLOADS, Uploads);
                }
            }
        }

        #endregion

        #region Implementation of IEquatable<News>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(News other)
        {
            if(other == null) return false;

            var result = base.Equals(other);
            result = result && Project == other.Project;
            result = result && Author == other.Author;
            result = result && string.Equals(Title, other.Title, StringComparison.Ordinal);
            result = result && string.Equals(Summary, other.Summary, StringComparison.Ordinal);
            result = result && string.Equals(Description, other.Description, StringComparison.Ordinal);
            result = result && CreatedOn == other.CreatedOn;
            result = result && (Attachments?.Equals<Attachment>(other.Attachments) ?? other.Attachments == null);
            result = result && (Comments?.Equals<NewsComment>(other.Comments) ?? other.Comments == null);
            result = result && (Uploads?.Equals<Upload>(other.Uploads) ?? other.Uploads == null);
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
            return Equals(obj as News);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 17;
                hashCode = HashCodeHelper.GetHashCode(Id, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Project, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Author, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Title, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Summary, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Description, hashCode);
                hashCode = HashCodeHelper.GetHashCode(CreatedOn, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Comments, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Attachments, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Uploads, hashCode);
                return hashCode;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(News left, News right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(News left, News right)
        {
            return !Equals(left, right);
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string DebuggerDisplay => $"[{nameof(News)}: {ToString()}, Project={Project}, Author={Author}, Title={Title}, Summary={Summary}, Description={Description}, CreatedOn={CreatedOn?.ToString("u", CultureInfo.InvariantCulture)}]";

    }
}