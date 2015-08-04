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
    public class XmlStreamingSerializer<T>
    {
        static XmlSerializerNamespaces ns;
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        XmlWriter writer;
        bool finished;

        static XmlStreamingSerializer()
        {
            ns = new XmlSerializerNamespaces();
            ns.Add("", "");
        }

        private XmlStreamingSerializer()
        {
            serializer = new XmlSerializer(typeof(T));
        }

        public XmlStreamingSerializer(TextWriter w)
                : this(XmlWriter.Create(w))
        {
        }

        public XmlStreamingSerializer(XmlWriter writer)
                : this()
        {
            this.writer = writer;
            writer.WriteStartDocument();
            writer.WriteStartElement("ArrayOf" + typeof(T).Name);
        }

        public void Finish()
        {
            writer.WriteEndDocument();
            writer.Flush();
            finished = true;
        }

        public void Close()
        {
            if (!finished)
                Finish();
            writer.Close();
        }

        public void Serialize(T item)
        {
            serializer.Serialize(writer, item, ns);
        }
    }
}