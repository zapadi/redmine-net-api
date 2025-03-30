using System.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure.Fixtures;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization.Xml;

[Collection(Constants.XmlRedmineSerializerCollection)]
public sealed class CustomFieldTests(XmlSerializerFixture fixture)
{
    [Fact]
    public void Should_Deserialize_CustomFields()
    {
        const string input = """
        <?xml version="1.0" encoding="UTF-8"?>
        <custom_fields type="array">
          <custom_field>
            <id>1</id>
            <name>Affected version</name>
            <customized_type>issue</customized_type>
            <field_format>list</field_format>
            <regexp/>
            <min_length/>
            <max_length/>
            <is_required>true</is_required>
            <is_filter>true</is_filter>
            <searchable>true</searchable>
            <multiple>true</multiple>
            <default_value/>
            <visible>false</visible>
            <possible_values type="array">
              <possible_value>
                <value>0.5.x</value>
              </possible_value>
              <possible_value>
                <value>0.6.x</value>
              </possible_value>
            </possible_values>  
          </custom_field>
        </custom_fields>
        """;
        
        var output = fixture.Serializer.DeserializeToPagedResults<CustomField>(input);
        
        Assert.NotNull(output);
        Assert.Equal(1, output.TotalItems);

        var customFields = output.Items.ToList();
        Assert.Equal(1, customFields[0].Id);
        Assert.Equal("Affected version", customFields[0].Name);
        Assert.Equal("issue", customFields[0].CustomizedType);
        Assert.Equal("list", customFields[0].FieldFormat);
        Assert.True(customFields[0].IsRequired);
        Assert.True(customFields[0].IsFilter);
        Assert.True(customFields[0].Searchable);
        Assert.True(customFields[0].Multiple);
        Assert.False(customFields[0].Visible);

        var possibleValues = customFields[0].PossibleValues.ToList();
        Assert.Equal(2, possibleValues.Count);
        Assert.Equal("0.5.x", possibleValues[0].Value);
        Assert.Equal("0.6.x", possibleValues[1].Value);
    }
}