using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure
{
    public sealed class TestContainerOptions
    {
        public string Url { get; set; }

        public AuthenticationMode AuthenticationMode { get; set; }
        
        public Authentication Authentication { get; set; }
        
        public TestContainerMode Mode { get; set; }
    }

    public sealed class Authentication
    {
        public string ApiKey { get; set; }

        public BasicAuthentication Basic { get; set; }
    }

    public sealed class BasicAuthentication
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public enum AuthenticationMode
    {
        None,
        ApiKey,
        Basic
    }
}