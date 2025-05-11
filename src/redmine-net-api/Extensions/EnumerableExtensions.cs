using System.Collections.Generic;
using System.Text;

namespace Redmine.Net.Api.Extensions;

/// <summary>
/// Provides extension methods for IEnumerable types.
/// </summary>
public static class EnumerableExtensions
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
    public static string Dump<TIn>(this IEnumerable<TIn> collection) where TIn : class
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
}