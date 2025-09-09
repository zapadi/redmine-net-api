using System.Collections.Generic;
using System.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public class NewsTests()
{
    [Theory]
    [MemberData(nameof(NewsDeserializeTheoryData))]
    public void Should_Deserialize_News(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
       
        var output = serializer.DeserializeToPagedResults<Redmine.Net.Api.Types.News>(input);

        Assert.NotNull(output);
        Assert.Equal(2, output.TotalItems);

        var newsItems = output.Items.ToList();
        Assert.Equal(2, newsItems.Count);

        Assert.Equal(54, newsItems[0].Id);
        Assert.Equal("Redmine", newsItems[0].Project.Name);
        Assert.Equal(1, newsItems[0].Project.Id);
        Assert.Equal("Jean-Philippe Lang", newsItems[0].Author.Name);
        Assert.Equal(1, newsItems[0].Author.Id);
        Assert.Equal("Redmine 1.1.3 released", newsItems[0].Title);
        Assert.Equal("Redmine 1.1.3 has been released", newsItems[0].Description);
        AssertDateTime.Equal("2011-4-29T12:00:25", newsItems[0].CreatedOn);

        Assert.Equal(53, newsItems[1].Id);
        Assert.Equal("Redmine", newsItems[1].Project.Name);
        Assert.Equal(1, newsItems[1].Project.Id);
        Assert.Equal("Jean-Philippe Lang", newsItems[1].Author.Name);
        Assert.Equal(1, newsItems[1].Author.Id);
        Assert.Equal("Redmine 1.1.2 bug/security fix released", newsItems[1].Title);
        Assert.Equal("Redmine 1.1.2 has been released", newsItems[1].Description);
        AssertDateTime.Equal("2011-3-7T20:07:03", newsItems[1].CreatedOn);

    }
    
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> NewsDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "news": [
                                    {
                                      "id": 54,
                                      "project": {
                                        "id": 1,
                                        "name": "Redmine"
                                      },
                                      "author": {
                                        "id": 1,
                                        "name": "Jean-Philippe Lang"
                                      },
                                      "title": "Redmine 1.1.3 released",
                                      "summary": null,
                                      "description": "Redmine 1.1.3 has been released",
                                      "created_on": "2011-04-29T14:00:25+02:00"
                                    },
                                    {
                                      "id": 53,
                                      "project": {
                                        "id": 1,
                                        "name": "Redmine"
                                      },
                                      "author": {
                                        "id": 1,
                                        "name": "Jean-Philippe Lang"
                                      },
                                      "title": "Redmine 1.1.2 bug/security fix released",
                                      "summary": null,
                                      "description": "Redmine 1.1.2 has been released",
                                      "created_on": "2011-03-07T21:07:03+01:00"
                                    }
                                  ],
                                  "limit": 25,
                                  "total_count": 2,
                                  "offset": 0
                                }
                                """;

            const string xml = """
                               <?xml version="1.0" encoding="UTF-8"?>
                               <news type="array" limit="25" total_count="2" offset="0">
                                   <news>
                                       <id>54</id>
                                       <project name="Redmine" id="1"/>
                                       <author name="Jean-Philippe Lang" id="1"/>
                                       <title>Redmine 1.1.3 released</title>
                                       <summary/>
                                       <description>Redmine 1.1.3 has been released</description>
                                       <created_on>2011-04-29T14:00:25+02:00</created_on>
                                   </news>
                                   <news>
                                       <id>53</id>
                                       <project name="Redmine" id="1"/>
                                       <author name="Jean-Philippe Lang" id="1"/>
                                       <title>Redmine 1.1.2 bug/security fix released</title>
                                       <summary/>
                                       <description>Redmine 1.1.2 has been released</description>
                                       <created_on>2011-03-07T21:07:03+01:00</created_on>
                                   </news>
                               </news>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
}

