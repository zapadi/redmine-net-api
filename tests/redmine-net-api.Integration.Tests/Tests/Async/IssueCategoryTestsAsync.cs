using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class IssueCategoryTestsAsync(RedmineTestContainerFixture fixture)
{
    private const string PROJECT_ID = "1";
    
    private async Task<IssueCategory> CreateTestIssueCategoryAsync()
    {
        var category = new IssueCategory
        {
            Name = $"Test Category {Guid.NewGuid()}"
        };
        
        return await fixture.RedmineManager.CreateAsync(category, PROJECT_ID);
    }

    [Fact]
    public async Task GetProjectIssueCategories_Should_Succeed()
    {
        // Act
        var categories = await fixture.RedmineManager.GetAsync<IssueCategory>(PROJECT_ID);

        // Assert
        Assert.NotNull(categories);
    }

    [Fact]
    public async Task CreateIssueCategory_Should_Succeed()
    {
        // Arrange
        var category = new IssueCategory
        {
            Name = $"Test Category {Guid.NewGuid()}"
        };

        // Act
        var createdCategory = await fixture.RedmineManager.CreateAsync(category, PROJECT_ID);

        // Assert
        Assert.NotNull(createdCategory);
        Assert.True(createdCategory.Id > 0);
        Assert.Equal(category.Name, createdCategory.Name);
    }

    [Fact]
    public async Task GetIssueCategory_Should_Succeed()
    {
        // Arrange
        var createdCategory = await CreateTestIssueCategoryAsync();
        Assert.NotNull(createdCategory);

        // Act
        var retrievedCategory = await fixture.RedmineManager.GetAsync<IssueCategory>(createdCategory.Id.ToInvariantString());

        // Assert
        Assert.NotNull(retrievedCategory);
        Assert.Equal(createdCategory.Id, retrievedCategory.Id);
        Assert.Equal(createdCategory.Name, retrievedCategory.Name);
    }

    [Fact]
    public async Task UpdateIssueCategory_Should_Succeed()
    {
        // Arrange
        var createdCategory = await CreateTestIssueCategoryAsync();
        Assert.NotNull(createdCategory);

        var updatedName = $"Updated Test Category {Guid.NewGuid()}";
        createdCategory.Name = updatedName;

        // Act
        await fixture.RedmineManager.UpdateAsync(createdCategory.Id.ToInvariantString(), createdCategory);
        var retrievedCategory = await fixture.RedmineManager.GetAsync<IssueCategory>(createdCategory.Id.ToInvariantString());

        // Assert
        Assert.NotNull(retrievedCategory);
        Assert.Equal(createdCategory.Id, retrievedCategory.Id);
        Assert.Equal(updatedName, retrievedCategory.Name);
    }

    [Fact]
    public async Task DeleteIssueCategory_Should_Succeed()
    {
        // Arrange
        var createdCategory = await CreateTestIssueCategoryAsync();
        Assert.NotNull(createdCategory);

        var categoryId = createdCategory.Id.ToInvariantString();

        // Act
        await fixture.RedmineManager.DeleteAsync<IssueCategory>(categoryId);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => 
            await fixture.RedmineManager.GetAsync<IssueCategory>(categoryId));
    }
}