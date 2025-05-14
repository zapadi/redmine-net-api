using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#nullable enable

namespace Redmine.Net.Api.Internals;

internal static class ArgumentNullThrowHelper
{
    public static void ThrowIfNull(
    #if INTERNAL_NULLABLE_ATTRIBUTES || NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        [NotNull]
    #endif
        object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
    #if !NET7_0_OR_GREATER || NETSTANDARD || NETFRAMEWORK
        if (argument is null)
        {
            Throw(paramName);
        }
    #else
        ArgumentNullException.ThrowIfNull(argument, paramName);
    #endif
    }
        
    #if !NET7_0_OR_GREATER || NETSTANDARD || NETFRAMEWORK
    #if INTERNAL_NULLABLE_ATTRIBUTES || NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    [DoesNotReturn]
    #endif
    internal static void Throw(string? paramName) =>
        throw new ArgumentNullException(paramName);
    #endif
}