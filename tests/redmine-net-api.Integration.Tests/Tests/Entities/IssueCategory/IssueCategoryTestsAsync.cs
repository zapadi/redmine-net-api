using System.Collections.Specialized;
using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Helpers;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Common;
using Redmine.Net.Api;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Http;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.IssueCategory;

[Collection(Constants.RedmineTestContainerCollection)]
public class IssueCategoryTestsAsync(RedmineTestContainerFixture fixture)
{
    private const string PROJECT_ID = TestConstants.Projects.DefaultProjectIdentifier;
    
    private async Task<Redmine.Net.Api.Types.IssueCategory> CreateRandomIssueCategoryAsync()
    {
        var category = new Redmine.Net.Api.Types.IssueCategory
        {
            Name = RandomHelper.GenerateText(5)
        };
        
        return await fixture.RedmineManager.CreateAsync(category, PROJECT_ID);
    }

    [Fact]
    public async Task GetProjectIssueCategories_Should_Succeed()
    {
        // Arrange
        var category = await CreateRandomIssueCategoryAsync();
        Assert.NotNull(category);
        
        // Act
        var categories = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.IssueCategory>(new RequestOptions()
        {
            QueryString = new NameValueCollection()
            {
                {RedmineKeys.PROJECT_ID, PROJECT_ID}
            }
        });

        // Assert
        Assert.NotNull(categories);
    }

    [Fact]
    public async Task CreateIssueCategory_Should_Succeed()
    {
        // Arrange & Act
        var category = await CreateRandomIssueCategoryAsync();

        // Assert
        Assert.NotNull(category);
        Assert.True(category.Id > 0);
    }

    [Fact]
    public async Task GetIssueCategory_Should_Succeed()
    {
        // Arrange
        var createdCategory = await CreateRandomIssueCategoryAsync();
        Assert.NotNull(createdCategory);

        // Act
        var retrievedCategory = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.IssueCategory>(createdCategory.Id.ToInvariantString());

        // Assert
        Assert.NotNull(retrievedCategory);
        Assert.Equal(createdCategory.Id, retrievedCategory.Id);
        Assert.Equal(createdCategory.Name, retrievedCategory.Name);
    }

    [Fact]
    public async Task UpdateIssueCategory_Should_Succeed()
    {
        // Arrange
        var createdCategory = await CreateRandomIssueCategoryAsync();
        Assert.NotNull(createdCategory);

        var updatedName = $"Updated Test Category {Guid.NewGuid()}";
        createdCategory.Name = updatedName;

        // Act
        await fixture.RedmineManager.UpdateAsync(createdCategory.Id.ToInvariantString(), createdCategory);
        var retrievedCategory = await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.IssueCategory>(createdCategory.Id.ToInvariantString());

        // Assert
        Assert.NotNull(retrievedCategory);
        Assert.Equal(createdCategory.Id, retrievedCategory.Id);
        Assert.Equal(updatedName, retrievedCategory.Name);
    }

    [Fact]
    public async Task DeleteIssueCategory_Should_Succeed()
    {
        // Arrange
        var createdCategory = await CreateRandomIssueCategoryAsync();
        Assert.NotNull(createdCategory);

        var categoryId = createdCategory.Id.ToInvariantString();

        // Act
        await fixture.RedmineManager.DeleteAsync<Redmine.Net.Api.Types.IssueCategory>(categoryId);

        // Assert
        await Assert.ThrowsAsync<RedmineNotFoundException>(async () => 
            await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.IssueCategory>(categoryId));
    }
}