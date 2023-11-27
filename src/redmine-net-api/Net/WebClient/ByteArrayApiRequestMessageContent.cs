namespace Redmine.Net.Api.Net.WebClient;

internal class ByteArrayApiRequestMessageContent : ApiRequestMessageContent
{
    public ByteArrayApiRequestMessageContent(byte[] content)
    {
        Body = content;
    }
}