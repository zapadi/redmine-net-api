using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;

[CollectionDefinition(Constants.RedmineTestContainerCollection)]
public sealed class RedmineTestContainerCollection : ICollectionFixture<RedmineTestContainerFixture> { }