using System.Collections.Specialized;
using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Common;
using Redmine.Net.Api;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Http;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.IssueCategory;

[Collection(Constants.RedmineTestContainerCollection)]
public class IssueCategoryTests(RedmineTestContainerFixture fixture)
{
    private const string PROJECT_ID = TestConstants.Projects.DefaultProjectIdentifier;

    private Redmine.Net.Api.Types.IssueCategory CreateCategory()
    {
        return fixture.RedmineManager.Create(
            new Redmine.Net.Api.Types.IssueCategory { Name = $"Test Category {Guid.NewGuid()}" },
            PROJECT_ID);
    }

    [Fact]
    public void GetProjectIssueCategories_Should_Succeed() =>
        Assert.NotNull(fixture.RedmineManager.Get<Redmine.Net.Api.Types.IssueCategory>(new RequestOptions()
        {
            QueryString = new NameValueCollection()
            {
                {RedmineKeys.PROJECT_ID, PROJECT_ID}
            }
        }));

    [Fact]
    public void CreateIssueCategory_Should_Succeed()
    {
        var cat = new Redmine.Net.Api.Types.IssueCategory { Name = $"Cat {Guid.NewGuid()}" };
        var created = fixture.RedmineManager.Create(cat, PROJECT_ID);

        Assert.True(created.Id > 0);
        Assert.Equal(cat.Name, created.Name);
    }

    [Fact]
    public void GetIssueCategory_Should_Succeed()
    {
        var created = CreateCategory();
        var retrieved = fixture.RedmineManager.Get<Redmine.Net.Api.Types.IssueCategory>(created.Id.ToInvariantString());

        Assert.Equal(created.Id, retrieved.Id);
        Assert.Equal(created.Name, retrieved.Name);
    }

    [Fact]
    public void UpdateIssueCategory_Should_Succeed()
    {
        var created = CreateCategory();
        created.Name = $"Updated {Guid.NewGuid()}";

        fixture.RedmineManager.Update(created.Id.ToInvariantString(), created);
        var retrieved = fixture.RedmineManager.Get<Redmine.Net.Api.Types.IssueCategory>(created.Id.ToInvariantString());

        Assert.Equal(created.Name, retrieved.Name);
    }

    [Fact]
    public void DeleteIssueCategory_Should_Succeed()
    {
        var created = CreateCategory();
        var id = created.Id.ToInvariantString();

        fixture.RedmineManager.Delete<Redmine.Net.Api.Types.IssueCategory>(id);

        Assert.Throws<RedmineNotFoundException>(() => fixture.RedmineManager.Get<Redmine.Net.Api.Types.IssueCategory>(id));
    }
}