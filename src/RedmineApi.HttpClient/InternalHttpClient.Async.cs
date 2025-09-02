using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Padi.RedmineApi.Authentication;
using Padi.RedmineApi.Exceptions;
using Padi.RedmineApi.Extensions;
using Padi.RedmineApi.HttpClient.Extensions;
using Padi.RedmineApi.Internals;
using Padi.RedmineApi.Net;

namespace Padi.RedmineApi.HttpClient;

internal sealed partial class InternalHttpClient
{
    protected override async Task<ApiResponseMessage> HandleRequestAsync(string address, string verb, RequestOptions? requestOptions = null, object? content = null,
        IProgress<int>? progress = null, CancellationToken cancellationToken = default)
    {
        var requestMessage = CreateRequestMessage(address, verb, requestOptions, content as ApiRequestContent);
        return await SendAsync(requestMessage, progress, cancellationToken).ConfigureAwait(false);
    }

    private async Task<ApiResponseMessage> SendAsync(ApiRequestMessage requestMessage, IProgress<int>? progress = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_httpClient == null)
            throw new InvalidOperationException("HttpClient is not initialized.");

        using var request = CreateHttpRequestMessage(requestMessage);

        try
        {
            using var response = await _httpClient
                .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false);

            return await ProcessResponseAsync(request, response, requestMessage, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            return CreateErrorResponse(requestMessage.RequestUri, HttpStatusCode.InternalServerError, ex, request);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return CreateErrorResponse(requestMessage.RequestUri, HttpStatusCode.InternalServerError, ex, request);
        }
    }

    private HttpRequestMessage CreateHttpRequestMessage(ApiRequestMessage requestMessage)
    {
        var request = new HttpRequestMessage(
            new HttpMethod(StringExtensions.IsNullOrWhiteSpace(requestMessage.Method) ? "GET" : requestMessage.Method),
            BuildRequestUri(requestMessage));

        SetRequestHeaders(request, requestMessage);
        SetRequestContent(request, requestMessage);

        return request;
    }

    private static string BuildRequestUri(ApiRequestMessage requestMessage)
    {
        if (requestMessage.QueryString?.Count > 0)
        {
            return AppendQueryString(requestMessage.RequestUri, requestMessage.QueryString);
        }
        return requestMessage.RequestUri;
    }

    private static string AppendQueryString(string uri, System.Collections.Specialized.NameValueCollection queryString)
    {
        var hasQuery = uri.Contains('?');
        var sb = new StringBuilder(uri);

        for (var i = 0; i < queryString.Count; i++)
        {
            var value = queryString[i];
            if (value == null) continue;

            var key = queryString.Keys[i];
            sb.Append(hasQuery ? '&' : '?');
            sb.Append(Uri.EscapeDataString(key));
            sb.Append('=');
            sb.Append(Uri.EscapeDataString(value));
            hasQuery = true;
        }

        return sb.ToString();
    }

    private void SetRequestHeaders(HttpRequestMessage request, ApiRequestMessage requestMessage)
    {
        AddHeaderIfNotEmpty(request, RedmineConstants.REQUEST_ID_HEADER_KEY, requestMessage.RequestId);
        AddHeaderIfNotEmpty(request, RedmineConstants.CONTENT_TYPE_HEADER_KEY, requestMessage.ContentType);
        AddHeaderIfNotEmpty(request, RedmineConstants.IMPERSONATE_HEADER_KEY, requestMessage.ImpersonateUser);
        AddHeaderIfNotEmpty(request, RedmineConstants.USER_AGENT_HEADER_KEY, requestMessage.UserAgent);

        SetAuthenticationHeader(request);
        AddCustomHeaders(request, requestMessage.Headers);
    }

    private static void AddHeaderIfNotEmpty(HttpRequestMessage request, string key, string value)
    {
        if (!value.IsNullOrWhiteSpace())
        {
            request.Headers.TryAddWithoutValidation(key, value);
        }
    }

    private void SetAuthenticationHeader(HttpRequestMessage request)
    {
        switch (_credentials)
        {
            case RedmineBasicAuthentication:
                request.Headers.Authorization = new AuthenticationHeaderValue(RedmineConstants.BASIC_AUTHORIZATION_HEADER_KEY, _credentials.Token);
                break;
            case RedmineApiKeyAuthentication:
                request.Headers.TryAddWithoutValidation((string)RedmineConstants.API_KEY_AUTHORIZATION_HEADER_KEY, (string)_credentials.Token);
                break;
        }
    }

    private static void AddCustomHeaders(HttpRequestMessage request, IDictionary<string, string>? headers)
    {
        if (!(headers?.Count > 0))
        {
            return;
        }

        foreach (var header in headers)
        {
            request.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
    }

    private static void SetRequestContent(HttpRequestMessage request, ApiRequestMessage requestMessage)
    {
        if (requestMessage.Content == null)
        {
            return;
        }

        request.Content = new ByteArrayContent(requestMessage.Content.Body);
        request.Content.Headers.ContentType = new MediaTypeHeaderValue(requestMessage.Content.ContentType);
    }

    private async Task<ApiResponseMessage> ProcessResponseAsync(HttpRequestMessage request, HttpResponseMessage response,
        ApiRequestMessage requestMessage, CancellationToken cancellationToken)
    {
        var headers = ConvertHeadersToDictionary(response.Headers, response.Content?.Headers);

        if (!response.IsSuccessStatusCode)
        {
            return await HandleErrorResponseAsync(request, response, requestMessage.RequestUri, headers, cancellationToken)
                .ConfigureAwait(false);
        }

        return await HandleSuccessResponseAsync(response, requestMessage, headers)
            .ConfigureAwait(false);
    }

    private async Task<ApiResponseMessage> HandleErrorResponseAsync(HttpRequestMessage request, HttpResponseMessage response,
        string endpoint, IDictionary<string, string>? headers, CancellationToken cancellationToken)
    {
        if (response.StatusCode == (HttpStatusCode)HttpConstants.StatusCodes.UnprocessableEntity)
        {
            var content = await TryGetStringAsync(response, cancellationToken).ConfigureAwait(false);
            return CreateApiResponse(endpoint, (int)response.StatusCode, headers, content, isSuccess: false);
        }

        var exception = await CreateExceptionFromResponseAsync(request, response, endpoint, cancellationToken)
            .ConfigureAwait(false);

        return CreateApiResponse(endpoint, (int)response.StatusCode, headers, exception: exception, isSuccess: false);
    }

    private async Task<ApiResponseMessage> HandleSuccessResponseAsync(HttpResponseMessage response,
        ApiRequestMessage requestMessage, IDictionary<string, string>? headers)
    {
        return requestMessage.Method switch
        {
            HttpConstants.HttpVerbs.GET => await HandleGetResponseAsync(response, requestMessage.RequestUri, headers)
                .ConfigureAwait(false),
            HttpConstants.HttpVerbs.POST => await HandlePostResponseAsync(response, requestMessage.RequestUri, headers)
                .ConfigureAwait(false),
            HttpConstants.HttpVerbs.PUT => await HandlePutResponseAsync(response, requestMessage.RequestUri, headers)
                .ConfigureAwait(false),
            HttpConstants.HttpVerbs.DELETE => await HandleDeleteResponseAsync(response, requestMessage.RequestUri, headers)
                .ConfigureAwait(false),
            HttpConstants.HttpVerbs.DOWNLOAD => await HandleDownloadResponseAsync(response, requestMessage.RequestUri, headers)
                .ConfigureAwait(false),
            _ => throw new RedmineApiException(RedmineApiErrorCode.UnsupportedHttpMethod, "Unsupported HTTP method", null)
        };

        static async Task<ApiResponseMessage> HandleGetResponseAsync(HttpResponseMessage response, string endpoint, IDictionary<string, string>? headers)
        {
            var content = await TryGetStringAsync(response).ConfigureAwait(false);
            return CreateApiResponse(endpoint, (int)response.StatusCode, headers, content, isSuccess: true);
        }

        static async Task<ApiResponseMessage> HandlePostResponseAsync(HttpResponseMessage response, string endpoint, IDictionary<string, string>? headers)
        {
            var content = await TryGetStringAsync(response).ConfigureAwait(false);
            return CreateApiResponse(endpoint, (int)response.StatusCode, headers, content, isSuccess: true);
        }

        static async Task<ApiResponseMessage> HandlePutResponseAsync(HttpResponseMessage response, string endpoint, IDictionary<string, string>? headers)
        {
            var content = await TryGetStringAsync(response).ConfigureAwait(false);
            return CreateApiResponse(endpoint, (int)response.StatusCode, headers, content, isSuccess: true);
        }

        static async Task<ApiResponseMessage> HandleDeleteResponseAsync(HttpResponseMessage response, string endpoint, IDictionary<string, string>? headers)
        {
            var content = await TryGetStringAsync(response).ConfigureAwait(false);
            return CreateApiResponse(endpoint, (int)response.StatusCode, headers, content, isSuccess: true);
        }

        static async Task<ApiResponseMessage> HandleDownloadResponseAsync(HttpResponseMessage response, string endpoint, IDictionary<string, string>? headers)
        {
            var rawContent = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            return CreateApiResponse(endpoint, (int)response.StatusCode, headers, rawContent: rawContent,
                contentLength: rawContent.LongLength, isSuccess: true);
        }
    }

    private static async Task<RedmineException> CreateExceptionFromResponseAsync(HttpRequestMessage request, HttpResponseMessage response,
        string endpoint, CancellationToken cancellationToken)
    {
        return response.StatusCode switch
        {
            HttpStatusCode.Unauthorized => new RedmineApiException(RedmineApiErrorCode.Unauthorized, 
                response.ReasonPhrase, 
                null, 
                method: request.Method.Method,
                httpStatusCode:  (int)response.StatusCode,
                endpoint: request.RequestUri?.ToString() ?? endpoint,
                responseHeaders: ConvertHeadersToDictionary(response.Headers, null)),
            HttpStatusCode.NotFound => new RedmineApiException(RedmineApiErrorCode.NotFound, 
                response.ReasonPhrase, 
                null, 
                method: request.Method.Method,
                httpStatusCode:  (int)response.StatusCode,
                endpoint: request.RequestUri?.ToString() ?? endpoint,
                responseHeaders: ConvertHeadersToDictionary(response.Headers, null)),
            HttpStatusCode.Forbidden => await CreateForbiddenExceptionAsync(request, response, endpoint, cancellationToken)
                .ConfigureAwait(false),
            HttpStatusCode.Conflict => new RedmineApiException(RedmineApiErrorCode.Conflict, 
                response.ReasonPhrase, 
                null, 
                method: request.Method.Method,
                httpStatusCode:  (int)response.StatusCode,
                endpoint: request.RequestUri?.ToString() ?? endpoint,
                responseHeaders: ConvertHeadersToDictionary(response.Headers, null)),
            HttpStatusCode.GatewayTimeout or 
            HttpStatusCode.RequestTimeout or
            HttpStatusCode.BadGateway or 
            HttpStatusCode.ServiceUnavailable => await CreateServiceUnavailableExceptionAsync(request, response)
                .ConfigureAwait(false),
            _ => new RedmineApiException(RedmineApiErrorCode.Unknown, response.ReasonPhrase, 
                null,
                method: request.Method.Method,
                httpStatusCode:  (int)response.StatusCode,
                endpoint: request.RequestUri?.ToString() ?? endpoint,
                responseHeaders: ConvertHeadersToDictionary(response.Headers, null))
        };
    }

    private static async Task<RedmineApiException> CreateForbiddenExceptionAsync(HttpRequestMessage request, HttpResponseMessage response, string endpoint, CancellationToken cancellationToken)
    {
        var msg = await TryGetStringAsync(response, cancellationToken: cancellationToken).ConfigureAwait(false);
        var message = new StringBuilder("Forbidden access to ")
            .Append("Method: ")
            .Append(request.Method)
            .Append(", Request: ")
            .AppendLine(request.RequestUri?.ToString() ?? endpoint)
            .Append(msg)
            .ToString();

        return new RedmineApiException(RedmineApiErrorCode.Forbidden, message,null,
            method: request.Method.Method,
            httpStatusCode:  (int)response.StatusCode,
            endpoint: request.RequestUri?.ToString() ?? endpoint,
            responseHeaders: ConvertHeadersToDictionary(response.Headers, null));
    }

    private static async Task<RedmineApiException> CreateServiceUnavailableExceptionAsync(HttpRequestMessage request, HttpResponseMessage response)
    {
        var msg = await TryGetStringAsync(response).ConfigureAwait(false);
        var message = $"An exception {response.StatusCode.ToInvariantString()} occurred while contacting {request.RequestUri}.{Environment.NewLine}{msg}.";
        return new RedmineApiException(RedmineApiErrorCode.ServiceUnavailable, message,null,
            method: request.Method.Method,
            isTransient: true,
            httpStatusCode:  (int)response.StatusCode,
            endpoint: request.RequestUri?.ToString(),
            responseHeaders: ConvertHeadersToDictionary(response.Headers, null));
    }

    private static async Task<string> TryGetStringAsync(HttpResponseMessage response, CancellationToken cancellationToken = default)
    {
        try
        {
            return await response.Content.ReadAsStringAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            return $"Could not read the response content: {e.Message}";
        }
    }

    private static IDictionary<string, string>? ConvertHeadersToDictionary(HttpHeaders? headers, HttpContentHeaders? contentHeaders)
    {
        if (headers == null && contentHeaders == null)
        {
            return null;
        }

        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (headers != null)
        {
            foreach (var h in headers)
            {
                result[h.Key] = string.Join(", ", h.Value);
            }
        }

        return result;
    }

    private static ApiResponseMessage CreateApiResponse(string endpoint, int statusCode,
        IDictionary<string, string>? headers = null, string? content = null,
        byte[]? rawContent = null, long contentLength = 0, Exception? exception = null, bool isSuccess = false)
    {
        return new ApiResponseMessage
        {
            EndPoint = endpoint,
            StatusCode = statusCode,
            Content = content,
            RawContent = rawContent,
            IsSuccessful = isSuccess,
            Exception = exception,
            //Headers = headers
        };
    }

    private static ApiResponseMessage CreateErrorResponse(string endpoint, HttpStatusCode statusCode, Exception exception, HttpRequestMessage? request)
    {
        var headers = ConvertHeadersToDictionary(request?.Headers, request?.Content?.Headers);
        return CreateApiResponse(endpoint, (int)statusCode, headers, exception: exception, isSuccess: false);
    }
}
