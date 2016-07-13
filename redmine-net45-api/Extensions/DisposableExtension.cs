using System;

namespace Redmine.Net.Api.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class DisposableExtension
    {
        /// <summary>
        /// Usings the specified resource factory.
        /// </summary>
        /// <typeparam name="TResource">The type of the resource.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="resourceFactory">The resource factory.</param>
        /// <param name="fn">The function.</param>
        /// <returns></returns>
        public static TResult Using<TResource, TResult>(Func<TResource> resourceFactory, Func<TResource, TResult> fn)
            where TResource : IDisposable
        {
            using (var resource = resourceFactory()) return fn(resource);
        }
    }
}