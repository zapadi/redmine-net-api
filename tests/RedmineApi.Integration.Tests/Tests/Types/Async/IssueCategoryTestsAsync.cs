using Padi.RedmineApi.Exceptions;
using Padi.RedmineApi.Extensions;
using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Helpers;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Padi.RedmineApi.Internals;
using Padi.RedmineApi.Types;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class IssueCategoryTestsAsync(RedmineTestContainerFixture fixture)
{
    private const string PROJECT_ID = "1";

    private async Task<IssueCategory> CreateTestProjectIssueCategoryAsync()
    {
        var category = new IssueCategory
        {
            Name = RandomHelper.GenerateText(5)
        };

        return await fixture.RedmineManager.CreateAsync(category, PROJECT_ID);
    }

    [Fact]
    public async Task GetProjectIssueCategories_Should_Succeed()
    {
        // Act
        var categories = await fixture.RedmineManager.GetProjectIssueCategoriesAsync(PROJECT_ID);

        // Assert
        Assert.NotNull(categories);
        Assert.NotNull(categories.Items);
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
        var createdCategory = await CreateTestProjectIssueCategoryAsync();
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
        var createdCategory = await CreateTestProjectIssueCategoryAsync();
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
        var createdCategory = await CreateTestProjectIssueCategoryAsync();
        Assert.NotNull(createdCategory);

        var categoryId = createdCategory.Id.ToInvariantString();

        // Act
        await fixture.RedmineManager.DeleteAsync<IssueCategory>(categoryId);

        // Assert
        var ex = await Assert.ThrowsAsync<RedmineApiException>(async () =>
            await fixture.RedmineManager.GetAsync<IssueCategory>(categoryId));
        Assert.NotNull(ex);
        Assert.Equal(HttpConstants.StatusCodes.NotFound, ex.HttpStatusCode);
    }
}
