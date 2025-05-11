using System.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure.Fixtures;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization.Json;

[Collection(Constants.JsonRedmineSerializerCollection)]
public sealed class CustomFieldTests(JsonSerializerFixture fixture)
{
    [Fact]
    public void Should_Deserialize_CustomFields()
    {
        const string input = """
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
                {
                  "value": "0.5.x"
                },
                {
                  "value": "0.6.x"
                }
              ]
            }
          ],
          "total_count": 1
        }
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