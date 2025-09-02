using System.Xml;

namespace Padi.RedmineApi.Serialization.Xml.Converters;

internal abstract class XmlRedmineConverter<T> : IXmlRedmineConverter<T>
{
    public abstract string RootName { get; }

    /// <summary>
    /// Reads the XML representation of the object and creates an instance of type T
    /// </summary>
    /// <param name="reader">The XmlReader to read from</param>
    /// <returns>An object of type T containing the deserialized data</returns>
    public abstract T? ReadXml(XmlReader reader);

    /// <summary>
    /// Writes the XML representation of the object
    /// </summary>
    /// <param name="writer">The XmlWriter to write to</param>
    /// <param name="value">The object to serialize</param>
    public abstract void WriteXml(XmlWriter writer, T value);
}