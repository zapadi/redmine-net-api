using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure.Fixtures;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization.Json;

[Collection(Constants.JsonRedmineSerializerCollection)]
public class IssuesTests(JsonSerializerFixture fixture)
{
    [Fact]
    public void Should_Deserialize_Issue_With_Watchers()
    {
        const string input = """
         {
           "issue": {
             "id": 5,
             "project": {
               "id": 1,
               "name": "Project-Test"
             },
             "tracker": {
               "id": 1,
               "name": "Bug"
             },
             "status": {
               "id": 1,
               "name": "New",
               "is_closed": true
             },
             "priority": {
               "id": 2,
               "name": "Normal"
             },
             "author": {
               "id": 90,
               "name": "Admin User"
             },
             "fixed_version": {
               "id": 2,
               "name": "version2"
             },
             "subject": "#380",
             "description": "",
             "start_date": "2025-04-28",
             "due_date": null,
             "done_ratio": 0,
             "is_private": false,
             "estimated_hours": null,
             "total_estimated_hours": null,
             "spent_hours": 0.0,
             "total_spent_hours": 0.0,
             "created_on": "2025-04-28T17:58:42Z",
             "updated_on": "2025-04-28T17:58:42Z",
             "closed_on": null,
             "watchers": [
               {
                 "id": 91,
                 "name": "Normal User"
               },
               {
                 "id": 90,
                 "name": "Admin User"
               }
             ]
           }
         }
         """;
                             
        var output = fixture.Serializer.Deserialize<Redmine.Net.Api.Types.Issue>(input);
        
        Assert.NotNull(output);
        Assert.Equal(5, output.Id);
        Assert.Equal("Project-Test", output.Project.Name);
        Assert.Equal(1, output.Project.Id);
        Assert.Equal("Bug", output.Tracker.Name);
        Assert.Equal(1, output.Tracker.Id);
        Assert.Equal("New", output.Status.Name);
        Assert.Equal(1, output.Status.Id);
        Assert.True(output.Status.IsClosed);
        Assert.False(output.Status.IsDefault);
        Assert.Equal(2, output.FixedVersion.Id);
        Assert.Equal("version2", output.FixedVersion.Name);
        Assert.Equal(new DateTime(2025, 4, 28), output.StartDate);
        Assert.Null(output.DueDate);
        Assert.Equal(0, output.DoneRatio);
        Assert.Null(output.EstimatedHours);
        Assert.Null(output.TotalEstimatedHours);

        var watchers = output.Watchers.ToList();
        Assert.Equal(2, watchers.Count);
        
        Assert.Equal(91, watchers[0].Id);
        Assert.Equal("Normal User", watchers[0].Name);
        Assert.Equal(90, watchers[1].Id);
        Assert.Equal("Admin User", watchers[1].Name);
    }
}