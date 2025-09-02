using System.Collections.Specialized;
using Padi.RedmineApi;
using Padi.RedmineApi.Extensions;
using Padi.RedmineApi.Net;
using Padi.RedmineAPI.Tests.Infrastructure.Fixtures;
using Padi.RedmineApi.Types;
using Xunit;

namespace Padi.RedmineAPI.Tests.Bugs;

public sealed class RedmineApi371 : IClassFixture<RedmineApiUrlsFixture>
{
    private readonly RedmineApiUrlsFixture _fixture;

    public RedmineApi371(RedmineApiUrlsFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public void Should_Return_IssueCategories_For_Project_Url()
    {
        var projectIdAsString = 1.ToInvariantString();
        var result = _fixture.Sut.GetListFragment<IssueCategory>(
            new RequestOptions
            {
                QueryString = new NameValueCollection{ { RedmineKeys.PROJECT_ID, projectIdAsString } }
            });
        
        Assert.Equal($"projects/{projectIdAsString}/issue_categories.{_fixture.Format}", result);
    }
}