using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    [XmlRoot("value")]
    public class CustomFieldValue
    {
        [XmlText]
        public string Info { get; set; }

        public override string ToString()
        {
            return Info;
        }
    }
}