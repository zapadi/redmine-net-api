namespace Redmine.Net.Api.Net.WebClient;

internal sealed class StreamApiRequestMessageContent : ByteArrayApiRequestMessageContent
{
    public StreamApiRequestMessageContent(byte[] content) : base(content)
    {
        ContentType = RedmineConstants.CONTENT_TYPE_APPLICATION_STREAM;
    }
}