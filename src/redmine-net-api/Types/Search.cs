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
using System.Xml.Schema;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Serialization.Json;
using Redmine.Net.Api.Serialization.Json.Extensions;
using Redmine.Net.Api.Serialization.Xml.Extensions;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay($"{{{nameof(DebuggerDisplay)},nq}}")]
    [XmlRoot(RedmineKeys.RESULT)]
    public sealed class Search: IXmlSerializable, IJsonSerializable, IEquatable<Search>
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? DateTime { get; set; }

        /// <inheritdoc />
        public XmlSchema GetSchema() { return null; }

        /// <inheritdoc />
        public void ReadXml(XmlReader reader)
        {
            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.Name)
                {
                    case RedmineKeys.ID: Id = reader.ReadElementContentAsInt(); break;
                    case RedmineKeys.DESCRIPTION: Description = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.DATE_TIME: DateTime = reader.ReadElementContentAsNullableDateTime(); break;
                    case RedmineKeys.URL: Url = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.TYPE: Type = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.TITLE: Title = reader.ReadElementContentAsString(); break;
                    default: reader.Read(); break;
                }
            }
        }

        /// <inheritdoc />
        public void WriteXml(XmlWriter writer)
        {
        }

        /// <inheritdoc />
        public void WriteJson(JsonWriter writer)
        {
        }

        /// <inheritdoc />
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
                    case RedmineKeys.ID: Id = reader.ReadAsInt(); break;
                    case RedmineKeys.DESCRIPTION: Description = reader.ReadAsString(); break;
                    case RedmineKeys.DATE_TIME: DateTime = reader.ReadAsDateTime(); break;
                    case RedmineKeys.URL: Url = reader.ReadAsString(); break;
                    case RedmineKeys.TYPE: Type = reader.ReadAsString(); break;
                    case RedmineKeys.TITLE: Title = reader.ReadAsString(); break;
                    default: reader.Read(); break;
                }
            }
        }

        /// <inheritdoc />
        public bool Equals(Search other)
        {
            if (other == null) return false;
            return Id == other.Id 
                    && string.Equals(Title, other.Title, StringComparison.Ordinal)
                    && string.Equals(Description, other.Description, StringComparison.Ordinal)
                    && string.Equals(Url, other.Url, StringComparison.Ordinal)
                    && string.Equals(Type, other.Type, StringComparison.Ordinal)
                    && DateTime == other.DateTime;
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
            return Equals(obj as Search);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = 397;
            hashCode = HashCodeHelper.GetHashCode(Id, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Title, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Type, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Url, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Description, hashCode);
            hashCode = HashCodeHelper.GetHashCode(DateTime, hashCode);
            return hashCode;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Search left, Search right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Search left, Search right)
        {
            return !Equals(left, right);
        }

        private string DebuggerDisplay => $"[Search: Id={Id.ToInvariantString()}, Title={Title}, Type={Type}]";
    }
}