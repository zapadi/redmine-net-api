using System.Collections.Generic;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public class FileTests()
{
    [Theory]
    [MemberData(nameof(FileDeserializeTheoryData))]
    public void Should_Deserialize_File(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        
        var output = serializer.Deserialize<File>(input);
        Assert.NotNull(output);
        Assert.Equal(12, output.Id);
        Assert.Equal("foo-1.0-setup.exe", output.Filename);
        Assert.Equal("application/octet-stream", output.ContentType);
        Assert.Equal("Foo App for Windows", output.Description);
        Assert.Equal("http://localhost:3000/attachments/download/12/foo-1.0-setup.exe", output.ContentUrl);
        Assert.Equal(1, output.Author.Id);
        Assert.Equal("Redmine Admin", output.Author.Name);
        AssertDateTime.Equal("2017-01-04T09:12:32Z", output.CreatedOn);
        Assert.Equal(2, output.Version.Id);
        Assert.Equal("1.0", output.Version.Name);
        Assert.Equal("1276481102f218c981e0324180bafd9f", output.Digest);
        Assert.Equal(12, output.Downloads);
    }

    [Theory]
    [MemberData(nameof(FilesDeserializeTheoryData))]
    public void Should_Deserialize_Files(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        
        var output = serializer.DeserializeToPagedResults<File>(input);
        Assert.NotNull(output);
    }
    
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> FileDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                "file": 
                                  {
                                    "id": 12,
                                    "filename": "foo-1.0-setup.exe",
                                    "filesize": 74753799,
                                    "content_type": "application/octet-stream",
                                    "description": "Foo App for Windows",
                                    "content_url": "http://localhost:3000/attachments/download/12/foo-1.0-setup.exe",
                                    "author": {
                                      "id": 1,
                                      "name": "Redmine Admin"
                                    },
                                    "created_on": "2017-01-04T09:12:32Z",
                                    "version": {
                                      "id": 2,
                                      "name": "1.0"
                                    },
                                    "digest": "1276481102f218c981e0324180bafd9f",
                                    "downloads": 12
                                  }
                                }
                                """;

            const string xml = """
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

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
    
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> FilesDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "files": [
                                    {
                                      "id": 12,
                                      "filename": "foo-1.0-setup.exe",
                                      "filesize": 74753799,
                                      "content_type": "application/octet-stream",
                                      "description": "Foo App for Windows",
                                      "content_url": "http://localhost:3000/attachments/download/12/foo-1.0-setup.exe",
                                      "author": {
                                        "id": 1,
                                        "name": "Redmine Admin"
                                      },
                                      "created_on": "2017-01-04T09:12:32Z",
                                      "version": {
                                        "id": 2,
                                        "name": "1.0"
                                      },
                                      "digest": "1276481102f218c981e0324180bafd9f",
                                      "downloads": 12
                                    },
                                    {
                                      "id": 11,
                                      "filename": "foo-1.0.dmg",
                                      "filesize": 6886287,
                                      "content_type": "application/x-octet-stream",
                                      "description": "Foo App for macOS",
                                      "content_url": "http://localhost:3000/attachments/download/11/foo-1.0.dmg",
                                      "author": {
                                        "id": 1,
                                        "name": "Redmine Admin"
                                      },
                                      "created_on": "2017-01-04T09:12:07Z",
                                      "version": {
                                        "id": 2,
                                        "name": "1.0"
                                      },
                                      "digest": "14758f1afd44c09b7992073ccf00b43d",
                                      "downloads": 5
                                    }
                                  ]
                                }
                                """;

            const string xml = """
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

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
}