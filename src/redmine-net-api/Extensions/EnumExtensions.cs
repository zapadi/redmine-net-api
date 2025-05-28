using System;
using Redmine.Net.Api.Authentication;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.Extensions;

/// <summary>
/// Provides extension methods for enumerations used in the Redmine.Net.Api.Types namespace.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Converts the specified enumeration value to its lowercase invariant string representation.
    /// </summary>
    /// <param name="enum">The enumeration value to be converted.</param>
    /// <returns>A string representation of the IssueRelationType enumeration value in a lowercase, or "undefined" if the value does not match a defined case.</returns>
    public static string ToLowerName(this IssueRelationType @enum)
    {
        return @enum switch
        {
            IssueRelationType.Relates => "relates",
            IssueRelationType.Duplicates => "duplicates",
            IssueRelationType.Duplicated => "duplicated",
            IssueRelationType.Blocks => "blocks",
            IssueRelationType.Blocked => "blocked",
            IssueRelationType.Precedes => "precedes",
            IssueRelationType.Follows => "follows",
            IssueRelationType.CopiedTo => "copied_to",
            IssueRelationType.CopiedFrom => "copied_from",
            _ => "undefined"
        };
    }

    /// <summary>
    /// Converts the specified VersionSharing enumeration value to its lowercase invariant string representation.
    /// </summary>
    /// <param name="enum">The VersionSharing enumeration value to be converted.</param>
    /// <returns>A string representation of the VersionSharing enumeration value in a lowercase, or "undefined" if the value does not match a valid case.</returns>
    public static string ToLowerName(this VersionSharing @enum)
    {
        return @enum switch
        {
            VersionSharing.Unknown => "unknown",
            VersionSharing.None => "none",
            VersionSharing.Descendants => "descendants",
            VersionSharing.Hierarchy => "hierarchy",
            VersionSharing.Tree => "tree",
            VersionSharing.System => "system",
            _ => "undefined"
        };
    }

    /// <summary>
    /// Converts the specified <see cref="VersionStatus"/> enumeration value to its lowercase invariant string representation.
    /// </summary>
    /// <param name="enum">The <see cref="VersionStatus"/> enumeration value to be converted.</param>
    /// <returns>A lowercase string representation of the enumeration value, or "undefined" if the value does not match a defined case.</returns>
    public static string ToLowerName(this VersionStatus @enum)
    {
        return @enum switch
        {
            VersionStatus.None => "none",
            VersionStatus.Open => "open",
            VersionStatus.Closed => "closed",
            VersionStatus.Locked => "locked",
            _ => "undefined"
        };
    }

    /// <summary>
    /// Converts the specified ProjectStatus enumeration value to its lowercase invariant string representation.
    /// </summary>
    /// <param name="enum">The ProjectStatus enumeration value to be converted.</param>
    /// <returns>A string representation of the ProjectStatus enumeration value in a lowercase, or "undefined" if the value does not match a defined case.</returns>
    public static string ToLowerName(this ProjectStatus @enum)
    {
        return @enum switch
        {
            ProjectStatus.None => "none",
            ProjectStatus.Active => "active",
            ProjectStatus.Archived => "archived",
            ProjectStatus.Closed => "closed",
            _ => "undefined"
        };
    }

    /// <summary>
    /// Converts the specified enumeration value to its lowercase invariant string representation.
    /// </summary>
    /// <param name="enum">The enumeration value to be converted.</param>
    /// <returns>A string representation of the UserStatus enumeration value in a lowercase, or "undefined" if the value does not match a defined case.</returns>
    public static string ToLowerName(this UserStatus @enum)
    {
        return @enum switch
        {
            UserStatus.StatusActive => "status_active",
            UserStatus.StatusLocked => "status_locked",
            UserStatus.StatusRegistered => "status_registered",
            _ => "undefined"
        };
    }

    internal static string ToText(this RedmineAuthenticationType @enum)
    {
        return @enum switch
        {
            RedmineAuthenticationType.NoAuthentication => "NoAuth",
            RedmineAuthenticationType.Basic => "Basic",
            RedmineAuthenticationType.ApiKey => "ApiKey",
            _ => "undefined"
        };
    }
}