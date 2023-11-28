using System.Collections.Specialized;

namespace Redmine.Net.Api.Net;

internal sealed class ApiResponseMessage
{
    public NameValueCollection Headers { get; init; }
    public byte[] Content { get; init; }
}