using System;
using System.Threading.Tasks;
using Redmine.Net.Api;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Tests;

public sealed class XmlPrettyPrintFixture : IAsyncLifetime
{
    public XmlPrettyPrintFixture()
    {
        Environment.SetEnvironmentVariable(RedmineConstants.PRETTY_XML_SWITCH, "1");
    }

    public ValueTask InitializeAsync() => new();
    public ValueTask DisposeAsync() => new();
}