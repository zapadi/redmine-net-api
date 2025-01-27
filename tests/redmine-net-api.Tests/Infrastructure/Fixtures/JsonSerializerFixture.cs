using Redmine.Net.Api.Serialization;

namespace Padi.DotNet.RedmineAPI.Tests.Infrastructure.Fixtures;

public sealed class JsonSerializerFixture
{
    internal IRedmineSerializer Serializer { get; private set; } = new JsonRedmineSerializer();
    
}