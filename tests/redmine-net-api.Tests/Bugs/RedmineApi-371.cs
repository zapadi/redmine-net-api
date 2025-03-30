using System.Collections.Specialized;
using Padi.DotNet.RedmineAPI.Tests.Tests;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Net;
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
        var result = _fixture.Sut.GetListFragment<IssueCategory>(
            new RequestOptions
            {
                QueryString = new NameValueCollection{ { "project_id", 1.ToInvariantString() } }
            });
        
        Assert.Equal($"projects/1/issue_categories.{_fixture.Format}", result);
    }
}