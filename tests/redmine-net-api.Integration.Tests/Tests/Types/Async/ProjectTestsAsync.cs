using System.Text;
using Newtonsoft.Json.Linq;
using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Helpers;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Types;
using Xunit;

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

    // Project custom field values are read straight from the API response,
    // because Project.CustomFieldValues does not deserialize the values.
    private async Task<JToken?> GetProjectCustomFieldValueAsync(int projectId, int customFieldId)
    {
        var content = await fixture.RedmineManager.DownloadFileAsync($"{fixture.RedmineHost}/projects/{projectId}.json", cancellationToken: TestContext.Current.CancellationToken);
        var customFields = JObject.Parse(Encoding.UTF8.GetString(content))["project"]?["custom_fields"];
        return customFields?.FirstOrDefault(cf => (int?)cf["id"] == customFieldId)?["value"];
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
            // Project custom fields 3 (string) and 4 (multi-value list) are seeded in init-redmine.sql.
            CustomFieldValues = [
                new CustomField()
                {
                    Id = 3,
                    PossibleValues = [new CustomFieldPossibleValue(value: "VALUE")]
                },
                new CustomField
                {
                    Id = 4,
                    PossibleValues = [new CustomFieldPossibleValue(value: "a"), new CustomFieldPossibleValue(value: "b")]
                },
            ]
        };

        //Act
        var createdProject = await fixture.RedmineManager.CreateAsync(projectPayload, cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(createdProject);

        var project = await fixture.RedmineManager.GetAsync<Project>(createdProject, RequestOptions.Include(
                RedmineKeys.TRACKERS,
                RedmineKeys.ENABLED_MODULES,
                RedmineKeys.ISSUE_CATEGORIES,
                RedmineKeys.TIME_ENTRY_ACTIVITIES,
                RedmineKeys.ISSUE_CUSTOM_FIELDS)
, TestContext.Current.CancellationToken);

        Assert.NotNull(project);
        Assert.Equal(projectName, project.Name);
        Assert.Equal(projectPayload.Name, project.Name);
        Assert.Equal(projectPayload.Identifier, project.Identifier);
        Assert.Equal(projectPayload.Description, project.Description);
        Assert.Equal(projectPayload.HomePage, project.HomePage);
        Assert.Equal(projectPayload.IsPublic, project.IsPublic);
        Assert.Equal(projectPayload.InheritMembers, project.InheritMembers);
        
        Assert.Equal(9, project.EnabledModules.Count);
        Assert.All(project.EnabledModules, module => Assert.True(module.Id > 0));
        
        Assert.Equal(3, project.Trackers.Count);
        Assert.All(project.Trackers, tracker => Assert.True(tracker.Id > 0));
        
       // Assert.Equal(2, project.IssueCustomFields.Count);
        Assert.All(project.IssueCustomFields, icf => Assert.True(icf.Id > 0));

        Assert.Equal("VALUE", (string?)await GetProjectCustomFieldValueAsync(project.Id, 3));
        Assert.Equal(["a", "b"], Assert.IsType<JArray>(await GetProjectCustomFieldValueAsync(project.Id, 4)).Values<string>());
    }
    
      [Fact]
    public async Task UpdateProject_Should_Succeed()
    {
        var projectName = RandomHelper.GenerateText(7);
        var projectPayload = new Project
        {
            Name = projectName,
            Identifier = projectName.ToLowerInvariant(),
            Description = RandomHelper.GenerateText(7),
            HomePage = RandomHelper.GenerateText(7),
            IsPublic = true,
            InheritMembers = false,
            Trackers =
            [
                new ProjectTracker(1),
            ],
            IssueCustomFields =
            [
                // Issue custom field should be already defined.
                // An existing icf is associated with the new project
                 IssueCustomField.CreateSingle(2, RandomHelper.GenerateText(5), RandomHelper.GenerateText(7))
            ],
        };

        //Act
        var createdProject = await fixture.RedmineManager.CreateAsync(projectPayload, cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(createdProject);

        createdProject.Description = "Changed description";
        // Project custom fields 3 (string) and 4 (multi-value list) are seeded in init-redmine.sql.
        createdProject.CustomFieldValues =
        [
            new CustomField()
            {
                Id = 3,
                PossibleValues = [new CustomFieldPossibleValue(value: "OTHER_VALUE")]
            },
            new CustomField
            {
                Id = 4,
                PossibleValues = [new CustomFieldPossibleValue(value: "b"), new CustomFieldPossibleValue(value: "c")]
            },
        ];
        
        await fixture.RedmineManager.UpdateAsync(createdProject.Identifier, createdProject, cancellationToken: TestContext.Current.CancellationToken);

        var project = await fixture.RedmineManager.GetAsync<Project>(createdProject, RequestOptions.Include(
                RedmineKeys.TRACKERS,
                RedmineKeys.ENABLED_MODULES,
                RedmineKeys.ISSUE_CATEGORIES,
                RedmineKeys.TIME_ENTRY_ACTIVITIES,
                RedmineKeys.ISSUE_CUSTOM_FIELDS)
, TestContext.Current.CancellationToken);

        Assert.NotNull(project);
        Assert.Equal(projectName, project.Name);
        Assert.Equal(projectPayload.Name, project.Name);
        Assert.Equal(projectPayload.Identifier, project.Identifier);
        Assert.Equal(createdProject.Description, project.Description);
        Assert.Equal(projectPayload.HomePage, project.HomePage);
        Assert.Equal(projectPayload.IsPublic, project.IsPublic);
        Assert.Equal(projectPayload.InheritMembers, project.InheritMembers);
        
        Assert.NotNull(project.EnabledModules);
        Assert.NotNull(project.Trackers);

        Assert.NotEmpty(project.TimeEntryActivities);
        
        Assert.NotEmpty(project.IssueCustomFields);
        Assert.Single(project.IssueCustomFields);
        Assert.All(project.IssueCustomFields, icf => Assert.True(icf.Id > 0));

        Assert.Equal("OTHER_VALUE", (string?)await GetProjectCustomFieldValueAsync(project.Id, 3));
        Assert.Equal(["b", "c"], Assert.IsType<JArray>(await GetProjectCustomFieldValueAsync(project.Id, 4)).Values<string>());
    }

    [Fact]
    public async Task DeleteIssue_Should_Succeed()
    {
        //Arrange
        var createdEntity = await CreateEntityAsync("DeleteTest");
        Assert.NotNull(createdEntity);

        var id = createdEntity.Id.ToInvariantString();

        //Act
        await fixture.RedmineManager.DeleteAsync<Project>(id, cancellationToken: TestContext.Current.CancellationToken);

        await Task.Delay(200, TestContext.Current.CancellationToken);

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
