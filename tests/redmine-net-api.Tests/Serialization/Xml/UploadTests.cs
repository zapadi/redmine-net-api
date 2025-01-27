using System.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure.Fixtures;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization.Xml;

[Collection(Constants.XmlRedmineSerializerCollection)]
public class UploadTests(XmlSerializerFixture fixture)
{
    [Fact]
    public void Should_Deserialize_Upload()
    {
        const string input = """ 
        <?xml version="1.0" encoding="UTF-8" ?>
        <upload>
         <token>#{token1}</token>
         <filename>test1.txt</filename>
        </upload>
        """;
        
        var output = fixture.Serializer.Deserialize<Redmine.Net.Api.Types.Upload>(input);
        
        Assert.NotNull(output);
        Assert.Equal("#{token1}", output.Token);
        Assert.Equal("test1.txt", output.FileName);
    }
    
    [Fact]
    public void Should_Deserialize_Uploads()
    {
        const string input = """ 
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
        
        var output = fixture.Serializer.DeserializeToPagedResults<Redmine.Net.Api.Types.Upload>(input);
        
        Assert.NotNull(output);
        Assert.Equal(2, output.TotalItems);

        var uploads = output.Items.ToList();
        Assert.Equal(2, uploads.Count);

        Assert.Equal("#{token1}", uploads[0].Token);
        Assert.Equal("test1.txt", uploads[0].FileName);

        Assert.Equal("#{token2}", uploads[1].Token);
        Assert.Equal("test1.txt", uploads[1].FileName);

    }
}

