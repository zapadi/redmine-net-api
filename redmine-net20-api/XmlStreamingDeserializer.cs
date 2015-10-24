using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Redmine.Net.Api
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>http://florianreischl.blogspot.ro/search/label/c%23</remarks>
    public class XmlStreamingDeserializer<T>
    {
        static XmlSerializerNamespaces ns;
        XmlSerializer serializer;
        XmlReader reader;

        static XmlStreamingDeserializer()
        {
            ns = new XmlSerializerNamespaces();
            ns.Add("", "");
        }

        private XmlStreamingDeserializer()
        {
            serializer = new XmlSerializer(typeof(T));
        }

        public XmlStreamingDeserializer(TextReader reader)
                : this(XmlReader.Create(reader))
        {
        }

        public XmlStreamingDeserializer(XmlReader reader)
                : this()
        {
            this.reader = reader;
        }

        public void Close()
        {
            reader.Close();
        }

        public T Deserialize()
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Depth == 1 && reader.Name == typeof(T).Name)
                {
                    XmlReader xmlReader = reader.ReadSubtree();
                    return (T)serializer.Deserialize(xmlReader);
                }
            }
            return default(T);
        }
    }
}