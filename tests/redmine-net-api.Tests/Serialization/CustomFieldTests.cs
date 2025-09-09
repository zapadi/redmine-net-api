using System.Collections.Generic;
using System.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public sealed class CustomFieldTests()
{
    [Theory]
    [MemberData(nameof(CustomFieldsDeserializeTheoryData))]
    public void CustomFields_Should_Deserialize(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        
        var output = serializer.DeserializeToPagedResults<CustomField>(input);
        
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
    
   public static IEnumerable<TheoryDataRow<string, SerializerKind>> CustomFieldsDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "custom_fields": [
                                    {
                                      "id": 1,
                                      "name": "Affected version",
                                      "customized_type": "issue",
                                      "field_format": "list",
                                      "regexp": null,
                                      "min_length": null,
                                      "max_length": null,
                                      "is_required": true,
                                      "is_filter": true,
                                      "searchable": true,
                                      "multiple": true,
                                      "default_value": null,
                                      "visible": false,
                                      "possible_values": [
                                        { "value": "0.5.x" },
                                        { "value": "0.6.x" }
                                      ]
                                    }
                                  ]
                                }
                                """;

            const string xml = """
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
                                     <possible_value><value>0.5.x</value></possible_value>
                                     <possible_value><value>0.6.x</value></possible_value>
                                   </possible_values>
                                 </custom_field>
                               </custom_fields>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }

}