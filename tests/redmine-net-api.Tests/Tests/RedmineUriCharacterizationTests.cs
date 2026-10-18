using Redmine.Net.Api;
using Redmine.Net.Api.Exceptions;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Tests;

public sealed class RedmineUriCharacterizationTests
{
    [Theory]
    [InlineData("example.com", null, "https://example.com/")]
    [InlineData("example.com", "http", "http://example.com/")]
    [InlineData("http.example.com", null, "https://http.example.com/")]
    [InlineData("http://example.com", null, "http://example.com/")]
    [InlineData("https://example.com", "http", "https://example.com/")]
    [InlineData("https://example.com:8443", null, "https://example.com:8443/")]
    [InlineData("example.com:3000", null, "https://example.com:3000/")]
    [InlineData("192.168.1.10", null, "https://192.168.1.10/")]
    [InlineData("http://192.168.1.10:8080", null, "http://192.168.1.10:8080/")]
    [InlineData("http://[::1]:8080", null, "http://[::1]:8080/")]
    [InlineData("https://[2001:db8::1]/redmine/", null, "https://[2001:db8::1]/redmine/")]
    [InlineData("[::1]", null, "https://[::1]/")]
    [InlineData("example.com\\", null, "https://example.com/")]
    [InlineData("localhost:3000?x=http://y", null, "https://localhost:3000/")]
    [InlineData("example.com:3000/redmine", null, "https://example.com:3000/redmine/")]
    [InlineData("localhost:3000/redmine", null, "https://localhost:3000/redmine/")]
    [InlineData("127.0.0.1:3000", null, "https://127.0.0.1:3000/")]
    [InlineData("[::1]:8080", null, "https://[::1]:8080/")]
    [InlineData("fe80::1%4", null, "https://[fe80::1]/")]
    [InlineData("http://example.com/a/b.xml?x=1", null, "http://example.com/a/")]
    [InlineData("https://example.com/redmine.JSON", null, "https://example.com/")]
    [InlineData("http://example.com/redmine.json/", null, "http://example.com/redmine.json/")]
    [InlineData("::1", null, "https://[::1]/")]
    [InlineData("2001:db8::1", "http", "http://[2001:db8::1]/")]
    [InlineData("  example.com  ", null, "https://example.com/")]
    [InlineData("http://user:pass@example.com:8080/redmine/?q=1#x", null, "http://example.com:8080/redmine/")]
    [InlineData("localhost:3000", null, "https://localhost:3000/")]
    [InlineData("localhost:3000", "http", "http://localhost:3000/")]
    [InlineData("http://localhost:3000", null, "http://localhost:3000/")]
    [InlineData("http://localhost:3000/", null, "http://localhost:3000/")]
    [InlineData("http://example.com/redmine", null, "http://example.com/redmine/")]
    [InlineData("http://example.com/redmine/", null, "http://example.com/redmine/")]
    [InlineData("http://example.com/apps/redmine", null, "http://example.com/apps/redmine/")]
    [InlineData("http://example.com/redmine.v5", null, "http://example.com/redmine.v5/")]
    [InlineData("http://example.com/redmine.v5/", null, "http://example.com/redmine.v5/")]
    [InlineData("http://example.com/redmine/issues.xml", null, "http://example.com/redmine/")]
    [InlineData("http://example.com/redmine/issues.json", null, "http://example.com/redmine/")]
    [InlineData("http://example.com/issues.xml", null, "http://example.com/")]
    public void CreateRedmineUri_Valid_Hosts(string host, string scheme, string expected)
    {
        Assert.Equal(expected, RedmineManagerOptionsBuilder.CreateRedmineUri(host, scheme).ToString());
    }

    [Theory]
    [InlineData("httpsfoo.example", null, "https://httpsfoo.example/")]
    [InlineData("httpsfoo.example", "http", "http://httpsfoo.example/")]
    [InlineData("https-redmine.example.com", null, "https://https-redmine.example.com/")]
    public void CreateRedmineUri_Host_Starting_With_Https_Without_Scheme_Gets_Scheme(string host, string scheme, string expected)
    {
        Assert.Equal(expected, RedmineManagerOptionsBuilder.CreateRedmineUri(host, scheme).ToString());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("ftp://example.com")]
    [InlineData("example.com", "ftp")]
    [InlineData("http://")]
    [InlineData("localhost:99999")]
    [InlineData("localhost:abc")]
    [InlineData("example.com:abc")]
    [InlineData("example.com:3000:4000")]
    [InlineData("::1]")]
    [InlineData("[")]
    [InlineData(":::")]
    [InlineData("http://[")]
    [InlineData("a:b")]
    [InlineData("javascript:alert(1)")]
    [InlineData("example.com:3000", "ftp")]
    [InlineData("example.com", "")]
    [InlineData("example.com", " ")]
    [InlineData("<invalid>")]
    [InlineData("file:///C:/x.txt")]
    [InlineData("exa mple.com")]
    [InlineData("user:pass@example.com:8080")]
    public void CreateRedmineUri_Invalid_Hosts_Throw_RedmineException(string host, string scheme = null)
    {
        Assert.Throws<RedmineException>(() => RedmineManagerOptionsBuilder.CreateRedmineUri(host, scheme));
    }
    [Theory]
    [InlineData("http://example.com/my%20redmine", "http://example.com/my%20redmine/")]
    [InlineData("http://example.com/caf%C3%A9/", "http://example.com/caf%C3%A9/")]
    [InlineData("http://example.com/a b", "http://example.com/a%20b/")]
    public void CreateRedmineUri_Does_Not_Double_Escape_Path(string host, string expected)
    {
        Assert.Equal(expected, RedmineManagerOptionsBuilder.CreateRedmineUri(host).AbsoluteUri);
    }

    [Fact]
    public void CreateRedmineUri_Very_Long_Host_Throws_RedmineException()
    {
        Assert.Throws<RedmineException>(() => RedmineManagerOptionsBuilder.CreateRedmineUri(new string('a', 70000)));
    }
}
