using System;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure.Fixtures;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization.Xml;

[Collection(Constants.XmlRedmineSerializerCollection)]
public class AttachmentTests(XmlSerializerFixture fixture)
{
    [Fact]
    public void Should_Deserialize_Attachment()
    {
        const string input = """ 
        <?xml version="1.0" encoding="UTF-8"?>
        <attachment>
            <id>6243</id>
            <filename>test.txt</filename>
            <filesize>124</filesize>
            <content_type>text/plain</content_type>
            <description>This is an attachment</description>
            <content_url>http://localhost:3000/attachments/download/6243/test.txt</content_url>
            <author name="Jean-Philippe Lang" id="1"/>
            <created_on>2011-07-18T22:58:40+02:00</created_on>
        </attachment>
        """;
        
        var output = fixture.Serializer.Deserialize<Redmine.Net.Api.Types.Attachment>(input);
        
        Assert.NotNull(output);
        Assert.Equal(6243, output.Id);
        Assert.Equal("test.txt", output.FileName);
        Assert.Equal(124, output.FileSize);
        Assert.Equal("text/plain", output.ContentType);
        Assert.Equal("This is an attachment", output.Description);
        Assert.Equal("http://localhost:3000/attachments/download/6243/test.txt", output.ContentUrl);
        Assert.Equal("Jean-Philippe Lang", output.Author.Name);
        Assert.Equal(1, output.Author.Id);
        Assert.Equal(new DateTime(2011, 7, 18, 20, 58, 40, DateTimeKind.Utc).ToLocalTime(), output.CreatedOn);

    }
}

