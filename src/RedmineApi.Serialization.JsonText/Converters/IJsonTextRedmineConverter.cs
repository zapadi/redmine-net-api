using System.Text.Json;

namespace Padi.RedmineApi.Serialization.JsonText.Converters;

internal interface IJsonTextRedmineConverter<T>
{
    T? ReadJson(ref Utf8JsonReader reader);
    void WriteJson(Utf8JsonWriter writer, T value, bool writeRoot);
}
