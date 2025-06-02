using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Helpers;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Common;
using Redmine.Net.Api;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Http;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.Wiki;

[Collection(Constants.RedmineTestContainerCollection)]
public class WikiTestsAsync(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task CreateWikiPage_WithValidData_ShouldSucceed()
    {
        // Arrange
        var (pageName, wikiPagePayload) = TestEntityFactory.CreateRandomWikiPagePayload();
        
        // Act
        var wikiPage = await fixture.RedmineManager.CreateWikiPageAsync(TestConstants.Projects.DefaultProjectIdentifier, pageName, wikiPagePayload);
       
        // Assert
        Assert.NotNull(wikiPage);
    }

    [Fact]
    public async Task GetWikiPage_WithValidTitle_ShouldReturnPage()
    {
        // Arrange
        var (pageName, wikiPagePayload) = TestEntityFactory.CreateRandomWikiPagePayload();
        
        // Act
        var wikiPage = await fixture.RedmineManager.CreateWikiPageAsync(TestConstants.Projects.DefaultProjectIdentifier, pageName, wikiPagePayload);
        Assert.NotNull(wikiPage);

        var retrievedPage = await fixture.RedmineManager.GetWikiPageAsync(TestConstants.Projects.DefaultProjectIdentifier, wikiPage.Title);

        // Assert
        Assert.NotNull(retrievedPage);
        Assert.Equal(pageName, retrievedPage.Title);
        Assert.Equal(wikiPage.Text, retrievedPage.Text);
    }

    [Fact]
    public async Task GetAllWikiPages_ForValidProject_ShouldReturnPages()
    {
        // Arrange
        var (firstPageName, firstWikiPagePayload) = TestEntityFactory.CreateRandomWikiPagePayload();
        var (secondPageName, secondWikiPagePayload) = TestEntityFactory.CreateRandomWikiPagePayload();
        
        // Act
        var wikiPage = await fixture.RedmineManager.CreateWikiPageAsync(TestConstants.Projects.DefaultProjectIdentifier, firstPageName, firstWikiPagePayload);
        Assert.NotNull(wikiPage);
        
        var wikiPage2 = await fixture.RedmineManager.CreateWikiPageAsync(TestConstants.Projects.DefaultProjectIdentifier, secondPageName, secondWikiPagePayload);
        Assert.NotNull(wikiPage2);

        // Act
        var wikiPages = await fixture.RedmineManager.GetAllWikiPagesAsync(TestConstants.Projects.DefaultProjectIdentifier);

        // Assert
        Assert.NotNull(wikiPages);
        Assert.NotEmpty(wikiPages);
    }

    [Fact]
    public async Task DeleteWikiPage_WithValidTitle_ShouldSucceed()
    {
        // Arrange
        var (pageName, wikiPagePayload) = TestEntityFactory.CreateRandomWikiPagePayload();
        
        // Act
        var wikiPage = await fixture.RedmineManager.CreateWikiPageAsync(TestConstants.Projects.DefaultProjectIdentifier, pageName, wikiPagePayload);
        Assert.NotNull(wikiPage);
        
        // Act
        await fixture.RedmineManager.DeleteWikiPageAsync(TestConstants.Projects.DefaultProjectIdentifier, wikiPage.Title);
        
        // Assert
        await Assert.ThrowsAsync<RedmineNotFoundException>(async () => await fixture.RedmineManager.GetWikiPageAsync(TestConstants.Projects.DefaultProjectIdentifier, wikiPage.Title));
    }

    [Fact]
    public async Task CreateWikiPage_Should_Succeed()
    {
        // Arrange
        var (pageName, wikiPagePayload) = TestEntityFactory.CreateRandomWikiPagePayload();
        
        // Act
        var wikiPage = await fixture.RedmineManager.CreateWikiPageAsync(TestConstants.Projects.DefaultProjectIdentifier, pageName, wikiPagePayload);
        
        // Assert
        Assert.NotNull(wikiPage);
        Assert.NotNull(wikiPage.Author);
        Assert.NotNull(wikiPage.CreatedOn);
        Assert.Equal(DateTime.Now, wikiPage.CreatedOn.Value, TimeSpan.FromSeconds(5));
        Assert.Equal(pageName, wikiPage.Title);
        Assert.Equal(wikiPagePayload.Text, wikiPage.Text);
        Assert.Equal(1, wikiPage.Version);
    }

    [Fact]
    public async Task UpdateWikiPage_WithValidData_ShouldSucceed()
    {
        // Arrange
        var (pageName, wikiPagePayload) = TestEntityFactory.CreateRandomWikiPagePayload();
        
        var wikiPage = await fixture.RedmineManager.CreateWikiPageAsync(TestConstants.Projects.DefaultProjectIdentifier, pageName, wikiPagePayload);
        Assert.NotNull(wikiPage);
        
        wikiPage.Text = "Updated wiki text content";
        wikiPage.Comments = "These are updated comments for the wiki page update.";

        // Act
        await fixture.RedmineManager.UpdateWikiPageAsync(TestConstants.Projects.DefaultProjectIdentifier, pageName, wikiPage);
        
        var retrievedPage = await fixture.RedmineManager.GetWikiPageAsync(TestConstants.Projects.DefaultProjectIdentifier, wikiPage.Title);

        // Assert
        Assert.NotNull(retrievedPage);
        Assert.Equal(wikiPage.Text, retrievedPage.Text);
        Assert.Equal(wikiPage.Comments, retrievedPage.Comments);
    }

    [Fact]
    public async Task GetWikiPage_WithNameAndAttachments_ShouldReturnCompleteData()
    {
        // Arrange
        var fileUpload = await FileTestHelper.UploadRandom500KbFileAsync(fixture.RedmineManager);
        Assert.NotNull(fileUpload);
        Assert.NotEmpty(fileUpload.Token);

        fileUpload.ContentType = "text/plain";
        fileUpload.Description = RandomHelper.GenerateText(15);
        fileUpload.FileName = "hello-world.txt";
        
        var (pageName, wikiPagePayload) = TestEntityFactory.CreateRandomWikiPagePayload(pageName: RandomHelper.GenerateText(prefix: "Te$t"), uploads: [fileUpload]);
        
        var wikiPage = await fixture.RedmineManager.CreateWikiPageAsync(TestConstants.Projects.DefaultProjectIdentifier, pageName, wikiPagePayload);
        Assert.NotNull(wikiPage);
        
        // Act
        var page = await fixture.RedmineManager.GetWikiPageAsync(TestConstants.Projects.DefaultProjectIdentifier, pageName, RequestOptions.Include(RedmineKeys.ATTACHMENTS));

        // Assert
        Assert.NotNull(page);
        Assert.Equal(pageName, page.Title);
        Assert.NotNull(page.Comments);
        Assert.NotNull(page.Author);
        Assert.NotNull(page.CreatedOn);
        Assert.Equal(DateTime.Now, page.CreatedOn.Value, TimeSpan.FromSeconds(5));
        
        Assert.NotNull(page.Attachments);
        Assert.NotEmpty(page.Attachments);
        
        var attachment = page.Attachments.FirstOrDefault(x => x.FileName == fileUpload.FileName);
        Assert.NotNull(attachment);
        Assert.Equal("text/plain", attachment.ContentType);
        Assert.NotNull(attachment.Description);
        Assert.Equal(attachment.FileName, attachment.FileName);
        Assert.EndsWith($"/attachments/download/{attachment.Id}/{attachment.FileName}", attachment.ContentUrl);
        Assert.True(attachment.FileSize > 0);
    }

    [Fact]
    public async Task GetWikiPage_WithOldVersion_ShouldReturnHistoricalData()
    {
        //Arrange
        var (pageName, wikiPagePayload) = TestEntityFactory.CreateRandomWikiPagePayload();
        
        var wikiPage = await fixture.RedmineManager.CreateWikiPageAsync(TestConstants.Projects.DefaultProjectIdentifier, pageName, wikiPagePayload);
        Assert.NotNull(wikiPage);
        
        wikiPage.Text = RandomHelper.GenerateText(8);
        wikiPage.Comments = RandomHelper.GenerateText(9);

        // Act
        await fixture.RedmineManager.UpdateWikiPageAsync(TestConstants.Projects.DefaultProjectIdentifier, pageName, wikiPage);
        
        var oldPage = await fixture.RedmineManager.GetWikiPageAsync(TestConstants.Projects.DefaultProjectIdentifier, wikiPage.Title, version: 1);

        // Assert
        Assert.NotNull(oldPage);
        Assert.Equal(wikiPagePayload.Text, oldPage.Text);
        Assert.Equal(wikiPagePayload.Comments, oldPage.Comments);
        Assert.Equal(1, oldPage.Version);
    }

    [Fact]
    public async Task GetWikiPage_WithSpecialChars_ShouldReturnPage()
    {
        //Arrange
        var (pageName, wikiPagePayload) = TestEntityFactory.CreateRandomWikiPagePayload(pageName: "some-page-with-umlauts-and-other-special-chars-äöüÄÖÜß");
     
        var wikiPage = await fixture.RedmineManager.CreateWikiPageAsync(TestConstants.Projects.DefaultProjectIdentifier, pageName, wikiPagePayload);
        Assert.Null(wikiPage); //it seems that Redmine returns 204 (No content) when the page name contains special characters
        
        // Act
        var page = await fixture.RedmineManager.GetWikiPageAsync(TestConstants.Projects.DefaultProjectIdentifier, pageName);

        // Assert
        Assert.NotNull(page);
        Assert.Equal(pageName, page.Title);
    }
}