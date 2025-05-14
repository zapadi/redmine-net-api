using System;
using System.Diagnostics.CodeAnalysis;

namespace Redmine.Net.Api.Common;

/// <summary>
/// A utility class to perform argument validations. 
/// </summary>
internal static class ArgumentVerifier
{
    /// <summary>
    /// Throws ArgumentNullException if the argument is null.
    /// </summary>
    /// <param name="value">Argument value to check.</param>
    /// <param name="name">Name of Argument.</param>
    public static void ThrowIfNull([NotNull] object value, string name)
    {
        if (value == null)
        {
            throw new ArgumentNullException(name);
        }
    }

    /// <summary>
    /// Validates string and throws:
    /// ArgumentNullException if the argument is null.
    /// ArgumentException if the argument is empty.
    /// </summary>
    /// <param name="value">Argument value to check.</param>
    /// <param name="name">Name of Argument.</param>
    public static void ThrowIfNullOrEmpty([NotNull] string value, string name)
    {
        if (value == null)
        {
            throw new ArgumentNullException(name);
        }

        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException("The value cannot be null or empty", name);
        }
    }
}