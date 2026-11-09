using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Padi.DotNet.RedmineAPI.Tests.Infrastructure;

internal sealed class ResettingServer : IAsyncDisposable
{
    private const string HeadersEnd = "\r\n\r\n";
    private static readonly TimeSpan ReadTimeout = TimeSpan.FromSeconds(5);

    private readonly TcpListener _listener;
    private readonly CancellationTokenSource _cts = new();
    private readonly Task _acceptLoop;
    private int _disposed;

    private ResettingServer(TcpListener listener)
    {
        _listener = listener;
        Url = $"http://127.0.0.1:{((IPEndPoint)listener.LocalEndpoint).Port}/";
        _acceptLoop = AcceptLoopAsync(_cts.Token);
    }

    public string Url { get; }

    public static ResettingServer Start()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        return new ResettingServer(listener);
    }

    private async Task AcceptLoopAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
#if NET
            var socket = await _listener.AcceptSocketAsync(cancellationToken).ConfigureAwait(false);
#else
            var socket = await _listener.AcceptSocketAsync().ConfigureAwait(false);
#endif
            using (socket)
            {
                socket.LingerState = new LingerOption(true, 0);
                await ReadHeadersAsync(socket, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    private static async Task ReadHeadersAsync(Socket socket, CancellationToken cancellationToken)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(ReadTimeout);
        using var registration = timeout.Token.Register(static state => ((Socket)state).Dispose(), socket);

        var buffer = new byte[4096];
        var matched = 0;
        try
        {
            int count;
            while ((count = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None).ConfigureAwait(false)) > 0)
            {
                for (var i = 0; i < count; i++)
                {
                    matched = buffer[i] == HeadersEnd[matched] ? matched + 1 : buffer[i] == '\r' ? 1 : 0;
                    if (matched == HeadersEnd.Length)
                    {
                        return;
                    }
                }
            }
        }
        catch (Exception exception) when (timeout.IsCancellationRequested
            || exception is SocketException { SocketErrorCode: SocketError.ConnectionReset or SocketError.ConnectionAborted })
        {
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        _cts.Cancel();
        _listener.Stop();
        try
        {
            await _acceptLoop.ConfigureAwait(false);
        }
        catch (Exception exception) when (IsShutdown(exception))
        {
        }
        finally
        {
            _cts.Dispose();
        }
    }

    private static bool IsShutdown(Exception exception) =>
        exception is OperationCanceledException
            or InvalidOperationException
            or SocketException { SocketErrorCode: SocketError.OperationAborted or SocketError.Interrupted };
}
