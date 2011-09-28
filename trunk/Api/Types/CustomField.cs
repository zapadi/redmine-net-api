using System;
using System.Xml;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    [Serializable]
    [XmlRoot("custom_field")]
    [XmlTypeAttribute(AnonymousType = true)]
    public class CustomField : IdentifiableName
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [XmlText]
        public String Value { get; set; }

        public override void ReadXml(XmlReader reader)
        {
            Id = Convert.ToInt32(reader.GetAttribute("id"));
            Name = reader.GetAttribute("name");
            Value = reader.ReadElementContentAsString();
        }
    }
}