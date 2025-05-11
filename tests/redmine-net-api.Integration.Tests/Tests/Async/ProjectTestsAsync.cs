using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class ProjectTestsAsync(RedmineTestContainerFixture fixture)
{
    private async Task<Project> CreateEntityAsync(string subjectSuffix = null)
    {
        var entity = new Project
        {
            Identifier = Guid.NewGuid().ToString("N"),
            Name = "test-random",
        };
        
        return await fixture.RedmineManager.CreateAsync(entity);
    }
    
    [Fact]
    public async Task CreateProject_Should_Succeed()
    {
        var data = new Project
        {
            IsPublic = true,
            EnabledModules = [
                new ProjectEnabledModule("files"), 
                new ProjectEnabledModule("wiki")
            ],
            Identifier = Guid.NewGuid().ToString("N"),
            InheritMembers = true,
            Name = "test-random",
            HomePage = "test-homepage",
            Trackers =
            [
                new ProjectTracker(1), 
                new ProjectTracker(2), 
                new ProjectTracker(3),
            ],
            Description = $"Description for create test",
            CustomFields =
            [
                new IssueCustomField 
                { 
                    Id = 1, 
                    Values = [ 
                        new CustomFieldValue
                        {
                            Info = "Custom field test value"
                        } 
                    ] 
                }
            ]
        };

        //Act
        var createdProject = await fixture.RedmineManager.CreateAsync(data);
        Assert.NotNull(createdProject);
    }
    
    [Fact]
    public async Task DeleteIssue_Should_Succeed()
    {
        //Arrange
        var createdEntity = await CreateEntityAsync("DeleteTest");
        Assert.NotNull(createdEntity);
        
        var id = createdEntity.Id.ToInvariantString();

        //Act
        await fixture.RedmineManager.DeleteAsync<Project>(id);

        await Task.Delay(200);
        
        //Assert
        await Assert.ThrowsAsync<NotFoundException>(TestCode);
        return;

        async Task TestCode()
        {
            await fixture.RedmineManager.GetAsync<Project>(id);
        }
    }
}