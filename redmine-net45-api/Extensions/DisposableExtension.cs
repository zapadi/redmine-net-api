using System;

namespace Redmine.Net.Api.Extensions
{
    public static class DisposableExtension
    {
        public static TResult Using<TResource, TResult>(Func<TResource> resourceFactory, Func<TResource, TResult> fn)
            where TResource : IDisposable
        {
            using (var resource = resourceFactory()) return fn(resource);
        }
    }
}