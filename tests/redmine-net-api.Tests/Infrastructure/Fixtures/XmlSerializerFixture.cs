using Redmine.Net.Api.Serialization;
using Redmine.Net.Api.Serialization.Xml;

namespace Padi.DotNet.RedmineAPI.Tests.Infrastructure.Fixtures;

public sealed class XmlSerializerFixture
{
    internal IRedmineSerializer Serializer { get; private set; } = new XmlRedmineSerializer();
}