#if NET45_OR_GREATER || NETCOREAPP
using System.Net.Http;

namespace Redmine.Net.Api.Http
{
    internal sealed class NonDisposableHttpClientHandler 
        : HttpClientHandler
    {
        /// <summary>
        /// Private constructor to prevent direct instantiation of the class.
        /// </summary>
        private NonDisposableHttpClientHandler()
        {
        }

        /// <summary>
        /// Gets the singleton instance of <see cref="NonDisposableHttpClientHandler"/>.
        /// </summary>
        public static NonDisposableHttpClientHandler Instance { get; } = new NonDisposableHttpClientHandler();
        
        #pragma warning disable CA2215 // Dispose methods should call base class dispose
        /// <summary>
        /// Disposes the underlying resources held by the <see cref="NonDisposableHttpClientHandler"/>.
        /// This implementation does nothing to prevent unintended disposal, as it may affect all references.
        /// </summary>
        /// <param name="disposing">True if called from <see cref="Dispose"/>, false if called from a finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            // Do nothing if called explicitly from Dispose, as it may unintentionally affect all references.
            // The base.Dispose(disposing) is not called to avoid invoking the disposal of HttpClientHandler resources.
            // This implementation assumes that the HttpClientHandler is being used as a singleton and should not be disposed directly.
        }
        #pragma warning restore CA2215 // Dispose methods should call base class dispose
    }
}
#endif