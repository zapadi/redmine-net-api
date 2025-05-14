using System.Text;

namespace Padi.DotNet.RedmineAPI.Integration.Tests;

public static class ThreadSafeRandom
{
    /// <summary>
    /// Generates a cryptographically strong, random string suffix.
    /// This method is thread-safe as Guid.NewGuid() is thread-safe.
    /// </summary>
    /// <returns>A random string, 32 characters long, consisting of hexadecimal characters, without hyphens.</returns>
    public static string GenerateSuffix()
    {
        return Guid.NewGuid().ToString("N");
    }

    private static readonly char[] EnglishAlphabetChars =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();

    // ThreadLocal ensures that each thread has its own instance of Random,
    // which is important because System.Random is not thread-safe for concurrent use.
    // Seed with Guid for better randomness across instances
    private static readonly ThreadLocal<Random> ThreadRandom =
        new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));

    /// <summary>
    /// Generates a random string of a specified length using only English alphabet characters.
    /// This method is thread-safe.
    /// </summary>
    /// <param name="length">The desired length of the random string. Defaults to 10.</param>
    /// <returns>A random string composed of English alphabet characters.</returns>
    private static string GenerateRandomAlphaNumericString(int length = 10)
    {
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length), "Length must be a positive integer.");
        }

        var random = ThreadRandom.Value;
        var result = new StringBuilder(length);
        for (var i = 0; i < length; i++)
        {
            result.Append(EnglishAlphabetChars[random.Next(EnglishAlphabetChars.Length)]);
        }

        return result.ToString();
    }

    /// <summary>
    /// Generates a random alphabetic suffix, defaulting to 10 characters.
    /// This method is thread-safe.
    /// </summary>
    /// <param name="length">The desired length of the suffix. Defaults to 10.</param>
    /// <returns>A random alphabetic string.</returns>
    public static string GenerateText(int length = 10)
    {
        return GenerateRandomAlphaNumericString(length);
    }

    /// <summary>
    /// Generates a random name by combining a specified prefix and a random alphabetic suffix.
    /// This method is thread-safe.
    /// Example: if prefix is "MyItem", the result could be "MyItem_aBcDeFgHiJ".
    /// </summary>
    /// <param name="prefix">The prefix for the name. A '_' separator will be added.</param>
    /// <param name="suffixLength">The desired length of the random suffix. Defaults to 10.</param>
    /// <returns>A string combining the prefix, an underscore, and a random alphabetic suffix.
    /// If the prefix is null or empty, it returns just the random suffix.</returns>
    public static string GenerateText(string prefix = null, int suffixLength = 10)
    {
        var suffix = GenerateRandomAlphaNumericString(suffixLength);
        return string.IsNullOrEmpty(prefix) ? suffix : $"{prefix}_{suffix}";
    }

    // Fisher-Yates shuffle algorithm
    public static void Shuffle<T>(this IList<T> list)
    {
        var n = list.Count;
        var random = ThreadRandom.Value;
        while (n > 1)
        {
            n--;
            var k = random.Next(n + 1);
            var value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}