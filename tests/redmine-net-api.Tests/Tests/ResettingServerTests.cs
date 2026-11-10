using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Tests;

public sealed class ResettingServerTests
{
    [Fact(Timeout = Constants.TestTimeout)]
    public async Task Client_Reset_Mid_Request_Should_Not_Stop_The_Server()
    {
        await using var server = ResettingServer.Start();

        using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
            socket.Connect(IPAddress.Loopback, new Uri(server.Url).Port);
            socket.Send(Encoding.ASCII.GetBytes("GET / HTTP/1.1\r\nHost: 127.0.0.1\r\n"));
            socket.LingerState = new LingerOption(true, 0);
            socket.Close();
        }

        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
        var stopwatch = Stopwatch.StartNew();
        await Assert.ThrowsAsync<HttpRequestException>(() => client.GetAsync(server.Url, TestContext.Current.CancellationToken));
        stopwatch.Stop();

        Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(2), $"Request took {stopwatch.Elapsed}.");
    }
}
