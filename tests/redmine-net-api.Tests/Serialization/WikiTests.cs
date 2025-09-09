using System;
using System.Collections.Generic;
using System.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public class WikiTests()
{
    [Theory]
    [MemberData(nameof(WikiDeserializeTheoryData))]
    public void Should_Deserialize_Wiki_Page(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        
        var output = serializer.Deserialize<Redmine.Net.Api.Types.WikiPage>(input);
        
        Assert.NotNull(output);
        Assert.Equal("UsersGuide", output.Title);
        Assert.NotNull(output.ParentTitle);
        Assert.Equal("Installation_Guide", output.ParentTitle);
        
        Assert.NotNull(output.Text);
        Assert.False(string.IsNullOrWhiteSpace(output.Text), "Text should not be empty");

        var lines = output.Text!.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        var firstLine = lines[0].Trim();
        
        Assert.Equal("h1. Users Guide", firstLine);
        
        Assert.Equal(22, output.Version);
        Assert.NotNull(output.Author);
        Assert.Equal(11, output.Author.Id);
        Assert.Equal("John Smith", output.Author.Name);
        Assert.Equal("Typo", output.Comments);
        // AssertDateTime.Equal("2009-5-18T20:11:52", output.CreatedOn);
        // AssertDateTime.Equal("2012-10-2T11:38:18", output.UpdatedOn);

    }

    [Theory]
    [MemberData(nameof(WikisDeserializeTheoryData))]
    public void Should_Deserialize_Wiki_Pages(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
    
        var output = serializer.DeserializeToPagedResults<Redmine.Net.Api.Types.WikiPage>(input);
        
        Assert.NotNull(output);
        Assert.Equal(1, output.TotalItems);

        var wikiPages = output.Items.ToList();
        Assert.Equal("UsersGuide", wikiPages[0].Title);
        Assert.Equal(2, wikiPages[0].Version);
        // AssertDateTime.Equal("2008-3-9T12:07:08", wikiPages[0].CreatedOn);
        // AssertDateTime.Equal("2008-3-9T23:41:33", wikiPages[0].UpdatedOn);
    }

    [Theory]
    [MemberData(nameof(WikiEmptyDeserializeTheoryData))]
    public void Should_Deserialize_Empty_Wiki_Pages(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        
        var output = serializer.DeserializeToPagedResults<Redmine.Net.Api.Types.WikiPage>(input);
        
        Assert.NotNull(output);
        Assert.Equal(0, output.TotalItems);
    }
    
    [Theory]
    [MemberData(nameof(WikiWithAttachmentsDeserializeTheoryData))]
    public void Should_Deserialize_Wiki_With_Attachments(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        
        var output = serializer.Deserialize<Redmine.Net.Api.Types.WikiPage>(input);
        
        Assert.NotNull(output);
        Assert.Single(output.Attachments);
    }
    
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> WikiDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "wiki_page": {
                                    "title": "UsersGuide",
                                    "parent": {
                                      "title": "Installation_Guide"
                                    },
                                    "text": "h1. Users Guide\n            ...\n            ...",
                                    "version": 22,
                                    "author": {
                                      "id": 11,
                                      "name": "John Smith"
                                    },
                                    "comments": "Typo",
                                    "created_on": "2009-05-18T20:11:52Z",
                                    "updated_on": "2012-10-02T11:38:18Z"
                                  }
                                }
                                """;

            const string xml = """
                               <?xml version="1.0" encoding="UTF-8"?>
                               <wiki_page>
                                   <title>UsersGuide</title>
                                   <parent title="Installation_Guide"/>
                                   <text>h1. Users Guide
                                   ...
                                   ...</text>
                                   <version>22</version>
                                   <author id="11" name="John Smith"/>
                                   <comments>Typo</comments>
                                   <created_on>2009-05-18T20:11:52Z</created_on>
                                   <updated_on>2012-10-02T11:38:18Z</updated_on>
                               </wiki_page>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    } 
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> WikisDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "wiki_pages": [
                                    {
                                      "title": "UsersGuide",
                                      "version": 2,
                                      "created_on": "2008-03-09T12:07:08Z",
                                      "updated_on": "2008-03-09T23:41:33+01:00"
                                    }
                                  ]
                                }
                                """;

            const string xml = """
                               <?xml version="1.0"?>
                               <wiki_pages type="array">
                                   <wiki_page>
                                       <title>UsersGuide</title>
                                       <version>2</version>
                                       <created_on>2008-03-09T12:07:08Z</created_on>
                                       <updated_on>2008-03-09T23:41:33+01:00</updated_on>
                                       <?xml version="1.0" encoding="UTF-8" standalone="no"?>
                                   </wiki_page>
                               </wiki_pages>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    } 
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> WikiEmptyDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "wiki_pages": []
                                }
                                """;

            const string xml = """
                               <?xml version="1.0" encoding="UTF-8"?>
                               <wiki_pages type="array">
                               </wiki_pages>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    } 
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> WikiWithAttachmentsDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "wiki_page": {
                                    "title": "Test",
                                    "text": "QEcISExBVZ",
                                    "version": 3,
                                    "author": {
                                      "id": 1,
                                      "name": "Redmine Admin"
                                    },
                                    "comments": "uAqCrmSBDUpNMOU",
                                    "created_on": "2025-05-26T16:32:41Z",
                                    "updated_on": "2025-05-26T16:43:01Z",
                                    "attachments": [
                                      {
                                        "id": 155,
                                        "filename": "test-file_QPqCTEa",
                                        "filesize": 512000,
                                        "content_type": "text/plain",
                                        "description": "JIIMEcwtuZUsIHY",
                                        "content_url": "http://localhost:8089/attachments/download/155/test-file_QPqCTEa",
                                        "author": {
                                          "id": 1,
                                          "name": "Redmine Admin"
                                        },
                                        "created_on": "2025-05-26T16:32:36Z"
                                      }
                                    ]
                                  }
                                }
                                """;

            const string xml = """
                               <?xml version="1.0" encoding="UTF-8"?>
                               <wiki_page>
                                   <title>Test</title>
                                   <text>QEcISExBVZ</text>
                                   <version>3</version>
                                   <author id="1" name="Redmine Admin"/>
                                   <comments>uAqCrmSBDUpNMOU</comments>
                                   <created_on>2025-05-26T16:32:41Z</created_on>
                                   <updated_on>2025-05-26T16:43:01Z</updated_on>
                                   <attachments type="array">
                                       <attachment>
                                           <id>155</id>
                                           <filename>test-file_QPqCTEa</filename>
                                           <filesize>512000</filesize>
                                           <content_type>text/plain</content_type>
                                           <description>JIIMEcwtuZUsIHY</description>
                                           <content_url>http://localhost:8089/attachments/download/155/test-file_QPqCTEa</content_url>
                                           <author id="1" name="Redmine Admin"/>
                                           <created_on>2025-05-26T16:32:36Z</created_on>
                                       </attachment>
                                   </attachments>
                               </wiki_page>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
}

                            

                             