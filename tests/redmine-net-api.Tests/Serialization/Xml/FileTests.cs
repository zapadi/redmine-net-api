using System;
using System.Collections.Generic;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure.Fixtures;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization.Xml;

[Collection(Constants.XmlRedmineSerializerCollection)]
public class FileTests(XmlSerializerFixture fixture)
{
    [Fact]
    public void Should_Deserialize_File()
    {
        const string input = """
        <?xml version="1.0" encoding="UTF-8"?>
        <file>
            <id>12</id>
            <filename>foo-1.0-setup.exe</filename>
            <filesize>74753799</filesize>
            <content_type>application/octet-stream</content_type>
            <description>Foo App for Windows</description>
            <content_url>http://localhost:3000/attachments/download/12/foo-1.0-setup.exe</content_url>
            <author id="1" name="Redmine Admin"/>
            <created_on>2017-01-04T09:12:32Z</created_on>
            <version id="2" name="1.0"/>
            <digest>1276481102f218c981e0324180bafd9f</digest>
            <downloads>12</downloads>
        </file>
        """;
        
        var output = fixture.Serializer.Deserialize<File>(input);
        Assert.NotNull(output);
        Assert.Equal(12, output.Id);
        Assert.Equal("foo-1.0-setup.exe", output.Filename);
        Assert.Equal("application/octet-stream", output.ContentType);
        Assert.Equal("Foo App for Windows", output.Description);
        Assert.Equal("http://localhost:3000/attachments/download/12/foo-1.0-setup.exe", output.ContentUrl);
        Assert.Equal(1, output.Author.Id);
        Assert.Equal("Redmine Admin", output.Author.Name);
        Assert.Equal(new DateTimeOffset(new DateTime(2017,01,04,09,12,32, DateTimeKind.Utc)), new DateTimeOffset(output.CreatedOn!.Value));
        Assert.Equal(2, output.Version.Id);
        Assert.Equal("1.0", output.Version.Name);
        Assert.Equal("1276481102f218c981e0324180bafd9f", output.Digest);
        Assert.Equal(12, output.Downloads);
    }

    [Fact]
    public void Should_Deserialize_Files()
    {
        const string input = """
        <?xml version="1.0" encoding="UTF-8"?>
        <files type="array">
            <file>
                <id>12</id>
                <filename>foo-1.0-setup.exe</filename>
                <filesize>74753799</filesize>
                <content_type>application/octet-stream</content_type>
                <description>Foo App for Windows</description>
                <content_url>http://localhost:3000/attachments/download/12/foo-1.0-setup.exe</content_url>
                <author id="1" name="Redmine Admin"/>
                <created_on>2017-01-04T09:12:32Z</created_on>
                <version id="2" name="1.0"/>
                <digest>1276481102f218c981e0324180bafd9f</digest>
                <downloads>12</downloads>
            </file>
            <file>
                <id>11</id>
                <filename>foo-1.0.dmg</filename>
                <filesize>6886287</filesize>
                <content_type>application/x-octet-stream</content_type>
                <description>Foo App for macOS</description>
                <content_url>http://localhost:3000/attachments/download/11/foo-1.0.dmg</content_url>
                <author id="1" name="Redmine Admin"/>
                <created_on>2017-01-04T09:12:07Z</created_on>
                <version id="2" name="1.0"/>
                <digest>14758f1afd44c09b7992073ccf00b43d</digest>
                <downloads>5</downloads>
            </file>
        </files>
        """;
        
        var output = fixture.Serializer.DeserializeToPagedResults<File>(input);
        Assert.NotNull(output);
    }
}