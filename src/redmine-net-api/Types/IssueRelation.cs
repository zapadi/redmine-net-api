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
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Serialization;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// Availability 1.3
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [XmlRoot(RedmineKeys.RELATION)]
    public sealed class IssueRelation : Identifiable<IssueRelation>
    {
        #region Properties
        /// <summary>
        /// Gets or sets the issue id.
        /// </summary>
        /// <value>The issue id.</value>
        public int IssueId { get; internal set; }

        /// <summary>
        /// Gets or sets the related issue id.
        /// </summary>
        /// <value>The issue to id.</value>
        public int IssueToId { get; set; }

        /// <summary>
        /// Gets or sets the type of relation.
        /// </summary>
        /// <value>The type.</value>
        public IssueRelationType Type { get; set; }

        /// <summary>
        /// Gets or sets the delay for a "precedes" or "follows" relation.
        /// </summary>
        /// <value>The delay.</value>
        public int? Delay { get; set; }
        #endregion

        #region Implementation of IXmlSerialization
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public override void ReadXml(XmlReader reader)
        {
            if (!reader.IsEmptyElement) reader.Read();
            while (!reader.EOF)
            {
                if (reader.IsEmptyElement && !reader.HasAttributes)
                {
                    reader.Read();
                    continue;
                }

                if (reader.IsEmptyElement && reader.HasAttributes)
                {
                    while (reader.MoveToNextAttribute())
                    {
                        var attributeName = reader.Name;
                        switch (reader.Name)
                        {
                            case RedmineKeys.ID: Id = reader.ReadAttributeAsInt(attributeName); break;
                            case RedmineKeys.DELAY: Delay = reader.ReadAttributeAsNullableInt(attributeName); break;
                            case RedmineKeys.ISSUE_ID: IssueId = reader.ReadAttributeAsInt(attributeName); break;
                            case RedmineKeys.ISSUE_TO_ID: IssueToId = reader.ReadAttributeAsInt(attributeName); break;
                            case RedmineKeys.RELATION_TYPE: Type = ReadIssueRelationType(reader.GetAttribute(attributeName)); break;
                        }
                    }
                    return;
                }

                switch (reader.Name)
                {
                    case RedmineKeys.ID: Id = reader.ReadElementContentAsInt(); break;
                    case RedmineKeys.DELAY: Delay = reader.ReadElementContentAsNullableInt(); break;
                    case RedmineKeys.ISSUE_ID: IssueId = reader.ReadElementContentAsInt(); break;
                    case RedmineKeys.ISSUE_TO_ID: IssueToId = reader.ReadElementContentAsInt(); break;
                    case RedmineKeys.RELATION_TYPE: Type = ReadIssueRelationType(reader.ReadElementContentAsString()); break;
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
            AssertValidIssueRelationType();

            writer.WriteElementString(RedmineKeys.ISSUE_TO_ID, IssueToId.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString(RedmineKeys.RELATION_TYPE, Type.ToString().ToLowerInv());

            if (Type == IssueRelationType.Precedes || Type == IssueRelationType.Follows)
            {
                writer.WriteValueOrEmpty(RedmineKeys.DELAY, Delay);
            }
        }
        #endregion

        #region Implementation of IJsonSerialization
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteJson(JsonWriter writer)
        {
            AssertValidIssueRelationType();

            using (new JsonObject(writer, RedmineKeys.RELATION))
            {
                writer.WriteProperty(RedmineKeys.ISSUE_TO_ID, IssueToId);
                writer.WriteProperty(RedmineKeys.RELATION_TYPE, Type.ToString().ToLowerInv());

                if (Type == IssueRelationType.Precedes || Type == IssueRelationType.Follows)
                {
                    writer.WriteValueOrEmpty(RedmineKeys.DELAY, Delay);
                }
            }
        }

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
                    case RedmineKeys.DELAY: Delay = reader.ReadAsInt32(); break;
                    case RedmineKeys.ISSUE_ID: IssueId = reader.ReadAsInt(); break;
                    case RedmineKeys.ISSUE_TO_ID: IssueToId = reader.ReadAsInt(); break;
                    case RedmineKeys.RELATION_TYPE: Type = ReadIssueRelationType(reader.ReadAsString()); break;
                }
            }
        }

        private void AssertValidIssueRelationType()
        {
            if (Type == IssueRelationType.Undefined)
            {
                throw new RedmineException($"The value `{nameof(IssueRelationType)}.`{nameof(IssueRelationType.Undefined)}` is not allowed to create relations!");
            }
        }
        
        private static IssueRelationType ReadIssueRelationType(string value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return IssueRelationType.Undefined;
            }
            
            if (short.TryParse(value, out var enumId))
            {
                return (IssueRelationType)enumId;
            }

            if (RedmineKeys.COPIED_TO.Equals(value, StringComparison.OrdinalIgnoreCase))
            {
                return IssueRelationType.CopiedTo;
            }

            if (RedmineKeys.COPIED_FROM.Equals(value, StringComparison.OrdinalIgnoreCase))
            {
                return IssueRelationType.CopiedFrom;
            }

            return (IssueRelationType)Enum.Parse(typeof(IssueRelationType), value, true);
        }
        
        #endregion

        #region Implementation of IEquatable<IssueRelation>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(IssueRelation other)
        {
            if (other == null) return false;
            return Id == other.Id && IssueId == other.IssueId && IssueToId == other.IssueToId && Type == other.Type && Delay == other.Delay;
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
                hashCode = HashCodeHelper.GetHashCode(IssueId, hashCode);
                hashCode = HashCodeHelper.GetHashCode(IssueToId, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Type, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Delay, hashCode);
                return hashCode;
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string DebuggerDisplay => $@"[{nameof(IssueRelation)}: {ToString()}, 
IssueId={IssueId.ToString(CultureInfo.InvariantCulture)}, 
IssueToId={IssueToId.ToString(CultureInfo.InvariantCulture)}, 
Type={Type:G},
Delay={Delay?.ToString(CultureInfo.InvariantCulture)}]";

    }
}