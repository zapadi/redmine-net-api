using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Helpers;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class WikiTestsAsync(RedmineTestContainerFixture fixture)
{
    private const string PROJECT_ID = "1";
    private const string WIKI_PAGE_TITLE = "TestWikiPage";

    private async Task<WikiPage> CreateOrUpdateTestWikiPageAsync()
    {
        var wikiPage = new WikiPage
        {
            Title = WIKI_PAGE_TITLE,
            Text = $"Test wiki page content {Guid.NewGuid()}",
            Comments = "Initial wiki page creation"
        };

        return await fixture.RedmineManager.CreateWikiPageAsync(PROJECT_ID, wikiPage.Title, wikiPage);
    }

    [Fact]
    public async Task CreateOrUpdateWikiPage_Should_Succeed()
    {
        // Arrange
        var wikiPage = new WikiPage
        {
            Title = $"TestWikiPage_{Guid.NewGuid()}".Replace("-", "").Substring(0, 20),
            Text = "Test wiki page content",
            Comments = "Initial wiki page creation"
        };

        // Act
        var createdPage = await fixture.RedmineManager.CreateWikiPageAsync(PROJECT_ID, "wikiPageName", wikiPage, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(createdPage);
    }

    [Fact]
    public async Task GetWikiPage_Should_Succeed()
    {
        // Arrange
        var (page, _, pageTitle) = await CreateTestWikiPageAsync();
        
        // Act
        var retrievedPage = await fixture.RedmineManager.GetWikiPageAsync(PROJECT_ID, pageTitle, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(page);
        Assert.NotNull(retrievedPage);
        Assert.Equal(pageTitle, retrievedPage.Title);
        Assert.Equal(page.Text, retrievedPage.Text);
    }

    [Fact]
    public async Task GetAllWikiPages_Should_Succeed()
    {
        // Arrange
        await CreateOrUpdateTestWikiPageAsync();

        // Act
        var wikiPages = await fixture.RedmineManager.GetAllWikiPagesAsync(PROJECT_ID, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(wikiPages);
        Assert.NotEmpty(wikiPages);
    }

    [Fact]
    public async Task DeleteWikiPage_Should_Succeed()
    {
        // Arrange
        var wikiPageName = RandomHelper.GenerateText(7);

        var wikiPage = new WikiPage
        {
            Title = RandomHelper.GenerateText(5),
            Text = "Test wiki page content for deletion",
            Comments = "Initial wiki page creation for deletion test"
        };

        var createdPage = await fixture.RedmineManager.CreateWikiPageAsync(PROJECT_ID, wikiPageName, wikiPage, cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(createdPage);

        // Act
        await fixture.RedmineManager.DeleteWikiPageAsync(PROJECT_ID, wikiPageName, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        var ex = await Assert.ThrowsAsync<RedmineApiException>(async () =>
            await fixture.RedmineManager.GetWikiPageAsync(PROJECT_ID, wikiPageName, cancellationToken: TestContext.Current.CancellationToken));
        
        Assert.NotNull(ex);
        Assert.Equal(HttpConstants.StatusCodes.NotFound, ex.HttpStatusCode);
    }

     private async Task<(WikiPage? Page, string ProjectId, string PageTitle)> CreateTestWikiPageAsync(
        string? pageTitleSuffix = null,
        string initialText = "Default initial text for wiki page.",
        string initialComments = "Initial comments for wiki page.")
    {
        var pageTitle = $"TestWikiPage_{(pageTitleSuffix ?? RandomHelper.GenerateText(5))}";
        var wikiPageData = new WikiPage
        {
            Text = initialText,
            Comments = initialComments
        };

        var createdPage = await fixture.RedmineManager.CreateWikiPageAsync(PROJECT_ID, pageTitle, wikiPageData);

        return (createdPage, PROJECT_ID, pageTitle);
    }

    [Fact]
    public async Task CreateWikiPage_Should_Succeed()
    {
        //Arrange
        var pageTitle = RandomHelper.GenerateText("NewWikiPage");
        var text = "This is the content of a new wiki page.";
        var comments = "Creation comment for new wiki page.";
        var wikiPageData = new WikiPage { Text = text, Comments = comments };

        //Act
        var createdPage = await fixture.RedmineManager.CreateWikiPageAsync(PROJECT_ID, pageTitle, wikiPageData, cancellationToken: TestContext.Current.CancellationToken);

        //Assert
        Assert.NotNull(createdPage);
        Assert.Equal(pageTitle, createdPage.Title);
        Assert.Equal(text, createdPage.Text);
        Assert.True(createdPage.Version >= 0);

    }

    [Fact]
    public async Task UpdateWikiPage_Should_Succeed()
    {
        //Arrange
        var (initialPage, projectId, pageTitle) = await CreateTestWikiPageAsync("UpdateTest", "Original Text.", "Original Comments.");

        var updatedText = $"Updated wiki text content {Guid.NewGuid():N}";
        var updatedComments = $"These are updated comments for the wiki page update({DateTime.Now.Ticks.ToInvariantString()}).";

        var version = -1;

        if (initialPage is not null)
        {
            version = initialPage.Version;
        }

        var wikiPageToUpdate = new WikiPage
        {
            Text = updatedText,
            Comments = updatedComments,
            Version = ++version
        };

        //Act
        await fixture.RedmineManager.UpdateWikiPageAsync(projectId, pageTitle, wikiPageToUpdate, cancellationToken: TestContext.Current.CancellationToken);

        WikiPage? retrievedPage = null;
        if (initialPage is not null)
        {
             retrievedPage = await fixture.RedmineManager.GetAsync<WikiPage>(initialPage.Id.ToInvariantString(), cancellationToken: TestContext.Current.CancellationToken);
        }
        else
        {
            retrievedPage = await fixture.RedmineManager.GetWikiPageAsync(PROJECT_ID, pageTitle, cancellationToken: TestContext.Current.CancellationToken);
        }

        //Assert
        Assert.NotNull(retrievedPage);
        Assert.Equal(updatedText, retrievedPage.Text);
        Assert.Equal(updatedComments, retrievedPage.Comments);

        Assert.True(retrievedPage.Version > version
                    || (retrievedPage.Version == 1 && version == 0)
                    || (retrievedPage.Version ==0 && version ==0));
    }
}
