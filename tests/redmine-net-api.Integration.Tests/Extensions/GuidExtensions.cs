namespace Padi.RedmineAPI.Integration.Tests.Extensions;

public static class GuidExtensions
{
    public static string ToNoDash(this Guid guid) => guid.ToString("N");
}