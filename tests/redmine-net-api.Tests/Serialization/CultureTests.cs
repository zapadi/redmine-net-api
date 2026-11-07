using System;
using System.Globalization;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public class CultureTests
{
    [Theory]
    [InlineData(SerializerKind.Xml)]
    [InlineData(SerializerKind.NewtonsoftJson)]
    public void Should_Serialize_Done_Ratio_Invariant_Under_Non_Invariant_Culture(SerializerKind kind)
    {
        var issue = new Issue { Subject = "subject", DoneRatio = 12.5f };

        var output = RunWithCulture("de-DE", () => SerializerFactory.Create(kind).Serialize(issue));

        Assert.Contains("12.5", output);
        Assert.DoesNotContain("12,5", output);
    }

    [Fact]
    public void Should_Deserialize_Xml_Date_Invariant_Under_Non_Gregorian_Culture()
    {
        const string input = """
                             <?xml version="1.0" encoding="UTF-8"?>
                             <issue>
                                 <id>1</id>
                                 <created_on>2024-06-15T10:00:00Z</created_on>
                             </issue>
                             """;

        var output = RunWithCulture("th-TH", () => SerializerFactory.Create(SerializerKind.Xml).Deserialize<Issue>(input));

        Assert.NotNull(output.CreatedOn);
        Assert.Equal(new DateTime(2024, 6, 15, 10, 0, 0, DateTimeKind.Utc).ToLocalTime(), output.CreatedOn.Value);
    }

    private static T RunWithCulture<T>(string cultureName, Func<T> action)
    {
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo(cultureName);
            return action();
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}
