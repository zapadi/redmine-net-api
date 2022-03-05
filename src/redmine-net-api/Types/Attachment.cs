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
using Redmine.Net.Api.Serialization;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// Availability 1.3
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [XmlRoot(RedmineKeys.ATTACHMENT)]
    public sealed class Attachment : Identifiable<Attachment>
    {
        #region Properties
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets the size of the file.
        /// </summary>
        /// <value>The size of the file.</value>
        public int FileSize { get; internal set; }

        /// <summary>
        /// Gets the type of the content.
        /// </summary>
        /// <value>The type of the content.</value>
        public string ContentType { get; internal set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets the content URL.
        /// </summary>
        /// <value>The content URL.</value>
        public string ContentUrl { get; internal set; }

        /// <summary>
        /// Gets the author.
        /// </summary>
        /// <value>The author.</value>
        public IdentifiableName Author { get; internal set; }

        /// <summary>
        /// Gets the created on.
        /// </summary>
        /// <value>The created on.</value>
        public DateTime? CreatedOn { get; internal set; }

        /// <summary>
        /// Gets the thumbnail url.
        /// </summary>
        public string ThumbnailUrl { get; internal set; }
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
                    case RedmineKeys.AUTHOR: Author = new IdentifiableName(reader); break;
                    case RedmineKeys.CONTENT_TYPE: ContentType = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.CONTENT_URL: ContentUrl = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                    case RedmineKeys.DESCRIPTION: Description = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.FILE_NAME: FileName = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.FILE_SIZE: FileSize = reader.ReadElementContentAsInt(); break;
                    case RedmineKeys.THUMBNAIL_URL: ThumbnailUrl = reader.ReadElementContentAsString(); break;
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
            writer.WriteElementString(RedmineKeys.DESCRIPTION, Description);
            writer.WriteElementString(RedmineKeys.FILE_NAME, FileName);
        }

        #endregion

        #region Implementation of IJsonSerializable
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
                    case RedmineKeys.CONTENT_TYPE: ContentType = reader.ReadAsString(); break;
                    case RedmineKeys.CONTENT_URL: ContentUrl = reader.ReadAsString(); break;
                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadAsDateTime(); break;
                    case RedmineKeys.DESCRIPTION: Description = reader.ReadAsString(); break;
                    case RedmineKeys.FILE_NAME: FileName = reader.ReadAsString(); break;
                    case RedmineKeys.FILE_SIZE: FileSize = reader.ReadAsInt(); break;
                    case RedmineKeys.THUMBNAIL_URL: ThumbnailUrl = reader.ReadAsString(); break;
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
            using (new JsonObject(writer, RedmineKeys.ATTACHMENT))
            {
                writer.WriteProperty(RedmineKeys.FILE_NAME, FileName);
                writer.WriteProperty(RedmineKeys.DESCRIPTION, Description);
            }
        }
        #endregion

        #region Implementation of IEquatable<CustomFieldValue>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(Attachment other)
        {
            if (other == null) return false;
            return Id == other.Id
                && FileName == other.FileName
                && FileSize == other.FileSize
                && ContentType == other.ContentType
                && Author == other.Author
                && ThumbnailUrl == other.ThumbnailUrl
                && CreatedOn == other.CreatedOn
                && Description == other.Description
                && ContentUrl == other.ContentUrl;
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
                hashCode = HashCodeHelper.GetHashCode(FileName, hashCode);
                hashCode = HashCodeHelper.GetHashCode(FileSize, hashCode);
                hashCode = HashCodeHelper.GetHashCode(ContentType, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Author, hashCode);
                hashCode = HashCodeHelper.GetHashCode(CreatedOn, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Description, hashCode);
                hashCode = HashCodeHelper.GetHashCode(ContentUrl, hashCode);
                hashCode = HashCodeHelper.GetHashCode(ThumbnailUrl, hashCode);

                return hashCode;
            }
        }
        #endregion

        private string DebuggerDisplay =>
        $@"[{nameof(Attachment)}: 
{ToString()}, 
FileName={FileName}, 
FileSize={FileSize.ToString(CultureInfo.InvariantCulture)}, 
ContentType={ContentType}, 
Description={Description}, 
ContentUrl={ContentUrl}, 
Author={Author}, 
CreatedOn={CreatedOn?.ToString("u", CultureInfo.InvariantCulture)}]";

    }
}