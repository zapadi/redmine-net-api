using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure.Fixtures;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization.Xml;

[Collection(Constants.XmlRedmineSerializerCollection)]
public class ProjectTests(XmlSerializerFixture fixture)
{
    [Fact]
    public void Should_Deserialize_Project()
    {
        const string input = """ 
        <?xml version="1.0" encoding="UTF-8"?>
        <project>
            <id>1</id>
            <name>Redmine</name>
            <identifier>redmine</identifier>
            <description>
             Redmine is a flexible project management web application written using Ruby on Rails framework.
            </description>
            <homepage></homepage>
            <status>1</status>
            <parent id="123" name="foo"/>
            <default_version id="3" name="2.0"/>
            <default_assignee id="2" name="John Smith"/>
            <created_on>2007-09-29T12:03:04+02:00</created_on>
            <updated_on>2009-03-15T12:35:11+01:00</updated_on>
            <is_public>true</is_public>
        </project>
        """;
        
        var output = fixture.Serializer.Deserialize<Redmine.Net.Api.Types.Project>(input);
        
        Assert.NotNull(output);

        Assert.Equal(1, output.Id);
        Assert.Equal("Redmine", output.Name);
        Assert.Equal("redmine", output.Identifier);
        Assert.Contains("Redmine is a flexible project management web application", output.Description);
        Assert.Equal(new DateTime(2007, 9, 29, 10, 3, 4, DateTimeKind.Utc).ToLocalTime(), output.CreatedOn);
        Assert.Equal(new DateTime(2009, 3, 15, 11, 35, 11, DateTimeKind.Utc).ToLocalTime(), output.UpdatedOn);
        Assert.True(output.IsPublic);
    }
    
    [Fact]
    public void Should_Deserialize_Projects()
    {
        const string input = """ 
        <?xml version="1.0" encoding="UTF-8"?>
        <projects type="array">
            <project>
                <id>1</id>
                <name>Redmine</name>
                <identifier>redmine</identifier>
                <description>
                 Redmine is a flexible project management web application written using Ruby on Rails framework.
                </description>
                <created_on>2007-09-29T12:03:04+02:00</created_on>
                <updated_on>2009-03-15T12:35:11+01:00</updated_on>
                <is_public>true</is_public>
            </project>
            <project>
                <id>2</id>
            </project>
        </projects>
        """;
        
        var output = fixture.Serializer.DeserializeToPagedResults<Redmine.Net.Api.Types.Project>(input);
        
        Assert.NotNull(output);
        Assert.Equal(2, output.TotalItems);

        var projects = output.Items.ToList();
        Assert.Equal(1, projects[0].Id);
        Assert.Equal("Redmine", projects[0].Name);
        Assert.Equal("redmine", projects[0].Identifier);
        Assert.Contains("Redmine is a flexible project management web application", projects[0].Description);
        Assert.Equal(new DateTime(2007, 9, 29, 10, 3, 4, DateTimeKind.Utc).ToLocalTime(), projects[0].CreatedOn);
        Assert.Equal(new DateTime(2009, 3, 15, 11, 35, 11, DateTimeKind.Utc).ToLocalTime(), projects[0].UpdatedOn);
        Assert.True(projects[0].IsPublic);

        Assert.Equal(2, projects[1].Id);
    }
}
