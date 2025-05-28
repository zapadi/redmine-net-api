using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Helpers;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api.Extensions;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.News;

[Collection(Constants.RedmineTestContainerCollection)]
public class NewsTests(RedmineTestContainerFixture fixture)
{
    [Fact]
    public void GetAllNews_Should_Succeed()
    {
        _ = fixture.RedmineManager.AddProjectNews(TestConstants.Projects.DefaultProjectIdentifier, TestEntityFactory.CreateRandomNewsPayload());

        _ = fixture.RedmineManager.AddProjectNews("2", TestEntityFactory.CreateRandomNewsPayload());

        var news = fixture.RedmineManager.Get<Redmine.Net.Api.Types.News>();

        Assert.NotNull(news);
    }

    [Fact]
    public void GetProjectNews_Should_Succeed()
    {
        _ = fixture.RedmineManager.AddProjectNews(TestConstants.Projects.DefaultProjectIdentifier, TestEntityFactory.CreateRandomNewsPayload());

        var news = fixture.RedmineManager.GetProjectNews(TestConstants.Projects.DefaultProjectIdentifier);

        Assert.NotNull(news);
    }
}