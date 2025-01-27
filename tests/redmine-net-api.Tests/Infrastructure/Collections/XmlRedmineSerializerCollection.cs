using Padi.DotNet.RedmineAPI.Tests.Infrastructure.Fixtures;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Infrastructure.Collections;

[CollectionDefinition(Constants.XmlRedmineSerializerCollection)]
public sealed class XmlRedmineSerializerCollection : ICollectionFixture<XmlSerializerFixture> { }