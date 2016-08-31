/*
   Copyright 2011 - 2016 Adrian Popescu.

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Threading.Tasks;

namespace Redmine.Net.Api.Extensions
{

    /// <summary>
    /// 
    /// </summary>
    public static class TaskExtensions
    {

        /// <summary>
        /// Maps the asynchronous.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="this">The this.</param>
        /// <param name="fn">The function.</param>
        /// <returns></returns>
        public static async Task<TResult> MapAsync<TSource, TResult>(this Task<TSource> @this, Func<TSource, Task<TResult>> fn)
        {
            return await fn(await @this);
        }

        /// <summary>
        /// Maps the asynchronous.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="this">The this.</param>
        /// <param name="fn">The function.</param>
        /// <returns></returns>
        public static async Task<TResult> MapAsync<TSource, TResult>(this TSource @this, Func<TSource, Task<TResult>> fn)
        {
            return await fn(@this);
        }

        /// <summary>
        /// Maps the asynchronous.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="this">The this.</param>
        /// <param name="fn">The function.</param>
        /// <returns></returns>
        public static async Task<TResult> MapAsync<TSource, TResult>(this Task<TSource> @this, Func<TSource, TResult> fn)
        {
            return fn(await @this);
        }

        /// <summary>
        /// Tees the asynchronous.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="this">The this.</param>
        /// <param name="act">The act.</param>
        /// <returns></returns>
        public static async Task<TSource> TeeAsync<TSource, TResult>(this TSource @this, Func<TSource, Task<TResult>> act)
        {
            await act(@this);
            return @this;
        }

        /// <summary>
        /// Tees the asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this">The this.</param>
        /// <param name="act">The act.</param>
        /// <returns></returns>
        public static async Task<T> TeeAsync<T>(this Task<T> @this, Action<T> act)
        {
            act(await @this);
            return await @this;
        }

        /// <summary>
        /// Tees the asynchronous.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="this">The this.</param>
        /// <param name="act">The act.</param>
        /// <returns></returns>
        public static async Task<TSource> TeeAsync<TSource, TResult>(this Task<TSource> @this, Func<TSource, Task<TResult>> act)
        {
            await act(await @this);
            return await @this;
        }

        /// <summary>
        /// Tees the asynchronous.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="this">The this.</param>
        /// <param name="act">The act.</param>
        /// <returns></returns>
        public static async Task<TSource> TeeAsync<TSource>(this TSource @this, Func<TSource, Task<TSource>> act)
        {
            await act(@this);
            return @this;
        }

        /// <summary>
        /// Tees the specified act.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="this">The this.</param>
        /// <param name="act">The act.</param>
        /// <returns></returns>
        public static async Task<TSource> Tee<TSource, TResult>(this Task<TSource> @this, Func<TSource, TResult> act)
        {
            act(await @this);
            return await @this;
        }
    }
}