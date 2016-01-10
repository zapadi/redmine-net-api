using System;

namespace Redmine.Net.Api.Extensions
{
    public static class FunctionalExtensions
    {

        /// <summary>
        /// The Tee extension method takes it’s name from the corresponding UNIX command which is used in command pipelines to cause a side-effect with a given input and return the original value. 
        /// </summary>
        /// <param name="this">This.</param>
        /// <param name="action">Action.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T Tee<T>(this T @this, Action<T> action)
        {
            action(@this);
            return @this;
        }

        public static TResult Map<TSource, TResult>(this TSource @this, Func<TSource, TResult> fn)
        {
            return fn(@this);
        }
    }
}