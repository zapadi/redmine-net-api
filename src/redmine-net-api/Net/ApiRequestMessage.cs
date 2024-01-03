using System.Collections.Specialized;

namespace Redmine.Net.Api.Net;

internal sealed class ApiRequestMessage
{
    public ApiRequestMessageContent Content { get; set; }
    public string Method { get; set; } = HttpVerbs.GET;
    public string RequestUri { get; set; }
    public NameValueCollection QueryString { get; set; }
    public string ImpersonateUser { get; set; }
        
    public string ContentType { get; set; }
}