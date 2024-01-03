#if !(NET20 || NET40)
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Infrastructure
{
	[CollectionDefinition("RedmineCollection")]
	public sealed class RedmineCollection : ICollectionFixture<RedmineFixture> { }
}
#endif