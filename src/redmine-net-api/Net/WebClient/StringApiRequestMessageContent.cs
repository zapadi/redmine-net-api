using System;
using System.Text;

namespace Redmine.Net.Api.Net.WebClient;

internal sealed class StringApiRequestMessageContent : ByteArrayApiRequestMessageContent
{
    private static readonly Encoding DefaultStringEncoding = Encoding.UTF8;

    public StringApiRequestMessageContent(string content, string mediaType) : this(content, mediaType, DefaultStringEncoding)
    {
    }

    public StringApiRequestMessageContent(string content, string mediaType, Encoding encoding) : base(GetContentByteArray(content, encoding))
    {
        ContentType = mediaType;
    }

    private static byte[] GetContentByteArray(string content, Encoding encoding)
    {
        #if NET5_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(content);
        #else
        if (content == null)
        {
            throw new ArgumentNullException(nameof(content));
        }
        #endif
        return (encoding ?? DefaultStringEncoding).GetBytes(content);
    }
}