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
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    [XmlRoot(RedmineKeys.ROLE)]
    public class MembershipRole : IdentifiableName, IEquatable<MembershipRole>
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="MembershipRole"/> is inherited.
        /// </summary>
        /// <value>
        ///   <c>true</c> if inherited; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute(RedmineKeys.INHERITED)]
        public bool Inherited { get; set; }

        /// <summary>
        /// Reads the XML.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            Inherited = reader.ReadAttributeAsBoolean(RedmineKeys.INHERITED);
            reader.Read();
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteValue(Id);
        }

        public bool Equals(MembershipRole other)
        {
            if (other == null) return false;
            return Id == other.Id && Name == other.Name && Inherited == other.Inherited;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 13;
                hashCode = Utils.GetHashCode(Id, hashCode);
                hashCode = Utils.GetHashCode(Name, hashCode);
                hashCode = Utils.GetHashCode(Inherited, hashCode);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format("[MembershipRole: {1}, Inherited={0}]", Inherited, base.ToString());
        }
    }
}