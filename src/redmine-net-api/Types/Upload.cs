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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Serialization;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// Support for adding attachments through the REST API is added in Redmine 1.4.0.
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [XmlRoot(RedmineKeys.UPLOAD)]
    public sealed class Upload : IXmlSerializable, IJsonSerializable, IEquatable<Upload>
    {
        #region Properties
        /// <summary>
        /// Gets or sets the uploaded token.
        /// </summary>
        /// <value>The name of the file.</value>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// Maximum allowed file size (1024000).
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the file description. (Undocumented feature)
        /// </summary>
        /// <value>The file descroütopm.</value>
        public string Description { get; set; }
        #endregion

        #region Implementation of IXmlSerialization
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public XmlSchema GetSchema() { return null; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public void ReadXml(XmlReader reader)
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
                    case RedmineKeys.CONTENT_TYPE: ContentType = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.DESCRIPTION: Description = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.FILE_NAME: FileName = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.TOKEN: Token = reader.ReadElementContentAsString(); break;
                    default: reader.Read(); break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString(RedmineKeys.TOKEN, Token);
            writer.WriteElementString(RedmineKeys.CONTENT_TYPE, ContentType);
            writer.WriteElementString(RedmineKeys.FILE_NAME, FileName);
            writer.WriteElementString(RedmineKeys.DESCRIPTION, Description);
        }
        #endregion

        #region Implementation of IJsonSerialization
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public void ReadJson(JsonReader reader)
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
                    case RedmineKeys.CONTENT_TYPE: ContentType = reader.ReadAsString(); break;
                    case RedmineKeys.DESCRIPTION: Description = reader.ReadAsString(); break;
                    case RedmineKeys.FILE_NAME: FileName = reader.ReadAsString(); break;
                    case RedmineKeys.TOKEN: Token = reader.ReadAsString(); break;
                    default: reader.Read(); break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public void WriteJson(JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WriteProperty(RedmineKeys.TOKEN, Token);
            writer.WriteProperty(RedmineKeys.CONTENT_TYPE, ContentType);
            writer.WriteProperty(RedmineKeys.FILE_NAME, FileName);
            writer.WriteProperty(RedmineKeys.DESCRIPTION, Description);
            writer.WriteEndObject();
        }
        #endregion

        #region Implementation of IEquatable<Upload>
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(Upload other)
        {
            return other != null
                && string.Equals(Token, other.Token, StringComparison.OrdinalIgnoreCase)
                && string.Equals(FileName, other.FileName, StringComparison.OrdinalIgnoreCase)
                && string.Equals(Description, other.Description, StringComparison.OrdinalIgnoreCase)
                && string.Equals(ContentType, other.ContentType, StringComparison.OrdinalIgnoreCase);
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
            return Equals(obj as Upload);
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
                hashCode = HashCodeHelper.GetHashCode(Token, hashCode);
                hashCode = HashCodeHelper.GetHashCode(FileName, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Description, hashCode);
                hashCode = HashCodeHelper.GetHashCode(ContentType, hashCode);
                return hashCode;
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string DebuggerDisplay => $"[Upload: Token={Token}, FileName={FileName}, ContentType={ContentType}, Description={Description}]";

    }
}