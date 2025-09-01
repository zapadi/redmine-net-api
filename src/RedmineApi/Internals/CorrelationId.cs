using System;

namespace Padi.RedmineApi.Internals;

internal static class CorrelationId
{
    public static string Generate()
    { 
        return Guid.NewGuid().ToString();
    }
}