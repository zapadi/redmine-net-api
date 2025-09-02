using System.Xml;

namespace Padi.RedmineApi.Serialization.Xml.Converters;

internal interface IXmlRedmineConverter<T>
{
    string RootName { get; }

    T? ReadXml(XmlReader reader);

    void WriteXml(XmlWriter writer, T value);
}