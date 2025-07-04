using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Helpers;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Entities.Project;

[Collection(Constants.RedmineTestContainerCollection)]
public class ProjectTestsAsync(RedmineTestContainerFixture fixture)
{
    private async Task<Redmine.Net.Api.Types.Project> CreateEntityAsync(string subjectSuffix = null)
    {
        var entity = new Redmine.Net.Api.Types.Project
        {
            Identifier = RandomHelper.GenerateText(5).ToLowerInvariant(),
            Name = "test-random",
        };
        
        return await fixture.RedmineManager.CreateAsync(entity);
    }

    [Fact]
    public async Task CreateProject_Should_Succeed()
    {
        //Arrange
        var projectName = RandomHelper.GenerateText(7);
        var data = new Redmine.Net.Api.Types.Project
        {
            Name = projectName,
            Identifier = projectName.ToLowerInvariant(),
            Description = RandomHelper.GenerateText(7),
            HomePage = RandomHelper.GenerateText(7),
            IsPublic = true,
            InheritMembers = true,
            
            EnabledModules = [
                new ProjectEnabledModule("files"), 
                new ProjectEnabledModule("wiki")
            ],

            Trackers =
            [
                new ProjectTracker(1), 
                new ProjectTracker(2), 
                new ProjectTracker(3),
            ],

            //CustomFieldValues = [IdentifiableName.Create<CustomField>(1, "cf1"), IdentifiableName.Create<CustomField>(2, "cf2")]
            // IssueCustomFields =
            // [
            //      IssueCustomField.CreateSingle(1, RandomHelper.GenerateText(5), RandomHelper.GenerateText(7)) 
            // ]
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
        await fixture.RedmineManager.DeleteAsync<Redmine.Net.Api.Types.Project>(id);

        await Task.Delay(200);
        
        //Assert
        await Assert.ThrowsAsync<RedmineNotFoundException>(TestCode);
        return;

        async Task TestCode()
        {
            await fixture.RedmineManager.GetAsync<Redmine.Net.Api.Types.Project>(id);
        }
    }
}