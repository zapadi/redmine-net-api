using System;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;

namespace Redmine.Net.Api.Internals;

internal static class HostHelper
{
    private static readonly char[] DotCharArray = ['.'];

    internal static void EnsureDomainNameIsValid(string domainName)
    {
        if (domainName.IsNullOrWhiteSpace())
        {
            throw new RedmineException("Domain name cannot be null or empty.");
        }

        if (domainName.Length > 255)
        {
            throw new RedmineException("Domain name cannot be longer than 255 characters.");
        }

        var labels = domainName.Split(DotCharArray);
        if (labels.Length == 1)
        {
            throw new RedmineException("Domain name is not valid.");
        }

        foreach (var label in labels)
        {
            if (label.IsNullOrWhiteSpace() || label.Length > 63)
            {
                throw new RedmineException("Domain name must be between 1 and 63 characters.");
            }

            if (!char.IsLetterOrDigit(label[0]) || !char.IsLetterOrDigit(label[label.Length - 1]))
            {
                throw new RedmineException("Domain name label starts or ends with a hyphen or invalid character.");
            }

            for (var index = 0; index < label.Length; index++)
            {
                var ch = label[index];

                if (!char.IsLetterOrDigit(ch) && ch != '-')
                {
                    throw new RedmineException("Domain name contains an invalid character.");
                }

                if (ch == '-' && index + 1 < label.Length && label[index + 1] == '-')
                {
                    throw new RedmineException("Domain name contains consecutive hyphens.");
                }
            }
        }
    }

    internal static Uri CreateRedmineUri(string host, string scheme = null)
    {
        if (host.IsNullOrWhiteSpace())
        {
            throw new RedmineException("The host is null or empty.");
        }

        if (!Uri.TryCreate(host, UriKind.Absolute, out var uri))
        {
            host = host.TrimEnd('/', '\\');
            EnsureDomainNameIsValid(host);

            if (!host.StartsWith(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) ||
                !host.StartsWith(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
            {
                host = $"{scheme ?? Uri.UriSchemeHttps}://{host}";

                if (!Uri.TryCreate(host, UriKind.Absolute, out uri))
                {
                    throw new RedmineException("The host is not valid.");
                }
            }
        }

        if (!uri.IsWellFormedOriginalString())
        {
            throw new RedmineException("The host is not well-formed.");
        }

        scheme ??= Uri.UriSchemeHttps;
        var hasScheme = false;
        if (!uri.Scheme.IsNullOrWhiteSpace())
        {
            if (uri.Host.IsNullOrWhiteSpace() && uri.IsAbsoluteUri && !uri.IsFile)
            {
                if (uri.Scheme.Equals("localhost", StringComparison.OrdinalIgnoreCase))
                {
                    int port = 0;
                    var portAsString = uri.AbsolutePath.RemoveTrailingSlash();
                    if (!portAsString.IsNullOrWhiteSpace())
                    {
                        int.TryParse(portAsString, out port);
                    }

                    var ub = new UriBuilder(scheme, "localhost", port);
                    return ub.Uri;
                }
            }
            else
            {
                if (!IsSchemaHttpOrHttps(uri.Scheme))
                {
                    throw new RedmineException("Invalid host scheme. Only HTTP and HTTPS are supported.");
                }

                hasScheme = true;
            }
        }
        else
        {
            if (!IsSchemaHttpOrHttps(scheme))
            {
                throw new RedmineException("Invalid host scheme. Only HTTP and HTTPS are supported.");
            }
        }

        var uriBuilder = new UriBuilder();

        if (uri.HostNameType == UriHostNameType.IPv6)
        {
            uriBuilder.Scheme = (hasScheme ? uri.Scheme : scheme ?? Uri.UriSchemeHttps);
            uriBuilder.Host = uri.Host;
        }
        else
        {
            if (uri.Authority.IsNullOrWhiteSpace())
            {
                if (uri.Port == -1)
                {
                    if (int.TryParse(uri.LocalPath, out var port))
                    {
                        uriBuilder.Port = port;
                    }
                }

                uriBuilder.Scheme = scheme ?? Uri.UriSchemeHttps;
                uriBuilder.Host = uri.Scheme;
            }
            else
            {
                uriBuilder.Scheme = uri.Scheme;
                uriBuilder.Port = int.TryParse(uri.LocalPath, out var port) ? port : uri.Port;
                uriBuilder.Host = uri.Host;
                if (!uri.LocalPath.IsNullOrWhiteSpace() && !uri.LocalPath.Contains("."))
                {
                    uriBuilder.Path = uri.LocalPath;
                }
            }
        }

        try
        {
            return uriBuilder.Uri;
        }
        catch (Exception ex)
        {
            throw new RedmineException($"Failed to create Redmine URI: {ex.Message}", ex);
        }
    }

    private static bool IsSchemaHttpOrHttps(string scheme)
    {
        return scheme == Uri.UriSchemeHttp || scheme == Uri.UriSchemeHttps;
    }
}