using Padi.DotNet.RedmineAPI.Tests.Infrastructure.Order;
using Redmine.Net.Api;
using Redmine.Net.Api.Exceptions;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Tests
{
    [Trait("Redmine-api", "Host")]
    [Order(1)]
    public sealed class HostTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("string.Empty")]
        [InlineData("localhost")]
        [InlineData("http://")]
        [InlineData("<invalid>")]
        [InlineData("xyztuv")]
        [InlineData("ftp://example.com")]
        [InlineData("ftp://localhost:3000")]
        [InlineData("\"https://localhost:3000\"")]
        [InlineData("C:/test/path/file.txt")]
        [InlineData(@"\\host\share\some\directory\name\")]
        [InlineData("xyz:c:\abc")]
        [InlineData("file:///C:/test/path/file.txt")]
        [InlineData("file://server/filename.ext")]
        [InlineData("ftp://myUrl/../..")]
        [InlineData("ftp://myUrl/%2E%2E/%2E%2E")]
        [InlineData("example--domain.com")]
        [InlineData("-example.com")]
        [InlineData("example.com-")]
        [InlineData("example.com/-")]
        [InlineData("invalid-host")]
        public void Should_Throw_Redmine_Exception_When_Host_Is_Invalid(string host)
        {
            // Arrange
            var optionsBuilder = new RedmineManagerOptionsBuilder().WithHost(host);
            
            // Act and Assert
            Assert.Throws<RedmineException>(() => optionsBuilder.Build());
        }

        [Theory]
        [InlineData("192.168.0.1", "https://192.168.0.1/")]
        [InlineData("127.0.0.1", "https://127.0.0.1/")]
        [InlineData("localhost:3000", "https://localhost:3000/")]
        [InlineData("localhost:3000/", "https://localhost:3000/")]
        [InlineData("https://localhost:3000/", "https://localhost:3000/")]
        [InlineData("example.com", "https://example.com/")]
        [InlineData("www.example.com", "https://www.example.com/")]
        [InlineData("www.domain.com/", "https://www.domain.com/")]
        [InlineData("www.domain.com:3000", "https://www.domain.com:3000/")]
        [InlineData("https://www.google.com", "https://www.google.com/")]
        [InlineData("http://example.com:8080", "http://example.com:8080/")]
        [InlineData("http://example.com/path", "http://example.com/path")]
        [InlineData("http://example.com?param=value", "http://example.com/")]
        [InlineData("http://example.com#fragment", "http://example.com/")]
        [InlineData("http://example.com/", "http://example.com/")]
        [InlineData("http://example.com/?param=value", "http://example.com/")]
        [InlineData("http://example.com/#fragment", "http://example.com/")]
        [InlineData("http://example.com/path/page", "http://example.com/path/page")]
        [InlineData("http://example.com/path/page?param=value", "http://example.com/path/page")]
        [InlineData("http://example.com/path/page#fragment","http://example.com/path/page")]
        [InlineData("http://[::1]:8080", "http://[::1]/")]
        [InlineData("http://www.domain.com/title/index.htm", "http://www.domain.com/")]
        [InlineData("http://www.localhost.com/", "http://www.localhost.com/")]
        [InlineData("https://www.localhost.com/", "https://www.localhost.com/")]
        [InlineData("http://www.domain.com/", "http://www.domain.com/")]
        [InlineData("http://www.domain.com/catalog/shownew.htm?date=today", "http://www.domain.com/")]
        [InlineData("HTTP://www.domain.com:80//thick%20and%20thin.htm", "http://www.domain.com/")]
        [InlineData("http://www.domain.com/index.htm#search", "http://www.domain.com/")]
        [InlineData("http://www.domain.com:8080/", "http://www.domain.com:8080/")]
        [InlineData("https://www.domain.com:8080/", "https://www.domain.com:8080/")]
        [InlineData("http://[fe80::200:39ff:fe36:1a2d%254]/", "http://[fe80::200:39ff:fe36:1a2d]/")]
        [InlineData("http://myUrl/%2E%2E/%2E%2E", "http://myurl/")]
        [InlineData("http://[fe80::200:39ff:fe36:1a2d%254]/temp/example.htm", "http://[fe80::200:39ff:fe36:1a2d]/")]
        [InlineData("http://myUrl/../..", "http://myurl/")]
        [InlineData("http://user:password@www.localhost.com/index.htm ", "http://www.localhost.com/")]
        public void Should_Not_Throw_Redmine_Exception_When_Host_Is_Valid(string host, string expected)
        {
            // Arrange
            var optionsBuilder = new RedmineManagerOptionsBuilder().WithHost(host);
            
            // Act
           var options = optionsBuilder.Build();
           
           // Assert
           Assert.NotNull(options);
           Assert.Equal(expected, options.BaseAddress.ToString());
        }
    }
}