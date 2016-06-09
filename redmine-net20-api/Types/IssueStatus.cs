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
using System.Xml.Serialization;
using Redmine.Net.Api.Internals;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// Availability 1.3
    /// </summary>
    [XmlRoot(RedmineKeys.ISSUE_STATUS)]
    public class IssueStatus : IdentifiableName, IEquatable<IssueStatus>
    {
        /// <summary>
        /// Gets or sets a value indicating whether IssueStatus is default.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if IssueStatus is default; otherwise, <c>false</c>.
        /// </value>
        [XmlElement(RedmineKeys.IS_DEFAULT)]
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IssueStatus is closed.
        /// </summary>
        /// <value><c>true</c> if IssueStatus is closed; otherwise, <c>false</c>.</value>
        [XmlElement(RedmineKeys.IS_CLOSED)]
        public bool IsClosed { get; set; }

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

                    case RedmineKeys.NAME: Name = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.IS_DEFAULT: IsDefault = reader.ReadElementContentAsBoolean(); break;

                    case RedmineKeys.IS_CLOSED: IsClosed = reader.ReadElementContentAsBoolean(); break;

                    default: reader.Read(); break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteXml(XmlWriter writer) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IssueStatus other)
        {
            if (other == null) return false;
            return (Id == other.Id && Name == other.Name && IsClosed == other.IsClosed && IsDefault == other.IsDefault);
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
                hashCode = HashCodeHelper.GetHashCode(Name, hashCode);
                hashCode = HashCodeHelper.GetHashCode(IsClosed, hashCode);
                hashCode = HashCodeHelper.GetHashCode(IsDefault, hashCode);
                return hashCode;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[IssueStatus: {2}, IsDefault={0}, IsClosed={1}]", IsDefault, IsClosed, base.ToString());
        }
    }
}