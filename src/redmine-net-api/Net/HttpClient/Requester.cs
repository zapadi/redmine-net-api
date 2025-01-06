namespace Redmine.Net.Api.Net.HttpClient;

/// <summary>
/// 
/// </summary>
public partial class OperationExecutor
{
    // public Operation Send(IOperation<OperationIdResult> operation, SessionInfo sessionInfo = null)
    // {
    //     return AsyncHelpers.RunSync(() => SendAsync(operation, sessionInfo));
    // }
    //
    // public async Task<Operation> SendAsync(IOperation<OperationIdResult> operation, SessionInfo sessionInfo = null, CancellationToken token = default(CancellationToken))
    // {
    //     using (GetContext(out JsonOperationContext context))
    //     {
    //         var command = operation.GetCommand(_store, _requestExecutor.Conventions, context, _requestExecutor.Cache);
    //
    //         await _requestExecutor.ExecuteAsync(command, context, sessionInfo, token).ConfigureAwait(false);
    //         var node = command.SelectedNodeTag ?? command.Result.OperationNodeTag;
    //         return new Operation(_requestExecutor, () => _store.Changes(_databaseName, node), _requestExecutor.Conventions, command.Result.OperationId, node);
    //     }
    // }
}


/// <summary>
/// 
/// </summary>
public class Requester
{
    /*
    internal HttpRequestMessage CreateRequest<TResult>(JsonOperationContext ctx, ServerNode node, RavenCommand<TResult> command, out string url)
    {
        var request = command.CreateRequest(ctx, node, out url);
        if (request == null)
            return null;

        var builder = new UriBuilder(url);

        if (command is IRaftCommand raftCommand)
        {
            Debug.Assert(raftCommand.RaftUniqueRequestId != null, $"Forget to create an id for {command.GetType()}?");

            var raftRequestString = "raft-request-id=" + raftCommand.RaftUniqueRequestId;
            builder.Query = builder.Query?.Length > 1 ? $"{builder.Query.Substring(1)}&{raftRequestString}" : raftRequestString;
        }

        if (Conventions.HttpVersion != null)
            request.Version = Conventions.HttpVersion;

        request.RequestUri = builder.Uri;

        return request;
    }

    private async Task<bool> HandleUnsuccessfulResponse<TResult>(ServerNode chosenNode, int? nodeIndex, JsonOperationContext context, RavenCommand<TResult> command,
            HttpRequestMessage request, HttpResponseMessage response, string url, SessionInfo sessionInfo, bool shouldRetry, CancellationToken token = default)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    Cache.SetNotFound(url, AggressiveCaching.Value != null);
                    if (command.ResponseType == RavenCommandResponseType.Empty)
                        return true;
                    else if (command.ResponseType == RavenCommandResponseType.Object)
                        command.SetResponse(context, null, fromCache: false);
                    else
                        command.SetResponseRaw(response, null, context);
                    return true;

                case HttpStatusCode.Forbidden:
                    var msg = await TryGetResponseOfError(response).ConfigureAwait(false);
                    var builder = new StringBuilder("Forbidden access to ").
                        Append(chosenNode.Database).Append("@").Append(chosenNode.Url).Append(", ");
                    if (Certificate == null)
                    {
                        builder.Append("a certificate is required. ");
                    }
                    else if (Certificate.PrivateKey != null)
                    {
                        builder.Append(Certificate.FriendlyName).Append(" does not have permission to access it or is unknown. ");
                    }
                    else
                    {
                        builder.Append("The certificate ").Append(Certificate.FriendlyName)
                            .Append(" contains no private key. Constructing the certificate with the 'X509KeyStorageFlags.MachineKeySet' flag may solve this problem. ");
                    }
                    builder.Append("Method: ").Append(request.Method).Append(", Request: ").AppendLine(request.RequestUri.ToString()).Append(msg);
                    throw new AuthorizationException(builder.ToString());
                case HttpStatusCode.Gone: // request not relevant for the chosen node - the database has been moved to a different one
                    
                     return true;
                case HttpStatusCode.GatewayTimeout:
                case HttpStatusCode.RequestTimeout:
                case HttpStatusCode.BadGateway:
                case HttpStatusCode.ServiceUnavailable:
                    return await HandleServerDown(url, chosenNode, nodeIndex, context, command, request, response, null, sessionInfo, shouldRetry, requestContext: null, token: token).ConfigureAwait(false);

                case HttpStatusCode.Conflict:
                    await HandleConflict(context, response).ConfigureAwait(false);
                    break;

                default:
                    command.OnResponseFailure(response);
                    await ExceptionDispatcher.Throw(context, response, AdditionalErrorInformation).ConfigureAwait(false);
                    break;
            }
            return false;
        }
    
     private static async Task<string> TryGetResponseOfError(HttpResponseMessage response)
        {
            try
            {
                return (await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            }
            catch (Exception e)
            {
                return $"Could not read request: {e.Message}";
            }
        }

        private static Task HandleConflict(JsonOperationContext context, HttpResponseMessage response)
        {
            return ExceptionDispatcher.Throw(context, response);
        }

        public static async Task<Stream> ReadAsStreamUncompressedAsync(HttpResponseMessage response)
        {
            var serverStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var stream = serverStream;
            var encoding = response.Content.Headers.ContentEncoding.FirstOrDefault();
            if (encoding != null && encoding.Contains("gzip"))
                return new GZipStream(stream, CompressionMode.Decompress);
            if (encoding != null && encoding.Contains("deflate"))
                return new DeflateStream(stream, CompressionMode.Decompress);

            return serverStream;
        }

        private async Task<bool> HandleServerDown<TResult>(string url, ServerNode chosenNode, int? nodeIndex, JsonOperationContext context, RavenCommand<TResult> command,
            HttpRequestMessage request, HttpResponseMessage response, Exception e, SessionInfo sessionInfo, bool shouldRetry, RequestContext requestContext = null, CancellationToken token = default)
        {
            if (command.FailedNodes == null)
                command.FailedNodes = new Dictionary<ServerNode, Exception>();

            command.FailedNodes[chosenNode] = await ReadExceptionFromServer(context, request, response, e).ConfigureAwait(false);

            return true;
        }
        */
}