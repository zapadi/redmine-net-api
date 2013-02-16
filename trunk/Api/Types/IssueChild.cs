using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    [XmlRoot("issue")]
    public class IssueChild : Identifiable<IssueChild>, IXmlSerializable, IEquatable<Issue>, ICloneable
    {
        /// <summary>
        /// Gets or sets the tracker.
        /// </summary>
        /// <value>The tracker.</value>
        [XmlElement("tracker")]
        public IdentifiableName Tracker { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>The subject.</value>
        [XmlElement("subject")]
        public String Subject { get; set; }

        public XmlSchema GetSchema() { return null; }

        public void ReadXml(XmlReader reader)
        {
            Id = Convert.ToInt32(reader.GetAttribute("id"));
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
                    case "tracker": Tracker = new IdentifiableName(reader); break;

                    case "subject": Subject = reader.ReadElementContentAsString(); break;

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

        public bool Equals(Issue other)
        {
            if (other == null) return false;
            return (Id == other.Id && Tracker == other.Tracker && Subject == other.Subject);
        }
    }
}
