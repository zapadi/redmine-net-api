using System;

namespace Padi.DotNet.RedmineAPI.Tests.Infrastructure;

/// <summary>
/// A helper class that translate between Disposable and Action
/// </summary>
internal sealed class DisposableAction : IDisposable
{
    private readonly Action _action;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisposableAction"/> class.
    /// </summary>
    /// <param name="action">The action.</param>
    public DisposableAction(Action action)
    {
        _action = action;
    }

    /// <summary>
    /// Execute the relevant actions
    /// </summary>
    public void Dispose()
    {
        _action();
    }
}