using System;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    [XmlRoot("error")]
    public class Error
    {
        [XmlText]
        public string Info { get; set; }

        public override string ToString()
        {
            return Info;
        }
    }
}