using System;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Tests;

[Collection(Constants.DeserializeCollection)]
public sealed class TimeEntryCommentsTests
{
    private const string BaseAddress = "http://localhost:3000/";

    [Fact]
    public void Builder_Should_Default_Time_Entry_Comments_Max_Length_To_255()
    {
        const string expected = """
                                <time_entry>
                                <spent_on>2024-06-15</spent_on>
                                <hours>1</hours>
                                <comments>aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa</comments>
                                </time_entry>
                                """;
        var options = new RedmineManagerOptionsBuilder().WithHost(BaseAddress).Build();

        var output = options.Serializer.Serialize(new TimeEntry { SpentOn = new DateTime(2024, 6, 15), Hours = 1, Comments = new string('a', 300) });

        Assert.Equal(expected, output);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Builder_Should_Throw_When_Time_Entry_Comments_Max_Length_Is_Not_Positive(int maxLength)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new RedmineManagerOptionsBuilder().WithTimeEntryCommentsMaxLength(maxLength));
    }

    [Fact]
    public void Builder_Should_Pass_Time_Entry_Comments_Max_Length_To_The_Serializer()
    {
        const string expected = """
                                <time_entry>
                                <spent_on>2024-06-15</spent_on>
                                <hours>1</hours>
                                <comments>abcde</comments>
                                </time_entry>
                                """;
        var options = new RedmineManagerOptionsBuilder().WithHost(BaseAddress).WithTimeEntryCommentsMaxLength(5).Build();

        var output = options.Serializer.Serialize(new TimeEntry { SpentOn = new DateTime(2024, 6, 15), Hours = 1, Comments = "abcdefgh" });

        Assert.Equal(expected, output);
    }
}
