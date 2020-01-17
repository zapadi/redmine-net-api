namespace Redmine.Net.Api.Serialization
{
    internal interface IRedmineSerializer
    {
        string Type { get; }

        string Serialize<T>(T obj) where T : class;

        PagedResults<T> DeserializeToPagedResults<T>(string response) where T : class, new();

        T Deserialize<T>(string response) where T : new();
    }
}