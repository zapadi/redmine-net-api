using System.Collections.Generic;
using System.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public sealed class ErrorTests()
{
    [Theory]
    [MemberData(nameof(ErrorsDeserializeTheoryData))]
    public void Should_Deserialize_Errors(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        
        var output = serializer.DeserializeToPagedResults<Error>(input);
        
        Assert.NotNull(output);
        Assert.Equal(2, output.TotalItems);

        var errors = output.Items.ToList();
        Assert.Equal("First name can't be blank", errors[0].Info);
        Assert.Equal("Email is invalid", errors[1].Info);

    }
    
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> ErrorsDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "errors": [
                                    "First name can't be blank",
                                    "Email is invalid"
                                  ]
                                }
                                """;

            const string xml = """
                               <errors type="array">
                                   <error>First name can't be blank</error>
                                   <error>Email is invalid</error>
                               </errors>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
}