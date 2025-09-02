using System;
using System.Text.Json;
using Padi.RedmineApi.Extensions;

namespace Padi.RedmineApi.Serialization.JsonText;

/// <summary>
/// 
/// </summary>
internal sealed class JsonObject : IDisposable
{
    private readonly bool _hasRoot;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="root"></param>
    public JsonObject(Utf8JsonWriter writer, string? root = null)
    {
        Writer = writer;
        Writer.WriteStartObject();

        if (root.IsNullOrWhiteSpace())
        {
            return;
        }
            
        _hasRoot = true;
        Writer.WritePropertyName(root);
        Writer.WriteStartObject();
    }

    private Utf8JsonWriter Writer { get; }

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        Writer.WriteEndObject();
        if (_hasRoot)
        {
            Writer.WriteEndObject();
        }
    }
}