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



using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;
using System;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Redmine.Net.Api.Serialization;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [XmlRoot(RedmineKeys.FILE)]
    public sealed class File : Identifiable<File>
    {
        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int FileSize { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public string ContentType { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ContentUrl { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public IdentifiableName Author { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreatedOn { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public IdentifiableName Version { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Digest { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public int Downloads { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public string Token { get; set; }
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
                    case RedmineKeys.DIGEST: Digest = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.DOWNLOADS: Downloads = reader.ReadElementContentAsInt(); break;
                    case RedmineKeys.FILE_NAME: Filename = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.FILE_SIZE: FileSize = reader.ReadElementContentAsInt(); break;
                    case RedmineKeys.TOKEN: Token = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.VERSION: Version = new IdentifiableName(reader); break;
                    case RedmineKeys.VERSION_ID: Version = IdentifiableName.Create<IdentifiableName>(reader.ReadElementContentAsInt()); break;
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
            writer.WriteElementString(RedmineKeys.TOKEN, Token);
            writer.WriteIdIfNotNull(RedmineKeys.VERSION_ID, Version);
            writer.WriteElementString(RedmineKeys.FILE_NAME, Filename);
            writer.WriteElementString(RedmineKeys.DESCRIPTION, Description);
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
                    case RedmineKeys.ID: Id = reader.ReadAsInt32().GetValueOrDefault(); break;
                    case RedmineKeys.AUTHOR: Author = new IdentifiableName(reader); break;
                    case RedmineKeys.CONTENT_TYPE: ContentType = reader.ReadAsString(); break;
                    case RedmineKeys.CONTENT_URL: ContentUrl = reader.ReadAsString(); break;
                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadAsDateTime(); break;
                    case RedmineKeys.DESCRIPTION: Description = reader.ReadAsString(); break;
                    case RedmineKeys.DIGEST: Digest = reader.ReadAsString(); break;
                    case RedmineKeys.DOWNLOADS: Downloads = reader.ReadAsInt32().GetValueOrDefault(); break;
                    case RedmineKeys.FILE_NAME: Filename = reader.ReadAsString(); break;
                    case RedmineKeys.FILE_SIZE: FileSize = reader.ReadAsInt32().GetValueOrDefault(); break;
                    case RedmineKeys.TOKEN: Token = reader.ReadAsString(); break;
                    case RedmineKeys.VERSION: Version = new IdentifiableName(reader); break;
                    case RedmineKeys.VERSION_ID: Version = IdentifiableName.Create<IdentifiableName>(reader.ReadAsInt32().GetValueOrDefault()); break;
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
            using (new JsonObject(writer, RedmineKeys.FILE))
            {
                using (new JsonObject(writer))
                {
                    writer.WriteProperty(RedmineKeys.TOKEN, Token);
                    writer.WriteIdIfNotNull(RedmineKeys.VERSION_ID, Version);
                    writer.WriteProperty(RedmineKeys.FILE_NAME, Filename);
                    writer.WriteProperty(RedmineKeys.DESCRIPTION, Description);
                }
            }
        }
        #endregion

        #region Implementation of IEquatable<File>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(File other)
        {
            if (other == null) return false;
            return Id == other.Id
                && Filename == other.Filename
                && FileSize == other.FileSize
                && Description == other.Description
                && ContentType == other.ContentType
                && ContentUrl == other.ContentUrl
                && Author == other.Author
                && CreatedOn == other.CreatedOn
                && Version == other.Version
                && Digest == other.Digest
                && Downloads == other.Downloads
                && Token == other.Token;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hashCode = base.GetHashCode();

            hashCode = HashCodeHelper.GetHashCode(Filename, hashCode);
            hashCode = HashCodeHelper.GetHashCode(FileSize, hashCode);
            hashCode = HashCodeHelper.GetHashCode(ContentType, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Description, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Author, hashCode);
            hashCode = HashCodeHelper.GetHashCode(ContentUrl, hashCode);

            hashCode = HashCodeHelper.GetHashCode(Author, hashCode);
            hashCode = HashCodeHelper.GetHashCode(CreatedOn, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Version, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Digest, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Downloads, hashCode);

            return hashCode;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string DebuggerDisplay => $"[{nameof(File)}: {ToString()}, Name={Filename}]";

    }
}