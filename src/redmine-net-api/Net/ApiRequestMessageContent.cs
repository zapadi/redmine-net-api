namespace Redmine.Net.Api.Net;

internal abstract class ApiRequestMessageContent
{
    public string ContentType { get; internal set; }

    public byte[] Body { get; internal set; }
}