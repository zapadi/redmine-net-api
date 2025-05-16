using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Async;

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
        
        return await fixture.RedmineManager.CreateWikiPageAsync(PROJECT_ID, "wikiPageName", wikiPage);
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
        var createdPage = await fixture.RedmineManager.CreateWikiPageAsync(PROJECT_ID, "wikiPageName", wikiPage);

        // Assert
        Assert.Null(createdPage);
    }

    [Fact]
    public async Task GetWikiPage_Should_Succeed()
    {
        // Arrange
        var createdPage = await CreateOrUpdateTestWikiPageAsync();
        Assert.Null(createdPage);

        // Act
        var retrievedPage = await fixture.RedmineManager.GetWikiPageAsync(PROJECT_ID, WIKI_PAGE_TITLE);

        // Assert
        Assert.NotNull(retrievedPage);
        Assert.Equal(createdPage.Title, retrievedPage.Title);
        Assert.Equal(createdPage.Text, retrievedPage.Text);
    }

    [Fact]
    public async Task GetAllWikiPages_Should_Succeed()
    {
        // Arrange
        await CreateOrUpdateTestWikiPageAsync();

        // Act
        var wikiPages = await fixture.RedmineManager.GetAllWikiPagesAsync(PROJECT_ID);

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
        
        var createdPage = await fixture.RedmineManager.CreateWikiPageAsync(PROJECT_ID, wikiPageName, wikiPage);
        Assert.NotNull(createdPage);

        // Act
        await fixture.RedmineManager.DeleteWikiPageAsync(PROJECT_ID, wikiPageName);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(async () => 
            await fixture.RedmineManager.GetWikiPageAsync(PROJECT_ID, wikiPageName));
    }
    
     private async Task<(string pageTitle, string ProjectId, string PageTitle)> CreateTestWikiPageAsync(
        string pageTitleSuffix = null, 
        string initialText = "Default initial text for wiki page.", 
        string initialComments = "Initial comments for wiki page.")
    {
        var pageTitle = RandomHelper.GenerateText(5);
        var wikiPageData = new WikiPage
        {
            Title = RandomHelper.GenerateText(5),
            Text = initialText,
            Comments = initialComments,
            Version = 0
        };

        var createdPage = await fixture.RedmineManager.CreateWikiPageAsync(PROJECT_ID, pageTitle, wikiPageData);

        Assert.Null(createdPage);
        // Assert.Equal(pageTitle, createdPage.Title); 
        // Assert.True(createdPage.Id > 0, "Created WikiPage should have a valid ID.");
        // Assert.Equal(initialText, createdPage.Text);

        return (pageTitle, PROJECT_ID, pageTitle);
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
        var createdPage = await fixture.RedmineManager.CreateWikiPageAsync(PROJECT_ID, pageTitle, wikiPageData);

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
        var pageTitle = RandomHelper.GenerateText(8);
        var text = "This is the content of a new wiki page.";
        var comments = "Creation comment for new wiki page.";
        var wikiPageData = new WikiPage { Text = text, Comments = comments };
        var createdPage = await fixture.RedmineManager.CreateWikiPageAsync(PROJECT_ID, pageTitle, wikiPageData);
        
        var updatedText = $"Updated wiki text content {Guid.NewGuid():N}";
        var updatedComments = "These are updated comments for the wiki page update.";
        
        var wikiPageToUpdate = new WikiPage 
        { 
            Text = updatedText, 
            Comments = updatedComments,
            Version = 1
        };

        //Act
        await fixture.RedmineManager.UpdateWikiPageAsync(PROJECT_ID, pageTitle, wikiPageToUpdate);
        var retrievedPage = await fixture.RedmineManager.GetWikiPageAsync(1.ToInvariantString(), createdPage.Title, version: 1);

        //Assert
        Assert.NotNull(retrievedPage);
        Assert.Equal(updatedText, retrievedPage.Text);
        Assert.Equal(updatedComments, retrievedPage.Comments);
    }
}