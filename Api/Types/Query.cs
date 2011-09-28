
using System;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    [Serializable]
    [XmlRoot("query")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class Query : IdentifiableName
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is public.
        /// </summary>
        /// <value><c>true</c> if this instance is public; otherwise, <c>false</c>.</value>
        [XmlElement("is_public")]
        public bool IsPublic { get; set; }

        /// <summary>
        /// Gets or sets the project id.
        /// </summary>
        /// <value>The project id.</value>
        [XmlElement("project_id")]
        public int ProjectId { get; set; }
    }
}