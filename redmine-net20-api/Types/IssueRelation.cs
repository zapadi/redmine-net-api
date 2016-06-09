/*
   Copyright 2011 - 2016 Adrian Popescu.

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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// Availability 1.3
    /// </summary>
    [XmlRoot(RedmineKeys.RELATION)]
    public class IssueRelation : Identifiable<IssueRelation>, IXmlSerializable, IEquatable<IssueRelation>
    {
        /// <summary>
        /// Gets or sets the issue id.
        /// </summary>
        /// <value>The issue id.</value>
        [XmlElement(RedmineKeys.ISSUE_ID)]
        public int IssueId { get; set; }

        /// <summary>
        /// Gets or sets the related issue id.
        /// </summary>
        /// <value>The issue to id.</value>
        [XmlElement(RedmineKeys.ISSUE_TO_ID)]
        public int IssueToId { get; set; }

        /// <summary>
        /// Gets or sets the type of relation.
        /// </summary>
        /// <value>The type.</value>
        [XmlElement(RedmineKeys.RELATION_TYPE)]
        public IssueRelationType Type { get; set; }

        /// <summary>
        /// Gets or sets the delay for a "precedes" or "follows" relation.
        /// </summary>
        /// <value>The delay.</value>
        [XmlElement(RedmineKeys.DELAY, IsNullable = true)]
        public int? Delay { get; set; }

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
                            case RedmineKeys.ISSUE_ID: IssueId = reader.ReadAttributeAsInt(attributeName); break;
                            case RedmineKeys.ISSUE_TO_ID: IssueToId = reader.ReadAttributeAsInt(attributeName); break;
                            case RedmineKeys.RELATION_TYPE:
                                var rt = reader.GetAttribute(attributeName);
                                if (!string.IsNullOrEmpty(rt))
                                {
                                    Type = (IssueRelationType)Enum.Parse(typeof(IssueRelationType), rt, true);
                                }
                                break;
                            case RedmineKeys.DELAY: Delay = reader.ReadAttributeAsNullableInt(attributeName); break;
                        }
                    }
                    return;
                }

                switch (reader.Name)
                {
                    case RedmineKeys.ID: Id = reader.ReadElementContentAsInt(); break;
                    case RedmineKeys.ISSUE_ID: IssueId = reader.ReadElementContentAsInt(); break;
                    case RedmineKeys.ISSUE_TO_ID: IssueToId = reader.ReadElementContentAsInt(); break;
                    case RedmineKeys.RELATION_TYPE:
                        var rt = reader.ReadElementContentAsString();
                        if (!string.IsNullOrEmpty(rt))
                        {
                            Type = (IssueRelationType)Enum.Parse(typeof(IssueRelationType), rt, true);
                        }
                        break;
                    case RedmineKeys.DELAY: Delay = reader.ReadElementContentAsNullableInt(); break;
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
            writer.WriteElementString(RedmineKeys.ISSUE_TO_ID, IssueToId.ToString());
            writer.WriteElementString(RedmineKeys.RELATION_TYPE, Type.ToString());
            if (Type == IssueRelationType.precedes || Type == IssueRelationType.follows)
                writer.WriteValueOrEmpty(Delay, RedmineKeys.DELAY);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IssueRelation other)
        {
            if (other == null) return false;
            return (Id == other.Id && IssueId == other.IssueId && IssueToId == other.IssueToId && Type == other.Type && Delay == other.Delay);
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[IssueRelation: {4}, IssueId={0}, IssueToId={1}, Type={2}, Delay={3}]", IssueId, IssueToId, Type, Delay, base.ToString());
        }
    }
}