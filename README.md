# Redmine .NET API Client

![Redmine .NET API](https://raw.githubusercontent.com/zapadi/redmine-net-api/master/logo.png)

A .NET client library for the [Redmine REST API](https://www.redmine.org/projects/redmine/wiki/Rest_api).

[![NuGet](https://img.shields.io/nuget/v/redmine-api.svg)](https://www.nuget.org/packages/redmine-api)
[![NuGet signed](https://img.shields.io/nuget/v/redmine-api-signed.svg?label=nuget%20signed)](https://www.nuget.org/packages/redmine-api-signed)
[![NuGet Downloads](https://img.shields.io/nuget/dt/redmine-api.svg)](https://www.nuget.org/packages/redmine-api)
[![Build](https://github.com/zapadi/redmine-net-api/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/zapadi/redmine-net-api/actions/workflows/build-and-test.yml)
[![License](https://img.shields.io/github/license/zapadi/redmine-net-api.svg)](LICENSE)

## Features

- Synchronous and asynchronous APIs (`Get`/`GetAsync`, `Create`/`CreateAsync`, ...), with `CancellationToken` support on the async methods
- XML (default) or JSON serialization
- Transparent handling of gzip/deflate compressed responses
- Paging: fetch a single page with `GetPagedAsync`, or let `GetAsync` walk through all pages
- User impersonation (`X-Redmine-Switch-User`)
- API key or HTTP basic authentication
- File upload and download

## Supported platforms

| Platform | Versions |
|----------|----------|
| .NET | 5, 6, 7, 8, 9 |
| .NET Framework | 4.5, 4.5.1, 4.5.2, 4.6, 4.6.1, 4.6.2, 4.7, 4.7.1, 4.7.2, 4.8, 4.8.1 |
| .NET Framework (deprecated) | 2.0, 4.0 |

Support for .NET Framework 2.0 and 4.0 is deprecated and will be removed in the next major version.

The integration tests run against Redmine 6.1.

## Installation

```bash
dotnet add package redmine-api
```

If your application requires strong-named assemblies, use the signed package instead:

```bash
dotnet add package redmine-api-signed
```

Both packages contain the same code; `redmine-api-signed` is built with a strong-name key.

## Quick start

Create a manager:

```csharp
using Redmine.Net.Api;
using Redmine.Net.Api.Types;

var options = new RedmineManagerOptionsBuilder()
    .WithHost("https://redmine.example.com")
    .WithApiKeyAuthentication("your-api-key");

var manager = new RedmineManager(options);
```

Get an issue by id:

```csharp
var issue = await manager.GetAsync<Issue>("123");

Console.WriteLine(issue.Subject);
```

List issues with a filter, one page at a time:

```csharp
using System.Collections.Specialized;
using Redmine.Net.Api.Net;

var requestOptions = new RequestOptions
{
    QueryString = new NameValueCollection
    {
        { RedmineKeys.PROJECT_ID, "my-project" },
        { RedmineKeys.STATUS_ID, "open" },
        { RedmineKeys.LIMIT, "25" },
        { RedmineKeys.OFFSET, "0" }
    }
};

var page = await manager.GetPagedAsync<Issue>(requestOptions);

Console.WriteLine($"{page.TotalItems} issues in total");

foreach (var item in page.Items)
{
    Console.WriteLine($"#{item.Id} {item.Subject}");
}
```

`GetAsync<Issue>(requestOptions)` returns all matching issues, requesting as many pages as needed.

Create an issue:

```csharp
var newIssue = new Issue
{
    Project = IdentifiableName.Create<IdentifiableName>(1),
    Tracker = IdentifiableName.Create<IdentifiableName>(1),
    Subject = "Login page returns 500",
    Description = "Steps to reproduce: ..."
};

var created = await manager.CreateAsync(newIssue);
```

Handle errors:

```csharp
using Redmine.Net.Api.Exceptions;

try
{
    await manager.GetAsync<Issue>("999999");
}
catch (RedmineApiException ex) when (ex.HttpStatusCode == 404)
{
    Console.WriteLine("Issue not found.");
}
catch (RedmineApiException ex)
{
    Console.WriteLine($"Request failed ({ex.HttpStatusCode}): {ex.Message}");
}
```

The synchronous API mirrors the async one, for example `manager.Get<Issue>("123")`; the synchronous equivalent of `GetPagedAsync` is `GetPaginated`.

## Configuration

### Authentication

```csharp
var apiKeyOptions = new RedmineManagerOptionsBuilder()
    .WithHost("https://redmine.example.com")
    .WithApiKeyAuthentication("your-api-key");

var basicAuthOptions = new RedmineManagerOptionsBuilder()
    .WithHost("https://redmine.example.com")
    .WithBasicAuthentication("login", "password");
```

### JSON or XML

XML is used by default. To use JSON:

```csharp
using Redmine.Net.Api.Serialization;

var options = new RedmineManagerOptionsBuilder()
    .WithHost("https://redmine.example.com")
    .WithApiKeyAuthentication("your-api-key")
    .WithSerializationType(SerializationType.Json);
```

### Page size

The page size used by list requests that do not set `limit` (default: 25):

```csharp
var options = new RedmineManagerOptionsBuilder()
    .WithHost("https://redmine.example.com")
    .WithApiKeyAuthentication("your-api-key")
    .WithPageSize(100);
```

### Impersonation

Send a request on behalf of another user (requires an administrator account):

```csharp
var issues = await manager.GetAsync<Issue>(new RequestOptions { ImpersonateUser = "jsmith" });
```

### Including associated data

```csharp
var issue = await manager.GetAsync<Issue>("123",
    RequestOptions.Include(RedmineKeys.JOURNALS, RedmineKeys.ATTACHMENTS));
```

### HttpClient transport

Requests are sent with `WebClient` by default. To send them with `HttpClient` (.NET Framework 4.5+ and .NET 5+),
install the `redmine-api.http` package (`redmine-api.http-signed` for strong-named assemblies):

```bash
dotnet add package redmine-api.http
```

```csharp
using Redmine.Net.Api.Http;

var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(100) };

var manager = new RedmineManager(new RedmineManagerOptionsBuilder()
    .WithHost("https://redmine.example.com")
    .WithApiKeyAuthentication("your-api-key")
    .WithHttpClient(httpClient));
```

The `HttpClient` belongs to you: the library never disposes it, so reuse a long-lived instance. Timeouts, proxy,
certificates, decompression and message handlers are configured on the `HttpClient` and its handler.
`HttpClient.Timeout` covers the whole request, including the reading of a download's body. On .NET 5+ the
synchronous API uses `HttpClient.Send`, which skips message handlers that override only `SendAsync` and needs a handler
that supports synchronous sends: `SocketsHttpHandler` (the default) does, while custom handlers or HTTP/2-only setups
may throw `NotSupportedException`, surfaced as a `RedmineApiException`. Prefer the async API in those cases.
`HttpClient.DefaultRequestHeaders` are sent only when the request does not set the same header, so authentication,
impersonation and `RequestOptions` headers always win. `WithHttpClient` cannot be combined with `WithWebClient` or
`WithWebClientOptions`.

### Custom transport

Any other HTTP stack can be plugged in by implementing `IRedmineTransport` and passing it to `WithTransport`. The request
already contains the absolute URI, the merged headers and the body; return every response, whatever its status code, and
throw only for transport failures. The XML documentation of `IRedmineTransport` describes the full contract.

## Supported resources

The generic methods (`Get`, `GetPaginated`, `Count`, `Create`, `Update`, `Delete` and their `Async` variants, including `GetPagedAsync`) cover most resources. Operations that need a dedicated endpoint are available as extension methods on `RedmineManager`; each has an `Async` counterpart, except `UpdateIssueAttachment`.

| Resource | List / Get | Create | Update | Delete | Notes / extra operations |
|----------|:----------:|:------:|:------:|:------:|--------------------------|
| [Attachments](https://www.redmine.org/projects/redmine/wiki/Rest_Attachments) | ✓ | | ✓ | ✓ | Get by id only. Attach files to an issue through `Issue.Uploads`. `UpdateIssueAttachment`, `DownloadFile` |
| [Custom fields](https://www.redmine.org/projects/redmine/wiki/Rest_CustomFields) | ✓ | | | | List only |
| [Enumerations](https://www.redmine.org/projects/redmine/wiki/Rest_Enumerations) | ✓ | | | | `IssuePriority`, `TimeEntryActivity`, `DocumentCategory`; list only |
| [Files](https://www.redmine.org/projects/redmine/wiki/Rest_Files) | ✓ | ✓ | | | List only, project scoped: `GetProjectFiles`, `Create(file, projectId)` |
| [Groups](https://www.redmine.org/projects/redmine/wiki/Rest_Groups) | ✓ | ✓ | ✓ | ✓ | `AddUserToGroup`, `RemoveUserFromGroup` |
| [Issues](https://www.redmine.org/projects/redmine/wiki/Rest_Issues) | ✓ | ✓ | ✓ | ✓ | `AddWatcherToIssue`, `RemoveWatcherFromIssue`; journals, relations, watchers, etc. via `RequestOptions.Include` |
| [Issue categories](https://www.redmine.org/projects/redmine/wiki/Rest_IssueCategories) | ✓ | ✓ | ✓ | ✓ | Project scoped: `GetProjectIssueCategories`, `Create(category, projectId)` |
| [Issue relations](https://www.redmine.org/projects/redmine/wiki/Rest_IssueRelations) | ✓ | ✓ | | ✓ | Issue scoped: list with `issue_id`, `Create(relation, issueId)` |
| [Issue statuses](https://www.redmine.org/projects/redmine/wiki/Rest_IssueStatuses) | ✓ | | | | List only |
| [My account](https://www.redmine.org/projects/redmine/wiki/Rest_MyAccount) | ✓ | | | | `GetMyAccount` |
| [News](https://www.redmine.org/projects/redmine/wiki/Rest_News) | ✓ | ✓ | ✓ | ✓ | `GetProjectNews`, `AddProjectNews` (create) |
| [Projects](https://www.redmine.org/projects/redmine/wiki/Rest_Projects) | ✓ | ✓ | ✓ | ✓ | `ArchiveProject`, `UnarchiveProject`, `CloseProject`, `ReopenProject`, `ProjectRepositoryAddRelatedIssue`, `ProjectRepositoryRemoveRelatedIssue` |
| [Project memberships](https://www.redmine.org/projects/redmine/wiki/Rest_Memberships) | ✓ | ✓ | ✓ | ✓ | Project scoped: `GetProjectMemberships`, `Create(membership, projectId)` |
| [Queries](https://www.redmine.org/projects/redmine/wiki/Rest_Queries) | ✓ | | | | List only |
| [Roles](https://www.redmine.org/projects/redmine/wiki/Rest_Roles) | ✓ | | | | |
| [Search](https://www.redmine.org/projects/redmine/wiki/Rest_Search) | ✓ | | | | `Search` with `SearchFilterBuilder` |
| [Time entries](https://www.redmine.org/projects/redmine/wiki/Rest_TimeEntries) | ✓ | ✓ | ✓ | ✓ | |
| [Trackers](https://www.redmine.org/projects/redmine/wiki/Rest_Trackers) | ✓ | | | | List only |
| [Uploads](https://www.redmine.org/projects/redmine/wiki/Rest_api#Attaching-files) | | ✓ | | | `UploadFile` returns a token to reference from issues, files, news or wiki pages |
| [Users](https://www.redmine.org/projects/redmine/wiki/Rest_Users) | ✓ | ✓ | ✓ | ✓ | `GetCurrentUser` |
| [Versions](https://www.redmine.org/projects/redmine/wiki/Rest_Versions) | ✓ | ✓ | ✓ | ✓ | Project scoped: list with `project_id`, `Create(version, projectId)` |
| [Wiki pages](https://www.redmine.org/projects/redmine/wiki/Rest_WikiPages) | ✓ | ✓ | ✓ | ✓ | Extension methods only: `GetAllWikiPages`, `GetWikiPage`, `CreateWikiPage`, `UpdateWikiPage`, `DeleteWikiPage` |

## Documentation

- [Wiki](https://github.com/zapadi/redmine-net-api/wiki): guides and usage examples
- [CHANGELOG.md](CHANGELOG.md): release history

## Building from source

Build the solution:

```bash
dotnet build redmine-net-api.slnx
```

Run the unit tests:

```bash
dotnet test --project tests/redmine-net-api.Tests/redmine-net-api.Tests.csproj -f net10.0
```

The integration tests (`tests/redmine-net-api.Integration.Tests`) start Redmine and PostgreSQL containers with [Testcontainers](https://dotnet.testcontainers.org/), so they need Docker. They target .NET Framework 4.8.1 and therefore run on Windows or with Mono.

## Contributing

Contributions are welcome. See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

<a href="https://github.com/zapadi/redmine-net-api/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=zapadi/redmine-net-api" alt="Contributors" />
</a>

## License

Licensed under the [Apache License 2.0](LICENSE).

## Support

If you find this project useful, you can support its development:

[![Buy Me a Coffee](https://cdn.buymeacoffee.com/buttons/lato-yellow.png)](https://www.buymeacoffee.com/vXCNnz9)
