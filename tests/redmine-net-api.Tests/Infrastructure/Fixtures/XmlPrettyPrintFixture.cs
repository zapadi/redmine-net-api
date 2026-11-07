using System;
using Padi.DotNet.RedmineAPI.Tests.Tests;
using Redmine.Net.Api;
using Xunit;

[assembly: AssemblyFixture(typeof(XmlPrettyPrintFixture))]

namespace Padi.DotNet.RedmineAPI.Tests.Tests;

public sealed class XmlPrettyPrintFixture
{
    public XmlPrettyPrintFixture()
    {
        Environment.SetEnvironmentVariable(RedmineConstants.PRETTY_XML_SWITCH, "1");
    }
}
