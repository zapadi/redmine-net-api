using System;

namespace Redmine.Net.Api.Internals;

/// <summary>
/// 
/// </summary>
internal static class ParameterValidator
{
    public static void ValidateNotNull<T>(T parameter, string parameterName)
        where T : class
    {
        if (parameter is null)
        {
            throw new ArgumentNullException(parameterName);
        }
    }

    public static void ValidateNotNullOrEmpty(string parameter, string parameterName)
    {
        if (string.IsNullOrEmpty(parameter))
        {
            throw new ArgumentException("Value cannot be null or empty", parameterName);
        }
    }

    public static void ValidateId(int id, string parameterName)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Id must be greater than 0", parameterName);
        }
    }
}