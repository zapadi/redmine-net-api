using System.Text;

namespace Padi.DotNet.RedmineAPI.Integration.Tests;

internal static class RandomHelper
{
    /// <summary>
    /// Generates a cryptographically strong, random string suffix.
    /// This method is thread-safe as Guid.NewGuid() is thread-safe.
    /// </summary>
    /// <returns>A random string, 32 characters long, consisting of hexadecimal characters, without hyphens.</returns>
    private static string GenerateSuffix()
    {
        return Guid.NewGuid().ToString("N");
    }

    private static readonly char[] EnglishAlphabetChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"
        .ToCharArray();

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
    private static string GenerateRandomString(int length = 10)
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

    internal static void FillRandomBytes(byte[] bytes)
    {
        ThreadRandom.Value.NextBytes(bytes);
    }

    internal static int GetRandomNumber(int max)
    {
        return ThreadRandom.Value.Next(max);
    }

    internal static int GetRandomNumber(int min, int max)
    {
        return ThreadRandom.Value.Next(min, max);
    }

    /// <summary>
    /// Generates a random alphabetic suffix, defaulting to 10 characters.
    /// This method is thread-safe.
    /// </summary>
    /// <param name="length">The desired length of the suffix. Defaults to 10.</param>
    /// <returns>A random alphabetic string.</returns>
    public static string GenerateText(int length = 10)
    {
        return GenerateRandomString(length);
    }

    /// <summary>
    /// Generates a random name by combining a specified prefix and a random alphabetic suffix.
    /// This method is thread-safe.
    /// Example: if the prefix is "MyItem", the result could be "MyItem_aBcDeFgHiJ".
    /// </summary>
    /// <param name="prefix">The prefix for the name. A '_' separator will be added.</param>
    /// <param name="suffixLength">The desired length of the random suffix. Defaults to 10.</param>
    /// <returns>A string combining the prefix, an underscore, and a random alphabetic suffix.
    /// If the prefix is null or empty, it returns just the random suffix.</returns>
    public static string GenerateText(string prefix = null, int suffixLength = 10)
    {
        var suffix = GenerateRandomString(suffixLength);
        return string.IsNullOrEmpty(prefix) ? suffix : $"{prefix}_{suffix}";
    }
    
    /// <summary>
    /// Generates a random email address with alphabetic characters only.
    /// </summary>
    /// <param name="localPartLength">Length of the local part (before @). Defaults to 8.</param>
    /// <param name="domainLength">Length of the domain name (without extension). Defaults to 6.</param>
    /// <returns>A random email address with only alphabetic characters.</returns>
    public static string GenerateEmail(int localPartLength = 8, int domainLength = 6)
    {
        if (localPartLength <= 0 || domainLength <= 0)
        {
            throw new ArgumentOutOfRangeException(
                localPartLength <= 0 ? nameof(localPartLength) : nameof(domainLength),
                "Length must be a positive integer.");
        }

        var localPart = GenerateRandomString(localPartLength);
        var domain = GenerateRandomString(domainLength).ToLower();

        // Use common TLDs
        var tlds = new[] { "com", "org", "net", "io" };
        var tld = tlds[ThreadRandom.Value.Next(tlds.Length)];

        return $"{localPart}@{domain}.{tld}";
    }

    /// <summary>
    /// Generates a random webpage URL with alphabetic characters only.
    /// </summary>
    /// <param name="domainLength">Length of the domain name (without extension). Defaults to 8.</param>
    /// <param name="pathLength">Length of the path segment. Defaults to 10.</param>
    /// <returns>A random webpage URL with only alphabetic characters.</returns>
    public static string GenerateWebpage(int domainLength = 8, int pathLength = 10)
    {
        if (domainLength <= 0 || pathLength <= 0)
        {
            throw new ArgumentOutOfRangeException(
                domainLength <= 0 ? nameof(domainLength) : nameof(pathLength),
                "Length must be a positive integer.");
        }

        var domain = GenerateRandomString(domainLength).ToLower();

        // Use common TLDs
        var tlds = new[] { "com", "org", "net", "io" };
        var tld = tlds[ThreadRandom.Value.Next(tlds.Length)];

        // Generate path segments
        var segments = ThreadRandom.Value.Next(0, 3);
        var path = "";

        if (segments > 0)
        {
            var pathSegments = new List<string>(segments);
            for (int i = 0; i < segments; i++)
            {
                pathSegments.Add(GenerateRandomString(ThreadRandom.Value.Next(3, pathLength)).ToLower());
            }

            path = "/" + string.Join("/", pathSegments);
        }

        return $"https://www.{domain}.{tld}{path}";
    }

    /// <summary>
    /// Generates a random name composed only of alphabetic characters from the English alphabet.
    /// </summary>
    /// <param name="length">Length of the name. Defaults to 6.</param>
    /// <param name="capitalize">Whether to capitalize the first letter. Defaults to true.</param>
    /// <returns>A random name with only English alphabetic characters.</returns>
    public static string GenerateName(int length = 6, bool capitalize = true)
    {
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length), "Length must be a positive integer.");
        }

        // Generate random name
        var name = GenerateRandomString(length);

        if (capitalize)
        {
            name = char.ToUpper(name[0]) + name.Substring(1).ToLower();
        }
        else
        {
            name = name.ToLower();
        }

        return name;
    }

    /// <summary>
    /// Generates a random full name composed only of alphabetic characters.
    /// </summary>
    /// <param name="firstNameLength">Length of the first name. Defaults to 6.</param>
    /// <param name="lastNameLength">Length of the last name. Defaults to 8.</param>
    /// <returns>A random full name with only alphabetic characters.</returns>
    public static string GenerateFullName(int firstNameLength = 6, int lastNameLength = 8)
    {
        if (firstNameLength <= 0 || lastNameLength <= 0)
        {
            throw new ArgumentOutOfRangeException(
                firstNameLength <= 0 ? nameof(firstNameLength) : nameof(lastNameLength),
                "Length must be a positive integer.");
        }

        // Generate random first and last names using the new alphabetic-only method
        var firstName = GenerateName(firstNameLength);
        var lastName = GenerateName(lastNameLength);

        return $"{firstName} {lastName}";
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