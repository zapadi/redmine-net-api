using System.Collections.Generic;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public class AttachmentTests
{
    [Fact]
    public void Attachments_Should_Serialize_Json_For_Issue_Attachment_Update()
    {
        var attachments = new Redmine.Net.Api.Types.Attachments
        {
            { 6243, new Redmine.Net.Api.Types.Attachment { FileName = "test.txt", Description = "updated" } },
            { 6244, new Redmine.Net.Api.Types.Attachment { FileName = "other.txt" } }
        };

        var output = SerializerFactory.Create(SerializerKind.NewtonsoftJson).Serialize(attachments);

        var expected = JObject.Parse("""
                                     {
                                       "attachments": {
                                         "6243": { "filename": "test.txt", "description": "updated" },
                                         "6244": { "filename": "other.txt" }
                                       }
                                     }
                                     """);
        Assert.True(JToken.DeepEquals(expected, JObject.Parse(output)), output);
    }

    [Fact]
    public void Attachments_Should_Serialize_Xml_For_Issue_Attachment_Update()
    {
        var attachments = new Redmine.Net.Api.Types.Attachments
        {
            { 6243, new Redmine.Net.Api.Types.Attachment { FileName = "test.txt", Description = "updated" } },
            { 6244, new Redmine.Net.Api.Types.Attachment { FileName = "other.txt" } }
        };

        var output = SerializerFactory.Create(SerializerKind.Xml).Serialize(attachments);

        var expected = XElement.Parse("""<attachments><attachment id="6243"><filename>test.txt</filename><description>updated</description></attachment><attachment id="6244"><filename>other.txt</filename></attachment></attachments>""");
        Assert.True(XNode.DeepEquals(expected, XElement.Parse(output)), output);
    }

    [Fact]
    public void Attachment_Should_Serialize_Json_For_Update()
    {
        var attachment = new Redmine.Net.Api.Types.Attachment { FileName = "test.txt", Description = "updated" };

        var output = SerializerFactory.Create(SerializerKind.NewtonsoftJson).Serialize(attachment);

        var expected = JObject.Parse("""{ "attachment": { "filename": "test.txt", "description": "updated" } }""");
        Assert.True(JToken.DeepEquals(expected, JObject.Parse(output)), output);
    }

    [Fact]
    public void Attachment_Should_Serialize_Xml_For_Update()
    {
        var attachment = new Redmine.Net.Api.Types.Attachment { FileName = "test.txt", Description = "updated" };

        var output = SerializerFactory.Create(SerializerKind.Xml).Serialize(attachment);

        var expected = XElement.Parse("<attachment><description>updated</description><filename>test.txt</filename></attachment>");
        Assert.True(XNode.DeepEquals(expected, XElement.Parse(output)), output);
    }

    [Fact]
    public void Attachment_Should_Omit_Null_Fields_From_Json()
    {
        var attachment = new Redmine.Net.Api.Types.Attachment { FileName = "test.txt" };

        var output = SerializerFactory.Create(SerializerKind.NewtonsoftJson).Serialize(attachment);

        var expected = JObject.Parse("""{ "attachment": { "filename": "test.txt" } }""");
        Assert.True(JToken.DeepEquals(expected, JObject.Parse(output)), output);
    }

    [Fact]
    public void Attachment_Should_Omit_Null_Fields_From_Xml()
    {
        var attachment = new Redmine.Net.Api.Types.Attachment { FileName = "test.txt" };

        var output = SerializerFactory.Create(SerializerKind.Xml).Serialize(attachment);

        var expected = XElement.Parse("<attachment><filename>test.txt</filename></attachment>");
        Assert.True(XNode.DeepEquals(expected, XElement.Parse(output)), output);
    }

    [Theory]
    [MemberData(nameof(AttachmentDeserializeTheoryData))]
    public void Attachment_Should_Deserialize(string input, SerializerKind kind)
    {
        // arrange
        var serializer = SerializerFactory.Create(kind);

        var output = serializer.Deserialize<Redmine.Net.Api.Types.Attachment>(input);
        
        Assert.NotNull(output);
        Assert.Equal(6243, output.Id);
        Assert.Equal("test.txt", output.FileName);
        Assert.Equal(124, output.FileSize);
        Assert.Equal("text/plain", output.ContentType);
        Assert.Equal("This is an attachment", output.Description);
        Assert.Equal("http://localhost:3000/attachments/download/6243/test.txt", output.ContentUrl);
        Assert.Equal("Jean-Philippe Lang", output.Author.Name);
        Assert.Equal(1, output.Author.Id);
        AssertDateTime.Equal("2011-7-18T22:58:40+02:00", output.CreatedOn);
    }

    public static IEnumerable<TheoryDataRow<string, SerializerKind>> AttachmentDeserializeTheoryData()
    {
        const string json = """
                            {
                              "attachment": {
                                  "id": 6243,
                                  "filename": "test.txt",
                                  "filesize": 124,
                                  "content_type": "text/plain",
                                  "description": "This is an attachment",
                                  "content_url": "http://localhost:3000/attachments/download/6243/test.txt",
                                  "author": {
                                    "name": "Jean-Philippe Lang",
                                    "id": 1
                                  },
                                  "created_on": "2011-07-18T22:58:40+02:00"
                                }
                            }
                            """;

        const string xml = """
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

        yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
        // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
        yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
    }
}