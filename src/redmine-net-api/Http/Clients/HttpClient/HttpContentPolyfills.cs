
#if !(NET20 || NET5_0_OR_GREATER)

using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Redmine.Net.Api.Http.Clients.HttpClient;

internal static class HttpContentPolyfills
{
    internal static Task<string> ReadAsStringAsync(this HttpContent httpContent, CancellationToken cancellationToken)
        => httpContent.ReadAsStringAsync(
#if !NETFRAMEWORK
                                cancellationToken
#endif
            );

    internal static Task<Stream> ReadAsStreamAsync(this HttpContent httpContent, CancellationToken cancellationToken)
        => httpContent.ReadAsStreamAsync(
#if !NETFRAMEWORK
                                cancellationToken
#endif
            );

    internal static Task<byte[]> ReadAsByteArrayAsync(this HttpContent httpContent, CancellationToken cancellationToken)
        => httpContent.ReadAsByteArrayAsync(
#if !NETFRAMEWORK
                                cancellationToken
#endif
            );
}

#endif