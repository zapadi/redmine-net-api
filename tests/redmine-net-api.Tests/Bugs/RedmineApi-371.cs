using System.Collections.Specialized;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Bugs;

public sealed class RedmineApi371 : IClassFixture<RedmineFixture>
{
    private readonly RedmineFixture _fixture;

    public RedmineApi371(RedmineFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public void Should_Return_IssueCategories()
    {
        var result = _fixture.RedmineManager.Get<IssueCategory>(new RequestOptions()
        {
            QueryString = new NameValueCollection()
            {
                { "project_id", 1.ToInvariantString() }
            }
        });
    }
}