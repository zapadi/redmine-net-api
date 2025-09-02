using System.Net;

namespace Padi.RedmineApi.HttpClient;

internal static class HttpDecompressionMethods
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static DecompressionMethods Default()
    {
#if NET
        return DecompressionMethods.All;
#else
        return DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.None;
#endif
    }
}