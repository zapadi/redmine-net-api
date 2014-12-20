using System.Xml;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    [XmlRoot("tracker")]
    public class TrackerCustomField : Tracker
    {
        public override void ReadXml(XmlReader reader)
        {
            Id = reader.ReadAttributeAsInt("id");
            Name = reader.GetAttribute("name");
            reader.Read();
        }
    }
}