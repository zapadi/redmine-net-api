namespace Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure.Options
{
    public sealed class RedmineOptions
    {
        public string Url { get; set; }

        public AuthenticationMode AuthenticationMode { get; set; }
        
        public AuthenticationOptions Authentication { get; set; }
        
        public int Port { get; set; }
        public string Image { get; set; } = string.Empty;
        public string SqlFilePath { get; set; } = string.Empty;
    }
}