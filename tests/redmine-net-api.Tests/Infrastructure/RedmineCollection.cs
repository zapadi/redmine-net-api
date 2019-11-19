#if !(NET20 || NET40)
using Xunit;

namespace redmine.net.api.Tests.Infrastructure
{
	[CollectionDefinition("RedmineCollection")]
	public class RedmineCollection : ICollectionFixture<RedmineFixture>
	{
		
	}
}
#endif