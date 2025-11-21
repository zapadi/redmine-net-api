using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Xunit;

namespace Padi.RedmineAPI.Integration.Tests.Collections;

[CollectionDefinition(Constants.RedmineTestContainerCollection)]
public sealed class RedmineTestContainerCollection : ICollectionFixture<RedmineTestContainerFixture> { }