using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Helpers;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class NewsTestsAsync(RedmineTestContainerFixture fixture)
{
    private const string PROJECT_ID = "1";

    [Fact]
    public async Task GetAllNews_Should_Succeed()
    {
        _ = await fixture.RedmineManager.AddProjectNewsAsync(PROJECT_ID, new News
        {
            Title       = RandomHelper.GenerateText(5),
            Summary     = RandomHelper.GenerateText(10),
            Description = RandomHelper.GenerateText(20),
        }, cancellationToken: TestContext.Current.CancellationToken);

        var project = fixture.RedmineManager.CreateAsync(new Project()
        {
            Identifier = RandomHelper.GenerateText(lowerCase: true),
            Name = RandomHelper.GenerateText(5),
        }, cancellationToken: TestContext.Current.CancellationToken);
        
        _ = await fixture.RedmineManager.AddProjectNewsAsync(project.Id.ToInvariantString(), new News
        {
            Title       = RandomHelper.GenerateText(5),
            Summary     = RandomHelper.GenerateText(10),
            Description = RandomHelper.GenerateText(20),
        }, cancellationToken: TestContext.Current.CancellationToken);

        var news = await fixture.RedmineManager.GetAsync<News>(cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(news);
    }

    [Fact]
    public async Task GetProjectNews_Should_Succeed()
    {
        _ = await fixture.RedmineManager.AddProjectNewsAsync(PROJECT_ID, new News
        {
            Title       = RandomHelper.GenerateText(5),
            Summary     = RandomHelper.GenerateText(10),
            Description = RandomHelper.GenerateText(20),
        }, cancellationToken: TestContext.Current.CancellationToken);

        var news = await fixture.RedmineManager.GetProjectNewsAsync(PROJECT_ID, cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(news);
    }
}
