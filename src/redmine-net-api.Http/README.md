# redmine-api.http

An `HttpClient` transport for [redmine-api](https://www.nuget.org/packages/redmine-api), the .NET client for the
[Redmine REST API](https://www.redmine.org/projects/redmine/wiki/Rest_api).

`redmine-api` sends its requests with `WebClient` by default. This package sends them with an `HttpClient` you provide
instead. Choose it when you want to:

- reuse an `HttpClient` from `IHttpClientFactory` and dependency injection
- add message handlers such as retries, logging or telemetry
- configure the proxy, certificates and timeouts on the `HttpClient` and its handler

## Installation

```bash
dotnet add package redmine-api.http
```

If your application requires strong-named assemblies, use the signed package instead:

```bash
dotnet add package redmine-api.http-signed
```

The package depends on `redmine-api` (`redmine-api-signed` for the signed package) and supports .NET 5 to 10 and
.NET Framework 4.5 to 4.8.1.

## Quick start

```csharp
using System;
using System.Net.Http;
using Redmine.Net.Api;
using Redmine.Net.Api.Http;
using Redmine.Net.Api.Types;

var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(100) };

var manager = new RedmineManager(new RedmineManagerOptionsBuilder()
    .WithHost("https://redmine.example.com")
    .WithApiKeyAuthentication("your-api-key")
    .WithHttpClient(httpClient));

var issue = await manager.GetAsync<Issue>("123");

var sameIssue = manager.Get<Issue>("123");
```

## One transport per manager

Installing the package changes nothing on its own: requests keep going through `WebClient` until `WithHttpClient` is
called. `WithHttpClient` then replaces `WebClient` for that manager.

`WithHttpClient` cannot be combined with `WithWebClient` or `WithWebClientOptions`. Doing so throws a `RedmineException`
("WithTransport cannot be combined with WithWebClient or WithWebClientOptions.") when the options are built, that is
when the `RedmineManager` is created. `WebClientOptions`, `VerifyServerCert` and the `ServicePointManager` settings do
not apply to the `HttpClient`: configure the proxy, certificates and decompression on its handler.

## HttpClient ownership and lifetime

The `HttpClient` belongs to you: the library never disposes it, so reuse a long-lived instance instead of creating one
per request. Timeouts, proxy, certificates, decompression and message handlers are configured on the `HttpClient` and
its handler. The request URI is absolute, so `HttpClient.BaseAddress` is not used; set the host with `WithHost`.

### IHttpClientFactory and dependency injection

An `HttpClient` created by `IHttpClientFactory` can be passed to `WithHttpClient`; the library does not dispose it, so
the factory keeps managing its handlers. `RedmineManager` is cheap to build, so create one per scope or per use:

```csharp
services.AddHttpClient("redmine", client => client.Timeout = TimeSpan.FromSeconds(100));

services.AddScoped(sp => new RedmineManager(new RedmineManagerOptionsBuilder()
    .WithHost("https://redmine.example.com")
    .WithApiKeyAuthentication("your-api-key")
    .WithHttpClient(sp.GetRequiredService<IHttpClientFactory>().CreateClient("redmine"))));
```

## Timeouts

`HttpClient.Timeout` covers the whole request, including the reading of a download's body. When it expires, the call
fails with a `RedmineApiException` whose `IsTransient` is `true`.

## Retries and resilience

The library does not retry. Add retries to the `HttpClient`, for example with
[Microsoft.Extensions.Http.Resilience](https://www.nuget.org/packages/Microsoft.Extensions.Http.Resilience):

```csharp
services.AddHttpClient("redmine")
    .AddStandardResilienceHandler(options => options.Retry.DisableForUnsafeHttpMethods());
```

The standard handler also applies its own timeouts (10 seconds per attempt and 30 seconds in total by default); raise
`options.AttemptTimeout.Timeout` and `options.TotalRequestTimeout.Timeout` for slow requests. The total timeout must not
be below the attempt timeout and `options.CircuitBreaker.SamplingDuration` must be at least twice the attempt timeout.
Keep `HttpClient.Timeout` above `TotalRequestTimeout` so the handler's timeouts apply first. When a handler timeout
expires (or its circuit is open), the call fails with a `RedmineApiException` whose `IsTransient` is `false` and whose
inner exception is Polly's `TimeoutRejectedException` (or `BrokenCircuitException`), because the handler has already
retried.

To retry at the `RedmineManager` level instead, use `RedmineApiException.IsTransient` (timeouts, connection failures
and HTTP 408, 429, 502, 503 and 504) and the `Retry-After` value in `ResponseHeaders`, for example with
[Polly](https://www.nuget.org/packages/Polly):

```csharp
var retry = new ResiliencePipelineBuilder()
    .AddRetry(new RetryStrategyOptions
    {
        ShouldHandle = new PredicateBuilder().Handle<RedmineApiException>(ex => ex.IsTransient),
        MaxRetryAttempts = 3,
        BackoffType = DelayBackoffType.Exponential,
        UseJitter = true,
        DelayGenerator = args => new ValueTask<TimeSpan?>(
            args.Outcome.Exception is RedmineApiException { ResponseHeaders: { } headers }
            && headers.TryGetValue("Retry-After", out var value)
            && int.TryParse(value, out var seconds)
                ? TimeSpan.FromSeconds(seconds)
                : null)
    })
    .Build();

var issue = await retry.ExecuteAsync(async token => await manager.GetAsync<Issue>("1", cancellationToken: token));
```

Retrying a request that creates something (`Create`, `UploadFile`, ...) can create it twice when the first attempt
reached the server but its response was lost. When retrying at the `RedmineManager` level, retry only reads, updates
and deletes, or check before retrying a create.

## Synchronous API

On .NET 5 and later, the synchronous methods (`Get`, `Create`, ...) use `HttpClient.Send`. It skips message handlers
that override only `SendAsync` and needs a handler that supports synchronous sends: `SocketsHttpHandler` (the default)
does, while custom handlers or HTTP/2-only setups may throw `NotSupportedException`, surfaced as a
`RedmineApiException`. Prefer the async API in those cases.

On .NET Framework, the synchronous methods run the asynchronous send on the thread pool and wait for it.

## Headers

`HttpClient.DefaultRequestHeaders` are sent only when the request does not set the same header, so authentication,
impersonation and `RequestOptions` headers always win.

## Errors

HTTP errors and transport failures surface as a `RedmineApiException` with:

- `HttpStatusCode`: the response status code, or `null` when no response was received
- `IsTransient`: `true` for timeouts, connection failures (except host name resolution failures) and HTTP 408, 429, 502,
  503 and 504
- `Method` and `Endpoint`: the HTTP method and the request URL
- `ResponseHeaders`: the response headers, such as `Retry-After`

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
    Console.WriteLine($"{ex.Method} {ex.Endpoint} failed ({ex.HttpStatusCode}): {ex.Message}");
}
```

## Links

- [redmine-api documentation](https://github.com/zapadi/redmine-net-api#readme)
- [Wiki](https://github.com/zapadi/redmine-net-api/wiki)
- [Changelog](https://github.com/zapadi/redmine-net-api/blob/master/CHANGELOG.md)
- [License (Apache 2.0)](https://github.com/zapadi/redmine-net-api/blob/master/LICENSE)
