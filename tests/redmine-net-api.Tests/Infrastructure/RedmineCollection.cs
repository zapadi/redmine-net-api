#if !(NET20 || NET40)
using Xunit;

namespace Padi.RedmineApi.Tests.Infrastructure
{
	[CollectionDefinition("RedmineCollection")]
	public class RedmineCollection : ICollectionFixture<RedmineFixture>
	{
		
	}
}
#endif