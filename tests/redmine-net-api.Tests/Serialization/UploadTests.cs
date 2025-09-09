using System.Collections.Generic;
using System.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public class UploadTests()
{
    [Theory]
    [MemberData(nameof(UploadDeserializeTheoryData))]
    public void Should_Deserialize_Upload(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        var output = serializer.Deserialize<Redmine.Net.Api.Types.Upload>(input);
        
        Assert.NotNull(output);
        Assert.Equal("#{token1}", output.Token);
        Assert.Equal("test1.txt", output.FileName);
    }
    
    [Theory]
    [MemberData(nameof(UploadsDeserializeTheoryData))]
    public void Should_Deserialize_Uploads(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
       
        var output = serializer.DeserializeToPagedResults<Redmine.Net.Api.Types.Upload>(input);
        
        Assert.NotNull(output);
        Assert.Equal(2, output.TotalItems);

        var uploads = output.Items.ToList();
        Assert.Equal(2, uploads.Count);

        Assert.Equal("#{token1}", uploads[0].Token);
        Assert.Equal("test1.txt", uploads[0].FileName);

        Assert.Equal("#{token2}", uploads[1].Token);
        Assert.Equal("test1.txt", uploads[1].FileName);

    }
    
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> UploadDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "upload": {
                                    "token": "#{token1}",
                                    "filename": "test1.txt"
                                  }
                                }
                                """;

            const string xml = """
                               <?xml version="1.0" encoding="UTF-8" ?>
                               <upload>
                                <token>#{token1}</token>
                                <filename>test1.txt</filename>
                               </upload>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    } 
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> UploadsDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "uploads": [
                                    {
                                      "token": "#{token1}",
                                      "filename": "test1.txt"
                                    },
                                    {
                                      "token": "#{token2}",
                                      "filename": "test1.txt"
                                    }
                                  ]
                                }
                                """;

            const string xml = """
                               <?xml version="1.0" encoding="UTF-8" ?>
                               <uploads type="array">
                                   <upload>
                                       <token>#{token1}</token>
                                       <filename>test1.txt</filename>
                                   </upload>
                                   <upload>
                                       <token>#{token2}</token>
                                       <filename>test1.txt</filename>
                                   </upload>
                               </uploads>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
}

