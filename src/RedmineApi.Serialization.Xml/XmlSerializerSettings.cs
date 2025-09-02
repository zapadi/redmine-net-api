using System.Xml;

namespace Padi.RedmineApi.Serialization.Xml;

/// <summary>
/// Provides configuration options for XML serialization
/// </summary>
public sealed class XmlSerializerSettings
{
    /// <summary>
    /// Gets or sets the XmlWriterSettings for serialization
    /// </summary>
    public XmlWriterSettings? WriterSettings { get; set; }
}