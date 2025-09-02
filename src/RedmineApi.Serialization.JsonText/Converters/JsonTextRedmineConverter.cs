using System.Text.Json;

namespace Padi.RedmineApi.Serialization.JsonText.Converters;

internal abstract class JsonTextRedmineConverter<T> :  IJsonTextRedmineConverter<T> where T : new()
{
    public abstract string RootName { get; }

    public abstract T? ReadJson(ref Utf8JsonReader reader);

    public abstract void WriteJson(Utf8JsonWriter writer, T value, bool writeRoot);
}
