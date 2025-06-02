using Padi.DotNet.RedmineAPI.Integration.Tests.Fixtures;
using Padi.DotNet.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests;

[Collection(Constants.RedmineTestContainerCollection)]
public class RedmineApiWebClientTests(RedmineTestContainerFixture fixture)
{
    [Fact]
    public async Task SendAsync_WhenRequestCanceled_ThrowsRedmineOperationCanceledException()
    {
        using var cts = new CancellationTokenSource();
        // Arrange
        cts.CancelAfter(TimeSpan.FromMilliseconds(100));

        // Act & Assert
        _ = await Assert.ThrowsAsync<RedmineOperationCanceledException>(async () =>
            await fixture.RedmineManager.GetAsync<Project>(cancellationToken: cts.Token));
    }

    [Fact]
    public async Task SendAsync_WhenWebExceptionOccurs_ThrowsRedmineApiException()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAnyAsync<RedmineApiException>(async () =>
            await fixture.RedmineManager.GetAsync<Project>("xyz"));

        Assert.NotNull(exception.InnerException);
    }

    [Fact]
    public async Task SendAsync_WhenOperationCanceled_ThrowsRedmineOperationCanceledException()
    {
        using var cts = new CancellationTokenSource();
        // Arrange
        await cts.CancelAsync();

        // Act & Assert
        _ = await Assert.ThrowsAsync<RedmineOperationCanceledException>(async () =>
            await fixture.RedmineManager.GetAsync<Project>(cancellationToken: cts.Token));
    }

    [Fact]
    public async Task SendAsync_WhenOperationTimedOut_ThrowsRedmineOperationCanceledException()
    {
        // Arrange
        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMilliseconds(1));

        // Act & Assert
        _ = await Assert.ThrowsAsync<RedmineOperationCanceledException>(async () =>
            await fixture.RedmineManager.GetAsync<Project>(cancellationToken: timeoutCts.Token));
    }

    [Fact]
    public async Task SendAsync_WhenTaskCanceled_ThrowsRedmineOperationCanceledException()
    {
        using var cts = new CancellationTokenSource();
        // Arrange
        cts.CancelAfter(TimeSpan.FromMilliseconds(50));

        // Act & Assert
        _ = await Assert.ThrowsAsync<RedmineOperationCanceledException>(async () =>
            await fixture.RedmineManager.GetAsync<Project>(cancellationToken: cts.Token));
    }

    [Fact]
    public async Task SendAsync_WhenGeneralException_ThrowsRedmineException()
    {
        // Act & Assert
        _ = await Assert.ThrowsAsync<RedmineException>(async () =>
            await fixture.RedmineManager.CreateAsync<Query>(null));
    }
}