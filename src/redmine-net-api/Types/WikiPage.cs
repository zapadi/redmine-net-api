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
using Redmine.Net.Api.Serialization;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// Availability 2.2
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [XmlRoot(RedmineKeys.WIKI_PAGE)]
    public sealed class WikiPage : Identifiable<WikiPage>
    {
        #region Properties
        /// <summary>
        /// Gets the title.
        /// </summary>
        public string Title { get; internal set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the comments
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets the version
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Gets the author.
        /// </summary>
        public IdentifiableName Author { get; internal set; }

        /// <summary>
        /// Gets the created on.
        /// </summary>
        /// <value>The created on.</value>
        public DateTime? CreatedOn { get; internal set; }

        /// <summary>
        /// Gets or sets the updated on.
        /// </summary>
        /// <value>The updated on.</value>
        public DateTime? UpdatedOn { get; internal set; }

        /// <summary>
        /// Gets the attachments.
        /// </summary>
        /// <value>
        /// The attachments.
        /// </value>
        public IList<Attachment> Attachments { get; set; }

        /// <summary>
        /// Sets the uploads.
        /// </summary>
        /// <value>
        /// The uploads.
        /// </value>
        /// <remarks>Availability starting with redmine version 3.3</remarks>
        public IList<Upload> Uploads { get; set; }
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
                    case RedmineKeys.ATTACHMENTS: Attachments = reader.ReadElementContentAsCollection<Attachment>(); break;
                    case RedmineKeys.AUTHOR: Author = new IdentifiableName(reader); break;
                    case RedmineKeys.COMMENTS: Comments = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                    case RedmineKeys.TEXT: Text = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.TITLE: Title = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.UPDATED_ON: UpdatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                    case RedmineKeys.VERSION: Version = reader.ReadElementContentAsInt(); break;
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
            writer.WriteElementString(RedmineKeys.TEXT, Text);
            writer.WriteElementString(RedmineKeys.COMMENTS, Comments);
            writer.WriteValueOrEmpty<int>(RedmineKeys.VERSION, Version);
            writer.WriteArray(RedmineKeys.UPLOADS, Uploads);
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
                    case RedmineKeys.ATTACHMENTS: Attachments = reader.ReadAsCollection<Attachment>(); break;
                    case RedmineKeys.AUTHOR: Author = new IdentifiableName(reader); break;
                    case RedmineKeys.COMMENTS: Comments = reader.ReadAsString(); break;
                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadAsDateTime(); break;
                    case RedmineKeys.TEXT: Text = reader.ReadAsString(); break;
                    case RedmineKeys.TITLE: Title = reader.ReadAsString(); break;
                    case RedmineKeys.UPDATED_ON: UpdatedOn = reader.ReadAsDateTime(); break;
                    case RedmineKeys.VERSION: Version = reader.ReadAsInt(); break;
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
            using (new JsonObject(writer, RedmineKeys.WIKI_PAGE))
            {
                writer.WriteProperty(RedmineKeys.TEXT, Text);
                writer.WriteProperty(RedmineKeys.COMMENTS, Comments);
                writer.WriteValueOrEmpty<int>(RedmineKeys.VERSION, Version);
                writer.WriteArray(RedmineKeys.UPLOADS, Uploads);
            }
        }
        #endregion

        #region Implementation of IEquatable<WikiPage>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(WikiPage other)
        {
            if (other == null) return false;

            return Id == other.Id
                && Title == other.Title
                && Text == other.Text
                && Comments == other.Comments
                && Version == other.Version
                && Author == other.Author
                && CreatedOn == other.CreatedOn
                && UpdatedOn == other.UpdatedOn;
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
                hashCode = HashCodeHelper.GetHashCode(Title, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Text, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Comments, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Version, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Author, hashCode);
                hashCode = HashCodeHelper.GetHashCode(CreatedOn, hashCode);
                hashCode = HashCodeHelper.GetHashCode(UpdatedOn, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Attachments, hashCode);
                return hashCode;
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string DebuggerDisplay => $@"[{nameof(WikiPage)}: {ToString()}, Title={Title}, Text={Text}, Comments={Comments}, 
Version={Version.ToString(CultureInfo.InvariantCulture)}, 
Author={Author}, 
CreatedOn={CreatedOn?.ToString("u", CultureInfo.InvariantCulture)}, 
UpdatedOn={UpdatedOn?.ToString("u", CultureInfo.InvariantCulture)}, 
Attachments={Attachments.Dump()}]";

    }
}