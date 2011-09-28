using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Redmine.Net.Api
{
    public static class ExtensionMethods
    {
        public static DateTime? ReadElementContentAsNullableDateTime(this XmlReader reader)
        {
            var str = reader.ReadElementContentAsString();

            DateTime result;
            if (String.IsNullOrWhiteSpace(str) || !DateTime.TryParse(str, out result)) return null;

            return result;
        }

        public static float? ReadElementContentAsNullableFloat(this XmlReader reader)
        {
            var str = reader.ReadElementContentAsString();

            float result;
            if (String.IsNullOrWhiteSpace(str) || !float.TryParse(str, out result)) return null;

            return result;
        }

        public static Collection<T> ReadElementContentAsCollection<T>(this XmlReader reader) where T : IXmlSerializable, new()
        {
            var result = new Collection<T>();

            String xml = reader.ReadOuterXml();
            XmlReader r = new XmlTextReader(new StringReader(xml));
            r.ReadStartElement();
            while (!r.EOF)
            {
                if (r.NodeType == XmlNodeType.EndElement)
                {
                    r.ReadEndElement();
                    continue;
                }

                var temp = new T();
                temp.ReadXml(r);
                result.Add(temp);

            }
            return result;
        }
    }
}