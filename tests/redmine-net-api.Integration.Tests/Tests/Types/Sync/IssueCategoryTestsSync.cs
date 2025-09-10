using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Sync;

[Collection(Constants.RedmineTestContainerCollection)]
public class IssueCategoryTests(RedmineTestContainerFixture fixture)
{
    private const string PROJECT_ID = "1";

    private IssueCategory CreateCategory()
    {
        return fixture.RedmineManager.Create(
            new IssueCategory { Name = $"Test Category {Guid.NewGuid()}" },
            PROJECT_ID);
    }

    [Fact]
    public void GetProjectIssueCategories_Should_Succeed() =>
        Assert.NotNull(fixture.RedmineManager.Get<IssueCategory>(PROJECT_ID));

    [Fact]
    public void CreateIssueCategory_Should_Succeed()
    {
        var cat = new IssueCategory { Name = $"Cat {Guid.NewGuid()}" };
        var created = fixture.RedmineManager.Create(cat, PROJECT_ID);

        Assert.True(created.Id > 0);
        Assert.Equal(cat.Name, created.Name);
    }

    [Fact]
    public void GetIssueCategory_Should_Succeed()
    {
        var created = CreateCategory();
        var retrieved = fixture.RedmineManager.Get<IssueCategory>(created.Id.ToInvariantString());

        Assert.Equal(created.Id, retrieved.Id);
        Assert.Equal(created.Name, retrieved.Name);
    }

    [Fact]
    public void UpdateIssueCategory_Should_Succeed()
    {
        var created = CreateCategory();
        created.Name = $"Updated {Guid.NewGuid()}";

        fixture.RedmineManager.Update(created.Id.ToInvariantString(), created);
        var retrieved = fixture.RedmineManager.Get<IssueCategory>(created.Id.ToInvariantString());

        Assert.Equal(created.Name, retrieved.Name);
    }

    [Fact]
    public void DeleteIssueCategory_Should_Succeed()
    {
        var created = CreateCategory();
        var id = created.Id.ToInvariantString();

        fixture.RedmineManager.Delete<IssueCategory>(id);

        var ex = Assert.Throws<RedmineApiException>(() => fixture.RedmineManager.Get<IssueCategory>(id));
        Assert.NotNull(ex);
        Assert.Equal(HttpConstants.StatusCodes.NotFound, ex.HttpStatusCode);
    }
}