using System;
using System.Globalization;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests;

public static class AssertDateTime
{
   

    public static void Equal(
        string expected,
        DateTime? actual,
        bool dateOnly = false,
        TimeSpan? within = null,
        string? format = null,
        CultureInfo? culture = null,
        DateTimeStyles styles = DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
        TimeZoneInfo? assumeZoneForUnspecified = null)
    {
        if (string.IsNullOrWhiteSpace(expected))
        {
            Assert.Null(actual);
            return;
        }

        if (actual is null)
            throw new Xunit.Sdk.XunitException($"Expected '{expected}', but actual was <null>.");

        Equal(expected, actual.Value, dateOnly: dateOnly, within: within,
              format: format, culture: culture ?? CultureInfo.InvariantCulture, styles: styles,
              assumeZoneForUnspecified: assumeZoneForUnspecified);
    }
    public static void Equal(
        string expected,
        DateTime actual,
        bool dateOnly = false,
        TimeSpan? within = null,
        string? format = null,
        CultureInfo? culture = null,
        DateTimeStyles styles = DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
        TimeZoneInfo? assumeZoneForUnspecified = null)
    {
        culture ??= CultureInfo.InvariantCulture;

        if (!TryParseWithZone(expected, format, culture, styles, assumeZoneForUnspecified, out var exp))
            throw new Xunit.Sdk.XunitException($"""Could not parse expected date: "{expected}" (format: {format ?? "<any>"}).""");

        var act = NormalizeToUtc(actual, styles, assumeZoneForUnspecified);

        if (dateOnly)
        {
            var expDate = assumeZoneForUnspecified is null
                ? exp.Date
                : TimeZoneInfo.ConvertTimeFromUtc(exp, assumeZoneForUnspecified).Date;

            var actDate = assumeZoneForUnspecified is null
                ? act.Date
                : TimeZoneInfo.ConvertTimeFromUtc(act, assumeZoneForUnspecified).Date;

            Assert.Equal(expDate, actDate);
            return;
        }

        if (within is { } tol)
        {
            var delta = (act - exp).Duration();
            Assert.True(delta <= tol, $"Expected {exp:o} ± {tol}, but was {act:o} (Δ={delta}).");
        }
        else
        {
            Assert.Equal(exp, act);
        }
    }

    private static bool TryParseWithZone(
        string input,
        string? format,
        CultureInfo culture,
        DateTimeStyles styles,
        TimeZoneInfo? zone,
        out DateTime utc)
    {
        if (!string.IsNullOrWhiteSpace(format))
        {
            if (DateTimeOffset.TryParseExact(input, format, culture, styles, out var dtoFmt))
            {
                utc = dtoFmt.UtcDateTime;
                return true;
            }

            if (DateTime.TryParseExact(input, format, culture, styles, out var dtFmt))
            {
                utc = NormalizeToUtc(dtFmt, styles, zone);
                return true;
            }

            utc = default;
            return false;
        }

        if (DateTimeOffset.TryParse(input, culture, styles, out var dto))
        {
            utc = dto.UtcDateTime;
            return true;
        }

        if (DateTime.TryParse(input, culture, styles, out var dt))
        {
            utc = NormalizeToUtc(dt, styles, zone);
            return true;
        }

        utc = default;
        return false;
    }

    private static DateTime NormalizeToUtc(DateTime dt, DateTimeStyles styles, TimeZoneInfo? zone)
    {
        switch (dt.Kind)
        {
            case DateTimeKind.Utc:
                return dt;
            case DateTimeKind.Local:
                return dt.ToUniversalTime();
            case DateTimeKind.Unspecified:
                if (zone is not null)
                    return TimeZoneInfo.ConvertTimeToUtc(dt, zone);
                if (styles.HasFlag(DateTimeStyles.AssumeUniversal))
                    return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                if (styles.HasFlag(DateTimeStyles.AssumeLocal))
                    return DateTime.SpecifyKind(dt, DateTimeKind.Local).ToUniversalTime();
                // default: assume local if truly unspecified
                return DateTime.SpecifyKind(dt, DateTimeKind.Local).ToUniversalTime();
            default:
                return dt;
        }
    }
}