using System;
using System.Collections.Generic;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public class TimeEntryTests
{
    [Theory]
    [MemberData(nameof(TimeEntryWithCommentsTruncatedToTenSerializeTheoryData))]
    public void Should_Serialize_Time_Entry_Comments_Truncated_To_The_Serializer_Limit(string expected, SerializerKind kind)
    {
        var timeEntry = CreateTimeEntry("abcdefghijklmnop");

        var output = SerializerFactory.Create(kind, 10).Serialize(timeEntry);

        Assert.Equal(expected, output, ignoreLineEndingDifferences: true);
        Assert.Equal("abcdefghijklmnop", timeEntry.Comments);
    }

    [Theory]
    [MemberData(nameof(TimeEntryWithCommentsTruncatedTo255SerializeTheoryData))]
    public void Should_Serialize_Time_Entry_Comments_Truncated_To_255_By_Default(string expected, SerializerKind kind)
    {
        var timeEntry = CreateTimeEntry(new string('a', 300));

        var output = SerializerFactory.Create(kind).Serialize(timeEntry);

        Assert.Equal(expected, output, ignoreLineEndingDifferences: true);
        Assert.Equal(new string('a', 300), timeEntry.Comments);
    }

    [Theory]
    [MemberData(nameof(TimeEntryWithShortCommentsSerializeTheoryData))]
    public void Should_Serialize_Time_Entry_Comments_Shorter_Than_The_Limit_Unchanged(string expected, SerializerKind kind)
    {
        var timeEntry = CreateTimeEntry("short comment");

        var output = SerializerFactory.Create(kind).Serialize(timeEntry);

        Assert.Equal(expected, output, ignoreLineEndingDifferences: true);
    }

    private static TimeEntry CreateTimeEntry(string comments)
    {
        return new TimeEntry
        {
            Issue = new IdentifiableName { Id = 5 },
            SpentOn = new DateTime(2024, 6, 15),
            Hours = 1.5m,
            Comments = comments
        };
    }

    [Theory]
    [MemberData(nameof(TimeEntryWithLongCommentsDeserializeTheoryData))]
    public void Should_Deserialize_Time_Entry_Without_Truncating_Comments(string input, SerializerKind kind)
    {
        var output = SerializerFactory.Create(kind).Deserialize<TimeEntry>(input);

        Assert.Equal(1, output.Id);
        Assert.Equal(new string('a', 300), output.Comments);
    }

    public static IEnumerable<TheoryDataRow<string, SerializerKind>> TimeEntryWithLongCommentsDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "time_entry": {
                                    "id": 1,
                                    "hours": 1.0,
                                    "comments": "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
                                  }
                                }
                                """;

            const string xml = """
                               <?xml version="1.0" encoding="UTF-8"?>
                               <time_entry>
                                   <id>1</id>
                                   <hours>1.0</hours>
                                   <comments>aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa</comments>
                               </time_entry>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }

    public static IEnumerable<TheoryDataRow<string, SerializerKind>> TimeEntryWithCommentsTruncatedToTenSerializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "time_entry": {
                                    "issue_id": 5,
                                    "spent_on": "2024-06-15",
                                    "hours": 1.5,
                                    "comments": "abcdefghij"
                                  }
                                }
                                """;

            const string xml = """
                               <time_entry>
                               <issue_id>5</issue_id>
                               <spent_on>2024-06-15</spent_on>
                               <hours>1.5</hours>
                               <comments>abcdefghij</comments>
                               </time_entry>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }

    public static IEnumerable<TheoryDataRow<string, SerializerKind>> TimeEntryWithCommentsTruncatedTo255SerializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "time_entry": {
                                    "issue_id": 5,
                                    "spent_on": "2024-06-15",
                                    "hours": 1.5,
                                    "comments": "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
                                  }
                                }
                                """;

            const string xml = """
                               <time_entry>
                               <issue_id>5</issue_id>
                               <spent_on>2024-06-15</spent_on>
                               <hours>1.5</hours>
                               <comments>aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa</comments>
                               </time_entry>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }

    public static IEnumerable<TheoryDataRow<string, SerializerKind>> TimeEntryWithShortCommentsSerializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "time_entry": {
                                    "issue_id": 5,
                                    "spent_on": "2024-06-15",
                                    "hours": 1.5,
                                    "comments": "short comment"
                                  }
                                }
                                """;

            const string xml = """
                               <time_entry>
                               <issue_id>5</issue_id>
                               <spent_on>2024-06-15</spent_on>
                               <hours>1.5</hours>
                               <comments>short comment</comments>
                               </time_entry>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
}
