using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    [Serializable]
    [XmlRoot("membership")]
    public class Membership
    {
        /// <summary>
        /// Gets or sets the project.
        /// </summary>
        /// <value>The project.</value>
        [XmlElement("project")]
        public IdentifiableName Project { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [XmlArray("roles")]
        [XmlArrayItem("role")]
        public Collection<Role> Type { get; set; }
    }
}