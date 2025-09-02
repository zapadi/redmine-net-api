using System.Text.Json;

namespace Padi.RedmineApi.Serialization.JsonText;

/// <summary>
/// Provides configuration options for JSON serialization
/// </summary>
public sealed class JsonTextSerializerSettings
{
    /// <summary>
    /// Gets or sets the JsonReaderOptions for serialization
    /// </summary>
    public JsonReaderOptions? ReaderOptions { get; set; }
    
    /// <summary>
    /// Gets or sets the JsonWriterOptions for serialization
    /// </summary>
    public JsonWriterOptions? WriterOptions { get; set; }
}