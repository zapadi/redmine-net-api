using System.Collections.Generic;
using System.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public class ApiCoverageFieldTests
{
    private static List<T> List<T>(string input, SerializerKind kind) where T : class, new()
    {
        return SerializerFactory.Create(kind).DeserializeToPagedResults<T>(input).Items.ToList();
    }

    [Theory]
    [InlineData("""{"issue_statuses":[{"id":1,"name":"New","is_closed":false,"description":"Just created"},{"id":2,"name":"Closed","is_closed":true}]}""", SerializerKind.NewtonsoftJson)]
    [InlineData("""<issue_statuses type="array"><issue_status><id>1</id><name>New</name><is_closed>false</is_closed><description>Just created</description></issue_status><issue_status><id>2</id><name>Closed</name><is_closed>true</is_closed><description/></issue_status></issue_statuses>""", SerializerKind.Xml)]
    public void IssueStatus_Should_Read_Description(string input, SerializerKind kind)
    {
        var statuses = List<IssueStatus>(input, kind);

        Assert.Equal("Just created", statuses[0].Description);
        Assert.False(statuses[0].IsClosed);
        Assert.True(string.IsNullOrEmpty(statuses[1].Description));
        Assert.True(statuses[1].IsClosed);
    }

    [Theory]
    [InlineData("""
                {"custom_fields":[{"id":1,"name":"Due","customized_type":"issue","field_format":"date","is_for_all":true,"default_value_mode":"current_date","visible":true,"editable":false,
                 "projects":[{"id":1,"name":"P1"},{"id":2,"name":"P2"}],"trackers":[{"id":3,"name":"Bug"}]}]}
                """, SerializerKind.NewtonsoftJson)]
    [InlineData("""
                <custom_fields type="array"><custom_field><id>1</id><name>Due</name><customized_type>issue</customized_type><field_format>date</field_format><is_for_all>true</is_for_all><default_value_mode>current_date</default_value_mode><visible>true</visible><editable>false</editable>
                <projects type="array"><project id="1" name="P1"/><project id="2" name="P2"/></projects><trackers type="array"><tracker id="3" name="Bug"/></trackers></custom_field></custom_fields>
                """, SerializerKind.Xml)]
    public void CustomField_Should_Read_New_Fields(string input, SerializerKind kind)
    {
        var field = Assert.Single(List<CustomField>(input, kind));

        Assert.True(field.IsForAll);
        Assert.False(field.Editable);
        Assert.True(field.Visible);
        Assert.Equal("current_date", field.DefaultValueMode);
        Assert.Equal([1, 2], field.Projects.Select(p => p.Id));
        Assert.Equal(["P1", "P2"], field.Projects.Select(p => p.Name));
        Assert.Equal(3, Assert.Single(field.Trackers).Id);
    }

    [Theory]
    [InlineData("""{"custom_fields":[{"id":1,"name":"F","projects":[],"editable":true}]}""", SerializerKind.NewtonsoftJson)]
    [InlineData("""<custom_fields type="array"><custom_field><id>1</id><name>F</name><projects type="array"/><editable>true</editable></custom_field></custom_fields>""", SerializerKind.Xml)]
    public void CustomField_Should_Read_Editable(string input, SerializerKind kind)
    {
        var field = Assert.Single(List<CustomField>(input, kind));

        Assert.True(field.Editable);
        Assert.Null(field.Projects);
    }

    [Theory]
    [InlineData("""{"group":{"id":12,"name":"Anonymous users","builtin":"anonymous","custom_fields":[]}}""", SerializerKind.NewtonsoftJson)]
    [InlineData("""<group><id>12</id><name>Anonymous users</name><builtin>anonymous</builtin><custom_fields type="array"/></group>""", SerializerKind.Xml)]
    public void Group_Should_Read_Builtin(string input, SerializerKind kind)
    {
        var group = SerializerFactory.Create(kind).Deserialize<Group>(input);

        Assert.Equal("anonymous", group.Builtin);
        Assert.Equal("Anonymous users", group.Name);
    }

    [Theory]
    [InlineData("""{"wiki_page":{"title":"Start","text":"x","version":2,"author":{"id":1,"name":"Admin"},"comments":"","project":{"id":3,"name":"Proj"},"created_on":"2024-01-01T00:00:00Z"}}""", SerializerKind.NewtonsoftJson)]
    [InlineData("""<wiki_page><title>Start</title><text>x</text><version>2</version><author id="1" name="Admin"/><comments/><project id="3" name="Proj"/><created_on>2024-01-01T00:00:00Z</created_on></wiki_page>""", SerializerKind.Xml)]
    public void WikiPage_Should_Read_Project(string input, SerializerKind kind)
    {
        var page = SerializerFactory.Create(kind).Deserialize<WikiPage>(input);

        Assert.Equal(3, page.Project.Id);
        Assert.Equal("Proj", page.Project.Name);
        Assert.Equal(1, page.Author.Id);
        Assert.NotNull(page.CreatedOn);
    }

    private const string EnumerationJson = """{"ROOT":[{"id":9,"name":"Design","is_default":false,"active":true,"custom_fields":[{"id":2,"name":"Billable","value":"1"}]}]}""";
    private const string EnumerationXml = """<ROOT type="array"><ITEM><id>9</id><name>Design</name><is_default>false</is_default><active>true</active><custom_fields type="array"><custom_field id="2" name="Billable"><value>1</value></custom_field></custom_fields></ITEM></ROOT>""";

    private static string Enumeration(SerializerKind kind, string root, string item)
    {
        return kind == SerializerKind.Xml
            ? EnumerationXml.Replace("ROOT", root).Replace("ITEM", item)
            : EnumerationJson.Replace("ROOT", root);
    }

    private static void AssertCustomField(IList<IssueCustomField> customFields)
    {
        var field = Assert.Single(customFields);
        Assert.Equal(2, field.Id);
        Assert.Equal("Billable", field.Name);
        Assert.Equal("1", Assert.Single(field.Values).Info);
    }

    [Theory]
    [InlineData(SerializerKind.NewtonsoftJson)]
    [InlineData(SerializerKind.Xml)]
    public void TimeEntryActivity_Should_Read_CustomFields(SerializerKind kind)
    {
        var activity = Assert.Single(List<TimeEntryActivity>(Enumeration(kind, "time_entry_activities", "time_entry_activity"), kind));

        Assert.True(activity.IsActive);
        AssertCustomField(activity.CustomFields);
    }

    [Theory]
    [InlineData(SerializerKind.NewtonsoftJson)]
    [InlineData(SerializerKind.Xml)]
    public void IssuePriority_Should_Read_CustomFields(SerializerKind kind)
    {
        var priority = Assert.Single(List<IssuePriority>(Enumeration(kind, "issue_priorities", "issue_priority"), kind));

        Assert.True(priority.IsActive);
        AssertCustomField(priority.CustomFields);
    }

    [Theory]
    [InlineData(SerializerKind.NewtonsoftJson)]
    [InlineData(SerializerKind.Xml)]
    public void DocumentCategory_Should_Read_CustomFields(SerializerKind kind)
    {
        var category = Assert.Single(List<DocumentCategory>(Enumeration(kind, "document_categories", "document_category"), kind));

        Assert.True(category.IsActive);
        AssertCustomField(category.CustomFields);
    }

    [Fact]
    public void New_Fields_Should_Take_Part_In_Equality()
    {
        Assert.NotEqual(new IssueStatus { Id = 1, Description = "a" }, new IssueStatus { Id = 1, Description = "b" });
        Assert.NotEqual(new CustomField { Id = 1, Editable = true }, new CustomField { Id = 1 });
        Assert.NotEqual(new CustomField { Id = 1, IsForAll = true }, new CustomField { Id = 1 });
        Assert.NotEqual(new CustomField { Id = 1, DefaultValueMode = "current_date" }, new CustomField { Id = 1 });
        Assert.NotEqual(new CustomField { Id = 1, Projects = [new IdentifiableName(1, "a")] }, new CustomField { Id = 1, Projects = [new IdentifiableName(2, "a")] });
        Assert.NotEqual(new Group { Id = 1, Builtin = "anonymous" }, new Group { Id = 1 });
        Assert.NotEqual(new WikiPage { Id = 1, Project = new IdentifiableName(1, "a") }, new WikiPage { Id = 1 });
        Assert.NotEqual(new TimeEntryActivity { Id = 1, CustomFields = [IssueCustomField.CreateSingle(1, "f", "a")] }, new TimeEntryActivity { Id = 1 });
        Assert.NotEqual(new IssuePriority { Id = 1, CustomFields = [IssueCustomField.CreateSingle(1, "f", "a")] }, new IssuePriority { Id = 1 });
        Assert.NotEqual(new DocumentCategory { Id = 1, CustomFields = [IssueCustomField.CreateSingle(1, "f", "a")] }, new DocumentCategory { Id = 1 });

        var field = new CustomField { Id = 1, Editable = true, IsForAll = true, DefaultValueMode = "m", Projects = [new IdentifiableName(1, "a")] };
        var same = new CustomField { Id = 1, Editable = true, IsForAll = true, DefaultValueMode = "m", Projects = [new IdentifiableName(1, "a")] };
        Assert.Equal(field, same);
        Assert.Equal(field.GetHashCode(), same.GetHashCode());
    }

    [Fact]
    public void IssueStatus_Clone_Should_Copy_Description()
    {
        var status = new IssueStatus { Id = 1, Name = "New", Description = "d" };

        var clone = status.Clone(false);

        Assert.Equal("d", clone.Description);
        Assert.Equal(status, clone);
    }
}
