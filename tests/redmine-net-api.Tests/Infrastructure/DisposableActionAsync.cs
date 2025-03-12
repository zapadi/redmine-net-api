using System;
using System.Threading.Tasks;

namespace Padi.DotNet.RedmineAPI.Tests.Infrastructure;

/// <summary>
/// A helper class that translate between Disposable and async action
/// </summary>
internal sealed class DisposableActionAsync : IAsyncDisposable
{
    private readonly Func<Task> _action;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisposableActionAsync"/> class.
    /// </summary>
    /// <param name="action">The async action.</param>
    public DisposableActionAsync(Func<Task> action)
    {
        _action = action;
    }

    /// <summary>
    /// Execute the relevant action
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        await _action().ConfigureAwait(false);
    }
}