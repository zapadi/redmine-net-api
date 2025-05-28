using System.Collections.Specialized;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure.Fixtures;
using Redmine.Net.Api;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Http;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Bugs;

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