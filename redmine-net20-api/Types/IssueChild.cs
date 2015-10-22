/*
   Copyright 2011 - 2015 Adrian Popescu, Dorin Huzum.

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

namespace Redmine.Net.Api.Types
{
    [XmlRoot(RedmineKeys.ISSUE)]
    public class IssueChild : Identifiable<IssueChild>, IXmlSerializable, IEquatable<IssueChild>, ICloneable
    {
        /// <summary>
        /// Gets or sets the tracker.
        /// </summary>
        /// <value>The tracker.</value>
        [XmlElement(RedmineKeys.TRACKER)]
        public IdentifiableName Tracker { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>The subject.</value>
        [XmlElement(RedmineKeys.SUBJECT)]
        public String Subject { get; set; }

        public XmlSchema GetSchema() { return null; }

        public void ReadXml(XmlReader reader)
        {
            Id = Convert.ToInt32(reader.GetAttribute(RedmineKeys.ID));
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
                    case RedmineKeys.TRACKER: Tracker = new IdentifiableName(reader); break;

                    case RedmineKeys.SUBJECT: Subject = reader.ReadElementContentAsString(); break;

                    default: reader.Read(); break;
                }
            }
        }

        public void WriteXml(XmlWriter writer) { }

        public object Clone()
        {
            var issueChild = new IssueChild { Subject = Subject, Tracker = Tracker };
            return issueChild;
        }

        public bool Equals(IssueChild other)
        {
            if (other == null) return false;
            return (Id == other.Id && Tracker == other.Tracker && Subject == other.Subject);
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other.GetType() != GetType()) return false;
            return Equals(other as IssueChild);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 13;
                hashCode = (hashCode * 397) ^ Id.GetHashCode();
                hashCode = (hashCode * 397) ^ Tracker.GetHashCode();
                hashCode = (hashCode * 397) ^ (string.IsNullOrEmpty(Subject) ? 0 : Subject.GetHashCode());
                return hashCode;
            }
        }

        public override string ToString()
        {
            return base.ToString() + ", " + Subject + ", " + Tracker;
        }
    }
}
