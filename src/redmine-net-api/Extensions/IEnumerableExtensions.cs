using System;
using System.Collections.Generic;
using System.Text;
using Redmine.Net.Api.Common;

namespace Redmine.Net.Api.Extensions;

/// <summary>
/// Provides extension methods for IEnumerable types.
/// </summary>
public static class IEnumerableExtensions
{
    /// <summary>
    /// Converts a collection of objects into a string representation with each item separated by a comma
    /// and enclosed within curly braces.
    /// </summary>
    /// <typeparam name="TIn">The type of items in the collection. The type must be a reference type.</typeparam>
    /// <param name="collection">The collection of items to convert to a string representation.</param>
    /// <returns>
    /// Returns a string containing all the items from the collection, separated by commas and
    /// enclosed within curly braces. Returns null if the collection is null.
    /// </returns>
    internal static string Dump<TIn>(this IEnumerable<TIn> collection) where TIn : class
    {
        if (collection == null)
        {
            return null;
        }

        var sb = new StringBuilder("{");
            
        foreach (var item in collection)
        {
            sb.Append(item).Append(',');
        }

        if (sb.Length > 1)
        {
            sb.Length -= 1;
        }
            
        sb.Append('}');

        var str = sb.ToString();
        sb.Length = 0;

        return str;
    }
    
    /// <summary>
    /// Returns the index of the first item in the sequence that satisfies the predicate. If no item satisfies the predicate, -1 is returned.
    /// </summary>
    /// <typeparam name="T">The type of objects in the <see cref="IEnumerable{T}"/>.</typeparam>
    /// <param name="source"><see cref="IEnumerable{T}"/> in which to search.</param>
    /// <param name="predicate">Function performed to check whether an item satisfies the condition.</param>
    /// <returns>Return the zero-based index of the first occurrence of an element that satisfies the condition, if found; otherwise, -1.</returns>
    internal static int IndexOf<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        ArgumentVerifier.ThrowIfNull(predicate, nameof(predicate));

        var index = 0;

        foreach (var item in source)
        {
            if (predicate(item))
            {
                return index;
            }

            index++;
        }

        return -1;
    }
}