using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Redmine.Net.Api.JSonConverters2
{
    //public interface IJsonConverter<T> : IJsonConverter
    //{
    //    void Serialize(JsonWriter writer, T obj, JsonSerializer serializer);
    //    new T Deserialize(JObject obj, JsonSerializer serializer);
    //}

    public interface IJsonConverter
    {
        void Serialize(JsonWriter writer, object obj, JsonSerializer serializer);
        object Deserialize(JObject obj, JsonSerializer serializer);
    }
}
