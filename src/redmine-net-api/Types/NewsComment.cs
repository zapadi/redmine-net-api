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

using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Redmine.Net.Api.Internals;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [XmlRoot(RedmineKeys.COMMENT)]
    public sealed class NewsComment: Identifiable<NewsComment>
    {
        /// <summary>
        /// 
        /// </summary>
        public IdentifiableName Author { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Content { get; set; }

        /// <inheritdoc />
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
                    case RedmineKeys.CONTENT: Content = reader.ReadElementContentAsString(); break;
                    default: reader.Read(); break;
                }
            }
        }

        /// <inheritdoc />
        public override void WriteXml(XmlWriter writer)
        {
        }

        /// <inheritdoc />
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
                    case RedmineKeys.ID:
                        Id = reader.ReadAsInt32().GetValueOrDefault();
                        break;
                    case RedmineKeys.AUTHOR: Author = new IdentifiableName(reader); break;
                    case RedmineKeys.CONTENT: Content = reader.ReadAsString(); break;
                    default: reader.Read(); break;
                }
            }
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, bool full = false)
        {
        }

        /// <inheritdoc />
        public override bool Equals(NewsComment other)
        {
            if (other == null) return false;
            return Id == other.Id && Author == other.Author && Content == other.Content;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = base.GetHashCode();

            hashCode = HashCodeHelper.GetHashCode(Author, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Content, hashCode);

            return hashCode;
        }

        private string DebuggerDisplay => $@"[{nameof(IssueAllowedStatus)}: {ToString()},
{nameof(NewsComment)}: {ToString()}, 
Author={Author}, 
CONTENT={Content}]";
    }
}