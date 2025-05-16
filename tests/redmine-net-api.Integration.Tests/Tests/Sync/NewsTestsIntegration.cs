using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Sync;

[Collection(Constants.RedmineTestContainerCollection)]
public class NewsTests(RedmineTestContainerFixture fixture)
{
    private const string PROJECT_ID = "1";

    [Fact]
    public void GetAllNews_Should_Succeed()
    {
        _ = fixture.RedmineManager.AddProjectNews(PROJECT_ID, new News
        {
            Title       = RandomHelper.GenerateText(5),
            Summary     = RandomHelper.GenerateText(10),
            Description = RandomHelper.GenerateText(20),
        });

        _ = fixture.RedmineManager.AddProjectNews("2", new News
        {
            Title       = RandomHelper.GenerateText(5),
            Summary     = RandomHelper.GenerateText(10),
            Description = RandomHelper.GenerateText(20),
        });

        var news = fixture.RedmineManager.Get<News>();

        Assert.NotNull(news);
    }

    [Fact]
    public void GetProjectNews_Should_Succeed()
    {
        _ = fixture.RedmineManager.AddProjectNews(PROJECT_ID, new News
        {
            Title       = RandomHelper.GenerateText(5),
            Summary     = RandomHelper.GenerateText(10),
            Description = RandomHelper.GenerateText(20),
        });

        var news = fixture.RedmineManager.GetProjectNews(PROJECT_ID);

        Assert.NotNull(news);
    }
}