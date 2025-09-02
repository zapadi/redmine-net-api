using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Padi.RedmineApi.Exceptions;
using Padi.RedmineApi.Extensions;
using Padi.RedmineApi.Serialization.Json.Extensions;
using Padi.RedmineApi.Types;

namespace Padi.RedmineApi.Serialization.Json;

/// <summary>
/// 
/// </summary>
public sealed class JsonRedmineSerializer : IRedmineSerializer
{
    private readonly JsonSerializerSettings? _serializerSettings;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serializerSettings"></param>
    public JsonRedmineSerializer(JsonSerializerSettings? serializerSettings = null)
    {
        _serializerSettings = serializerSettings;
    }
    
    /// <summary>
    /// 
    /// </summary>
    public string Format { get; } =  "json";
    
    /// <summary>
    /// 
    /// </summary>
    public string ContentType { get; } =  "application/json";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="RedmineException"></exception>
    public string Serialize<T>(T data) where T : class
    {
        if (data == null)
        {
            throw new RedmineException(RedmineApiErrorCode.SerializationError, $"Could not serialize null of type {typeof(T).Name}");
        }
            
        var stringBuilder = new StringBuilder();

        using var sw = new StringWriter(stringBuilder);
        using var writer = new JsonTextWriter(sw);
#if DEBUG
        writer.Formatting = Formatting.Indented;
#endif
        if (_serializerSettings != null)
        {
            writer.DateFormatHandling = _serializerSettings.DateFormatHandling;
            writer.DateFormatString = _serializerSettings.DateFormatString;
            writer.DateTimeZoneHandling = _serializerSettings.DateTimeZoneHandling;
            writer.Formatting = _serializerSettings.Formatting;
            writer.Culture = _serializerSettings.Culture;
        }
        else
        {
            writer.DateFormatHandling = DateFormatHandling.IsoDateFormat;
        }

        var converter = JsonConverterRegistry.GetConverter<T>();
        converter.WriteJson(writer, data);

        var json = stringBuilder.ToString();
                    
        stringBuilder.Length = 0;

        return json;
    }

    public T? Deserialize<T>(string input) where T : new()
    {
        using var stringReader = new StringReader(input);
        using var jsonReader = new JsonTextReader(stringReader);

        var converter = JsonConverterRegistry.GetConverter<T>();
       
        if (jsonReader.Read() && jsonReader.Read())
        {
            return converter.ReadJson(jsonReader);
        }

        return default;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public string SerializeUserId(int userId)
    {
        return $$"""{"user_id":"{{userId.ToInvariantString()}}"}""";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public PagedResults<T> DeserializeToPagedResults<T>(string input) where T : class, new()
    {
        using var sr = new StringReader(input);
        using var reader = new JsonTextReader(sr);
        var total = 0;
        var offset = 0;
        var limit = 0;
        List<T>? list = null;

        while (reader.Read())
        {
            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
            {
                case RedmineKeys.TOTAL_COUNT:
                    total = reader.ReadAsInt32().GetValueOrDefault();
                    break;
                case RedmineKeys.OFFSET:
                    offset = reader.ReadAsInt32().GetValueOrDefault();
                    break;
                case RedmineKeys.LIMIT:
                    limit = reader.ReadAsInt32().GetValueOrDefault();
                    break;
                default:
                    list = reader.ReadAsCollection2<T>();
                    break;
            }
        }

        return new PagedResults<T>(list, total, offset, limit);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public int Count<T>(string input)
    {
        using var sr = new StringReader(input);
        using var reader = new JsonTextReader(sr);
        var total = 0;

        while (reader.Read())
        {
            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            if (reader.Value is not RedmineKeys.TOTAL_COUNT)
            {
                continue;
            }
            
            total = reader.ReadAsInt32().GetValueOrDefault();
            return total;
        }

        return total;
    }
}