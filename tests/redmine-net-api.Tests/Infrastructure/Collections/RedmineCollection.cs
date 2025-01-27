#if !(NET20 || NET40)
using Padi.DotNet.RedmineAPI.Tests.Infrastructure.Fixtures;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Infrastructure.Collections
{
	[CollectionDefinition(Constants.RedmineCollection)]
	public sealed class RedmineCollection : ICollectionFixture<RedmineFixture> { }
}
#endif