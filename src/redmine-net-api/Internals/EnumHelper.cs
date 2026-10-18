using System;

namespace Redmine.Net.Api.Internals;

/// <summary>
/// 
/// </summary>
internal static class EnumHelper
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T ParseOrDefault<T>(string value, bool ignoreCase) where T : struct
    {
#if NET20
        return (T)Enum.Parse(typeof(T), value, ignoreCase);
#else
        return Enum.TryParse(value, ignoreCase, out T result) ? result : default;
#endif
    }
}