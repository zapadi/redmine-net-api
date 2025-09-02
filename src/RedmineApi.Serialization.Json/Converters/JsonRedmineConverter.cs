using Newtonsoft.Json;

namespace Padi.RedmineApi.Serialization.Json.Converters;

public abstract class JsonRedmineConverter<T>
{
    public abstract string RootName { get; }
    public abstract T? ReadJson(JsonReader reader);
    public abstract void WriteJson(JsonWriter writer, T value);
}