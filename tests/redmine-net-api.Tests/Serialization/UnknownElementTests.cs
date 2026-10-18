using System.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public class UnknownElementTests
{
    [Fact]
    public void Should_Skip_Unknown_Nested_Json_Elements_In_Issue()
    {
        const string input = """
                             {
                                 "issue": {
                                     "id": 1,
                                     "plugin_data": { "id": 999, "name": "x", "nested": { "id": 998 } },
                                     "plugin_list": [ { "id": 997 }, [ 1, 2 ] ],
                                     "subject": "subject",
                                     "project": { "id": 2, "name": "Project" },
                                     "status": { "id": 3, "name": "New" },
                                     "done_ratio": 50
                                 }
                             }
                             """;

        var output = SerializerFactory.Create(SerializerKind.NewtonsoftJson).Deserialize<Issue>(input);

        AssertIssue(output, 1);
        Assert.Equal(50, output.DoneRatio);
    }

    [Fact]
    public void Should_Skip_Unknown_Nested_Xml_Elements_In_Issue()
    {
        const string input = """
                             <?xml version="1.0" encoding="UTF-8"?>
                             <issue>
                                 <id>1</id>
                                 <plugin_data>
                                     <id>999</id>
                                     <subject>wrong</subject>
                                     <nested><id>998</id></nested>
                                 </plugin_data>
                                 <subject>subject</subject>
                                 <project id="2" name="Project"/>
                                 <status id="3" name="New"/>
                                 <done_ratio>50</done_ratio>
                             </issue>
                             """;

        var output = SerializerFactory.Create(SerializerKind.Xml).Deserialize<Issue>(input);

        AssertIssue(output, 1);
        Assert.Equal(50, output.DoneRatio);
    }

    [Fact]
    public void Should_Skip_Unknown_Nested_Json_Elements_In_Issue_List()
    {
        const string input = """
                             {
                                 "issues": [
                                     { "id": 1, "plugin_data": { "id": 999 }, "subject": "subject", "project": { "id": 2, "name": "Project" }, "status": { "id": 3, "name": "New" } },
                                     { "id": 2, "plugin_data": { "id": 999 }, "subject": "subject", "project": { "id": 2, "name": "Project" }, "status": { "id": 3, "name": "New" } }
                                 ],
                                 "total_count": 2,
                                 "offset": 0,
                                 "limit": 25
                             }
                             """;

        var output = SerializerFactory.Create(SerializerKind.NewtonsoftJson).DeserializeToPagedResults<Issue>(input);

        var issues = output.Items.ToList();
        Assert.Equal(2, issues.Count);
        AssertIssue(issues[0], 1);
        AssertIssue(issues[1], 2);
    }

    [Fact]
    public void Should_Skip_Unknown_Nested_Xml_Elements_In_Issue_List()
    {
        const string input = """
                             <?xml version="1.0" encoding="UTF-8"?>
                             <issues total_count="2" offset="0" limit="25" type="array">
                                 <issue>
                                     <id>1</id>
                                     <plugin_data><id>999</id><subject>wrong</subject></plugin_data>
                                     <subject>subject</subject>
                                     <project id="2" name="Project"/>
                                     <status id="3" name="New"/>
                                 </issue>
                                 <issue>
                                     <id>2</id>
                                     <plugin_data><id>999</id><subject>wrong</subject></plugin_data>
                                     <subject>subject</subject>
                                     <project id="2" name="Project"/>
                                     <status id="3" name="New"/>
                                 </issue>
                             </issues>
                             """;

        var output = SerializerFactory.Create(SerializerKind.Xml).DeserializeToPagedResults<Issue>(input);

        var issues = output.Items.ToList();
        Assert.Equal(2, issues.Count);
        AssertIssue(issues[0], 1);
        AssertIssue(issues[1], 2);
    }

    [Fact]
    public void Should_Skip_Unknown_Nested_Json_Elements_In_Project()
    {
        const string input = """
                             {
                                 "project": {
                                     "id": 1,
                                     "plugin_data": { "id": 999, "identifier": "wrong" },
                                     "plugin_list": [ { "id": 997 } ],
                                     "name": "Project",
                                     "identifier": "project",
                                     "parent": { "id": 5, "name": "Parent" },
                                     "is_public": true
                                 }
                             }
                             """;

        var output = SerializerFactory.Create(SerializerKind.NewtonsoftJson).Deserialize<Project>(input);

        AssertProject(output);
    }

    [Fact]
    public void Should_Skip_Unknown_Nested_Xml_Elements_In_Project()
    {
        const string input = """
                             <?xml version="1.0" encoding="UTF-8"?>
                             <project>
                                 <id>1</id>
                                 <plugin_data><id>999</id><identifier>wrong</identifier></plugin_data>
                                 <name>Project</name>
                                 <identifier>project</identifier>
                                 <parent id="5" name="Parent"/>
                                 <is_public>true</is_public>
                             </project>
                             """;

        var output = SerializerFactory.Create(SerializerKind.Xml).Deserialize<Project>(input);

        AssertProject(output);
    }

    [Fact]
    public void Should_Skip_Unknown_Nested_Json_Elements_In_Issue_Custom_Field()
    {
        const string input = """
                             {
                                 "issue": {
                                     "id": 1,
                                     "custom_fields": [
                                         { "id": 2, "plugin_data": { "id": 999, "value": "wrong" }, "name": "Field", "value": "a" },
                                         { "id": 3, "name": "Other", "value": "b" }
                                     ],
                                     "subject": "subject"
                                 }
                             }
                             """;

        var output = SerializerFactory.Create(SerializerKind.NewtonsoftJson).Deserialize<Issue>(input);

        Assert.Equal(1, output.Id);
        Assert.Equal("subject", output.Subject);
        Assert.Equal(2, output.CustomFields.Count);
        Assert.Equal(2, output.CustomFields[0].Id);
        Assert.Equal("Field", output.CustomFields[0].Name);
        Assert.Equal("a", output.CustomFields[0].Values[0].Info);
        Assert.Equal(3, output.CustomFields[1].Id);
        Assert.Equal("b", output.CustomFields[1].Values[0].Info);
    }

    private static void AssertIssue(Issue issue, int id)
    {
        Assert.Equal(id, issue.Id);
        Assert.Equal("subject", issue.Subject);
        Assert.Equal(2, issue.Project.Id);
        Assert.Equal("Project", issue.Project.Name);
        Assert.Equal(3, issue.Status.Id);
        Assert.Equal("New", issue.Status.Name);
    }

    private static void AssertProject(Project project)
    {
        Assert.Equal(1, project.Id);
        Assert.Equal("Project", project.Name);
        Assert.Equal("project", project.Identifier);
        Assert.Equal(5, project.Parent.Id);
        Assert.True(project.IsPublic);
    }
}
