using System;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    [Serializable]
    [XmlRoot("relation")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class IssueRelation
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [XmlElement("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the issue id.
        /// </summary>
        /// <value>The issue id.</value>
        [XmlElement("issue_id")]
        public int IssueId { get; set; }

        /// <summary>
        /// Gets or sets the issue to id.
        /// </summary>
        /// <value>The issue to id.</value>
        [XmlElement("issue_to_id")]
        public int IssueToId { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [XmlElement("relation_type")]
        public String Type { get; set; }

        /// <summary>
        /// Gets or sets the delay.
        /// </summary>
        /// <value>The delay.</value>
        [XmlElement("delay")]
        public int Delay { get; set; }
    }
}