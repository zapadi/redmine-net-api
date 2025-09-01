using System.Collections.Generic;

namespace Padi.RedmineApi.Internals;

/// <summary>
/// Provides extension methods for operations on lists.
/// </summary>
internal static class ListExtensions
{
    /// <summary>
    /// Creates a deep clone of the specified list.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list. Must implement <see cref="ICloneable{T}"/>.</typeparam>
    /// <param name="listToClone">The list to be cloned.</param>
    /// <param name="resetId">Specifies whether to reset the ID for each cloned item.</param>
    /// <returns>A new list containing cloned copies of the elements from the original list. Returns null if the original list is null.</returns>
    public static List<T> Clone<T>(this List<T> listToClone, bool resetId) where T : ICloneable<T>
    {
        if (listToClone == null)
        {
            return null;
        }

        var clonedList = new List<T>(listToClone.Count);
            
        foreach (var item in listToClone)
        {
            clonedList.Add(item.Clone(resetId));
        }
        return clonedList;
    }

    /// <summary>
    /// Compares two lists for equality based on their elements.
    /// </summary>
    /// <typeparam name="T">The type of elements in the lists. Must be a reference type.</typeparam>
    /// <param name="list">The first list to compare.</param>
    /// <param name="listToCompare">The second list to compare.</param>
    /// <returns>True if both lists are non-null, have the same count, and all corresponding elements are equal; otherwise, false.</returns>
    public static bool Equals<T>(this List<T> list, List<T> listToCompare) where T : class
    {
        if (list == null || listToCompare == null)
        {
            return false;
        }
            
        if (list.Count != listToCompare.Count)
        {
            return false;
        }
            
        var index = 0;
        while (index < list.Count && list[index].Equals(listToCompare[index]))
        {
            index++;
        }

        return index == list.Count;
    }
}