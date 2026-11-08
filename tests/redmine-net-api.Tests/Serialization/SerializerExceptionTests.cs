using System.Collections.Generic;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Serialization;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public class SerializerExceptionTests
{
    [Theory]
    [MemberData(nameof(InvalidIssueDeserializeTheoryData))]
    public void Should_Wrap_Deserialize_Parse_Failure_In_RedmineException(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);

        var exception = Assert.Throws<RedmineException>(() => serializer.Deserialize<Issue>(input));

        Assert.NotNull(exception.InnerException);
        Assert.IsNotType<RedmineException>(exception.InnerException);
    }

    [Theory]
    [MemberData(nameof(InvalidIssuesDeserializeTheoryData))]
    public void Should_Wrap_DeserializeToPagedResults_Parse_Failure_In_RedmineException(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);

        var exception = Assert.Throws<RedmineException>(() => serializer.DeserializeToPagedResults<Issue>(input));

        Assert.NotNull(exception.InnerException);
        Assert.IsNotType<RedmineException>(exception.InnerException);
    }

    [Theory]
    [MemberData(nameof(InvalidIssuesDeserializeTheoryData))]
    public void Should_Wrap_Count_Parse_Failure_In_RedmineException(string input, SerializerKind kind)
    {
        var exception = Assert.Throws<RedmineException>(() => kind == SerializerKind.Xml
            ? new XmlRedmineSerializer().Count<Issue>(input)
            : new JsonRedmineSerializer().Count<Issue>(input));

        Assert.NotNull(exception.InnerException);
        Assert.IsNotType<RedmineException>(exception.InnerException);
    }

    [Theory]
    [InlineData(SerializerKind.Xml)]
    [InlineData(SerializerKind.NewtonsoftJson)]
    public void Should_Keep_RedmineException_Message_When_Serializing_Invalid_Entity(SerializerKind kind)
    {
        var relation = new IssueRelation { IssueToId = 2, Type = IssueRelationType.Undefined };

        var exception = Assert.Throws<RedmineException>(() => SerializerFactory.Create(kind).Serialize(relation));

        Assert.Equal("The value `IssueRelationType.`Undefined` is not allowed to create relations!", exception.Message);
    }

    public static IEnumerable<TheoryDataRow<string, SerializerKind>> InvalidIssueDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "issue": {
                                    "id": "abc",
                                    "subject": "subject"
                                  }
                                }
                                """;

            const string xml = """
                               <?xml version="1.0" encoding="UTF-8"?>
                               <issue>
                                   <id>1</id>
                                   <subject>subject
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }

    public static IEnumerable<TheoryDataRow<string, SerializerKind>> InvalidIssuesDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "issues": [
                                    { "id": 1, "subject": "subject" }
                                  ],
                                  "total_count": "abc"
                                """;

            const string xml = """
                               <?xml version="1.0" encoding="UTF-8"?>
                               <issues total_count="1 offset="0" limit="25" type="array">
                                   <issue>
                                       <id>1</id>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
}
