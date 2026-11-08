using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Tests;

public class IdentifiableNameExtensionsTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ToIdentifier_Should_Throw_With_Descriptive_Message_When_Value_Is_Not_Positive(int value)
    {
        var exception = Assert.Throws<RedmineException>(() => value.ToIdentifier());

        Assert.Equal("The identifier must be greater than zero.", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ToIssueStatusIdentifier_Should_Throw_With_Descriptive_Message_When_Value_Is_Not_Positive(int value)
    {
        var exception = Assert.Throws<RedmineException>(() => value.ToIssueStatusIdentifier());

        Assert.Equal("The identifier must be greater than zero.", exception.Message);
    }
}
