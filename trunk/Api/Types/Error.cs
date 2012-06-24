using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    [Serializable]
    [XmlRoot("error")]
    public class Error
    {
        [XmlText]
        public string Info { get; set; }
    }
}