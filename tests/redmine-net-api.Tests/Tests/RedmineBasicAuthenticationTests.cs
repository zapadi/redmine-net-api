using Redmine.Net.Api.Authentication;
using Redmine.Net.Api.Exceptions;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Tests;

public class RedmineBasicAuthenticationTests
{
    [Theory]
    [InlineData(null, "password", "The username for basic authentication cannot be null.")]
    [InlineData("username", null, "The password for basic authentication cannot be null.")]
    public void Should_Throw_With_Descriptive_Message_When_Credentials_Are_Null(string username, string password, string expected)
    {
        var exception = Assert.Throws<RedmineException>(() => new RedmineBasicAuthentication(username, password));

        Assert.Equal(expected, exception.Message);
    }
}
