using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Padi.DotNet.RedmineAPI.Tests.Infrastructure;

internal sealed class ResettingServer : IDisposable
{
    private readonly TcpListener _listener = new(IPAddress.Loopback, 0);

    public ResettingServer()
    {
        _listener.Start();
        Url = $"http://127.0.0.1:{((IPEndPoint)_listener.LocalEndpoint).Port}/";
        _ = Task.Run(Accept);
    }

    public string Url { get; }

    private void Accept()
    {
        while (true)
        {
            Socket socket;
            try
            {
                socket = _listener.AcceptSocket();
            }
            catch (Exception exception) when (exception is SocketException or ObjectDisposedException or InvalidOperationException)
            {
                return;
            }

            try
            {
                socket.LingerState = new LingerOption(true, 0);
                socket.ReceiveTimeout = 5000;
                ReadRequestHeaders(socket);
            }
            catch (Exception exception) when (exception is SocketException or ObjectDisposedException)
            {
            }
            finally
            {
                socket.Close();
            }
        }
    }

    private static void ReadRequestHeaders(Socket socket)
    {
        var received = new StringBuilder();
        var buffer = new byte[4096];
        int count;
        while (received.ToString().IndexOf("\r\n\r\n", StringComparison.Ordinal) < 0 && (count = socket.Receive(buffer)) > 0)
        {
            received.Append(Encoding.ASCII.GetString(buffer, 0, count));
        }
    }

    public void Dispose()
    {
        _listener.Stop();
    }
}