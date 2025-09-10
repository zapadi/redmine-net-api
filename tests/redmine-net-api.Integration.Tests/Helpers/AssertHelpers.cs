using Xunit;

namespace Padi.RedmineAPI.Integration.Tests.Helpers;

internal static class AssertHelpers
{
    /// <summary>
    /// Asserts that two <see cref="float"/> values are equal within the specified tolerance.
    /// </summary>
    public static void Equal(float expected, float actual, float tolerance = 1e-4f)
        => Assert.InRange(actual, expected - tolerance, expected + tolerance);

    /// <summary>
    /// Asserts that two <see cref="decimal"/> values are equal within the specified tolerance.
    /// </summary>
    public static void Equal(decimal expected, decimal actual, decimal tolerance = 0.0001m)
        => Assert.InRange(actual, expected - tolerance, expected + tolerance);
    
    /// <summary>
    /// Asserts that two <see cref="DateTime"/> values are equal within the supplied tolerance.
    /// Kind is ignored – both values are first converted to UTC.
    /// </summary>
    public static void Equal(DateTime expected, DateTime actual, TimeSpan? tolerance = null)
    {
        tolerance ??= TimeSpan.FromSeconds(1);

        var expectedUtc = expected.ToUniversalTime();
        var actualUtc   = actual.ToUniversalTime();

        Assert.InRange(actualUtc, expectedUtc - tolerance.Value, expectedUtc + tolerance.Value);
    }

}
