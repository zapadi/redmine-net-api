using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using Padi.RedmineApi.Exceptions;
using Padi.RedmineApi.Extensions;
using Padi.RedmineApi.Serialization.JsonText.Extensions;
using Padi.RedmineApi.Types;

namespace Padi.RedmineApi.Serialization.JsonText;

public sealed class JsonTextRedmineSerializer : IRedmineSerializer
{
    private readonly JsonWriterOptions _writerOptions;

    private static readonly JsonWriterOptions WriterOptions = new()
    {
#if DEBUG
        Indented = true,
#endif

        // writer.DateFormatHandling = DateFormatHandling.IsoDateFormat;
    };

    public JsonTextRedmineSerializer(JsonWriterOptions? writerOptions = null)
    {
        _writerOptions = writerOptions ?? WriterOptions;
    }

    public string Format { get; } = "json";

    public string ContentType { get; } = "application/json";

    public string Serialize<T>(T data) where T : class
    {
        if (data == null)
        {
            throw new RedmineException(RedmineApiErrorCode.SerializationError, $"Could not serialize null of type {typeof(T).Name}");
        }

        using var memoryStream = new MemoryStream();
        using var utf8JsonWriter = new Utf8JsonWriter(memoryStream, _writerOptions);

        var converter = JsonConverterRegistry.GetConverter<T>();

        converter.WriteJson(utf8JsonWriter, data, true);

        utf8JsonWriter.Flush();

        return Encoding.UTF8.GetString(memoryStream.ToArray());
    }

    public T? Deserialize<T>(string input) where T : new()
    {
        ReadOnlySpan<byte> jsonReadOnlySpan = Encoding.UTF8.GetBytes(input);
        var utf8JsonReader = new Utf8JsonReader(jsonReadOnlySpan);

        var converter = JsonConverterRegistry.GetConverter<T>();

        if (utf8JsonReader.Read() && utf8JsonReader.Read())
        {
            return converter.ReadJson(ref utf8JsonReader);
        }

        return default;
    }

    public string SerializeUserId(int userId)
    {
        return $$"""{"user_id":"{{userId.ToInvariantString()}}"}""";
    }

    string IRedmineSerializer.Serialize<T>(T obj)
    {
        return Serialize(obj);
    }

    public PagedResults<T> DeserializeToPagedResults<T>(string input) where T : class, new()
    {
        ReadOnlySpan<byte> jsonReadOnlySpan = Encoding.UTF8.GetBytes(input);
        var utf8JsonReader = new Utf8JsonReader(jsonReadOnlySpan);
        var total = 0;
        var offset = 0;
        var limit = 0;
        List<T>? list = null;

        while (utf8JsonReader.Read())
        {
            if (utf8JsonReader.TokenType == JsonTokenType.PropertyName)
            {
                var value = utf8JsonReader.GetString();

                switch (value)
                {
                    case RedmineKeys.TOTAL_COUNT: total = utf8JsonReader.ReadAsInt(); break;
                    case RedmineKeys.OFFSET: offset = utf8JsonReader.ReadAsInt(); break;
                    case RedmineKeys.LIMIT: limit = utf8JsonReader.ReadAsInt(); break;
                }
            }

            if (utf8JsonReader.TokenType == JsonTokenType.StartArray)
            {
                list = utf8JsonReader.ReadAsCollection<T>();
            }
        }

        return new PagedResults<T>(list, total, offset, limit);
    }

    public int Count<T>(string input)
    {
        ReadOnlySpan<byte> jsonReadOnlySpan = Encoding.UTF8.GetBytes(input);
        var utf8JsonReader = new Utf8JsonReader(jsonReadOnlySpan);
        var total = 0;

        while (utf8JsonReader.Read())
        {
            if (utf8JsonReader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            if (utf8JsonReader.ValueTextEquals(RedmineKeys.TOTAL_COUNT))
            {
                total = utf8JsonReader.GetInt32();
                return total;
            }
        }

        return total;
    }
}
