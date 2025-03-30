using System.Diagnostics;
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

    [Conditional("DEBUG_JSON")]
    private void SetMimeTypeJson()
    {
        Format = "json";
    }

    [Conditional("DEBUG_XML")]
    private void SetMimeTypeXml()
    {
        Format = "json";
    }
}