using Xunit;
using Redmine.Net.Api.Types;

namespace xUnitTestredminenet45api
{
	[Collection("RedmineCollection")]
	public class QueryTests
	{
		private const int NUMBER_OF_QUERIES = 2;
		private const bool EXISTS_PUBLIC_QUERY = true;

	    private readonly RedmineFixture fixture;
		public QueryTests (RedmineFixture fixture)
		{
			this.fixture = fixture;
		}

		[Fact]
		public void Should_Get_All_Queries()
		{
			var queries = fixture.RedmineManager.GetObjects<Query>(null);

			Assert.NotNull(queries);
			Assert.True(queries.Count == NUMBER_OF_QUERIES, "Queries count != " + NUMBER_OF_QUERIES);
			Assert.All (queries, q => Assert.IsType<Query> (q));

			Assert.True(queries.Exists(q => q.IsPublic) == EXISTS_PUBLIC_QUERY, EXISTS_PUBLIC_QUERY ? "Public query should exist." : "Public query should not exist.");
		}
	}
}