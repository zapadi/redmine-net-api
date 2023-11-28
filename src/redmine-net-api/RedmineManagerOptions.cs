using System;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Serialization;

namespace Redmine.Net.Api
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class RedmineManagerOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public Uri BaseAddress { get; init; }
        
        /// <summary>
        /// Gets or sets the page size for paginated Redmine API responses.
        /// The default page size is 25, but you can customize it as needed. 
        /// </summary>
        public int PageSize { get; init; }

        /// <summary>
        /// Gets or sets the desired MIME format for Redmine API responses, which represents the way of serialization.
        /// Supported formats include XML and JSON. The default format is XML.
        /// </summary>
        public IRedmineSerializer Serializer { get; init; }
        
        /// <summary>
        /// Gets or sets the authentication method to be used when connecting to the Redmine server.
        /// The available authentication types include API token-based authentication and basic authentication
        /// (using a username and password). You can set an instance of the corresponding authentication class
        /// to use the desired authentication method.
        /// </summary>
        public IRedmineAuthentication Authentication { get; init; }

        /// <summary>
        /// Gets or sets a custom function that creates and returns a specialized instance of the WebClient class.
        /// </summary>
        public Func<IRedmineApiClient> ClientFunc { get; init; }
        
        /// <summary>
        /// Gets or sets the settings for configuring the Redmine web client.
        /// </summary>
        public IRedmineApiClientOptions ClientOptions { get; init; }
        
        /// <summary>
        /// Gets or sets the version of the Redmine server to which this client will connect.
        /// </summary>
        public Version RedmineVersion { get; init; }
        
        internal bool VerifyServerCert { get; init; }
    }
}