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
    public static T Parse<T>(string value, bool ignoreCase) where T : struct
    {
#if NETFRAMEWORK
        return (T)Enum.Parse(typeof(T), value, ignoreCase);
#else
        return Enum.Parse<T>(value, ignoreCase);
#endif
    }
}