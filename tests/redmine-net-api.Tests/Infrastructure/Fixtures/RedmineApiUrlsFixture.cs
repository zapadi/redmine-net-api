using System.Diagnostics;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Net;

namespace Padi.DotNet.RedmineAPI.Tests.Tests;

public sealed class RedmineApiUrlsFixture
{
    internal string Format { get; private set; }

    public RedmineApiUrlsFixture()
    {
        SetMimeTypeJson();
        SetMimeTypeXml();
        
        Sut = new RedmineApiUrls(Format);
    }
    
    internal RedmineApiUrls Sut { get; } 

    [Conditional(Constants.ConditionalCompilationSymbol.DebugJson)]
    private void SetMimeTypeJson()
    {
        Format = RedmineConstants.JSON;
    }

    [Conditional(Constants.ConditionalCompilationSymbol.DebugXml)]
    private void SetMimeTypeXml()
    {
        Format = RedmineConstants.XML;
    }
}