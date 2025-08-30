using Padi.RedmineApi;
using Padi.RedmineApi.Exceptions;
using Padi.RedmineApi.Extensions;
using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Helpers;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Padi.RedmineApi.Internals;
using Padi.RedmineApi.Net;
using Padi.RedmineApi.Types;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class ProjectTestsAsync(RedmineTestContainerFixture fixture)
{
    private async Task<Project> CreateEntityAsync(string? suffix = null)
    {
        var entity = new Project
        {
            Identifier = RandomHelper.GenerateText(5, true),
            Name = $"{RandomHelper.GenerateText(5)}{suffix}",
        };

        return await fixture.RedmineManager.CreateAsync(entity);
    }

    [Fact]
    public async Task CreateProject_Should_Succeed()
    {
        var projectName = RandomHelper.GenerateText(7);
        var projectPayload = new Project
        {
            Name = projectName,
            Identifier = projectName.ToLowerInvariant(),
            Description = RandomHelper.GenerateText(7),
            HomePage = RandomHelper.GenerateText(7),
            IsPublic = true,
            InheritMembers = true,

            EnabledModules = [
                new ProjectEnabledModule(RedmineKeys.FILES),
                new ProjectEnabledModule(RedmineKeys.WIKI),
                new ProjectEnabledModule("boards"),
                new ProjectEnabledModule("calendar"),
                new ProjectEnabledModule(RedmineKeys.DOCUMENTS),
                new ProjectEnabledModule("issue_tracking"),
                new ProjectEnabledModule(RedmineKeys.NEWS),
                new ProjectEnabledModule(RedmineKeys.REPOSITORY),
                new ProjectEnabledModule("time_tracking")
            ],

            Trackers =
            [
                new ProjectTracker(1),
                new ProjectTracker(2),
                new ProjectTracker(3),
            ],
            IssueCustomFields =
            [
                // Issue custom field should be already defined.
                // An existing icf is associated with the new project
                 IssueCustomField.CreateSingle(1, RandomHelper.GenerateText(5), RandomHelper.GenerateText(7))
            ],
            CustomFieldValues = [
                new IdentifiableName(1, "VALUE"),
                new IdentifiableName(2, "OTHER_VALUE")
            ]
        };

        //Act
        var createdProject = await fixture.RedmineManager.CreateAsync(projectPayload);
        Assert.NotNull(createdProject);

        var project = await fixture.RedmineManager.GetAsync<Project>(createdProject,
            RequestOptions.Include(
                RedmineKeys.TRACKERS,
                RedmineKeys.ENABLED_MODULES,
                RedmineKeys.ISSUE_CATEGORIES,
                RedmineKeys.TIME_ENTRY_ACTIVITIES,
                RedmineKeys.ISSUE_CUSTOM_FIELDS)
            );

        Assert.NotNull(project);
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
        var ex = await Assert.ThrowsAsync<RedmineApiException>(TestCode);
        Assert.NotNull(ex);
        Assert.Equal(HttpConstants.StatusCodes.NotFound, ex.HttpStatusCode);
        return;

        async Task TestCode()
        {
            await fixture.RedmineManager.GetAsync<Project>(id);
        }
    }
}
