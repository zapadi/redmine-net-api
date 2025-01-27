using Padi.DotNet.RedmineAPI.Tests.Infrastructure.Fixtures;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Infrastructure.Collections;

[CollectionDefinition(Constants.JsonRedmineSerializerCollection)]
public sealed class JsonRedmineSerializerCollection : ICollectionFixture<JsonSerializerFixture> { }