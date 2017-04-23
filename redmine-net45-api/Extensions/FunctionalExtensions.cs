/*
   Copyright 2011 - 2017 Adrian Popescu.

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

namespace Redmine.Net.Api.Extensions
{
    /// <summary>
    /// 
    /// </summary>
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

        /// <summary>
        /// Maps the specified function.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="this">The this.</param>
        /// <param name="fn">The function.</param>
        /// <returns></returns>
        public static TResult Map<TSource, TResult>(this TSource @this, Func<TSource, TResult> fn)
        {
            return fn(@this);
        }

        /// <summary>
        /// Curries the specified function.
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="func">The function.</param>
        /// <returns></returns>
        public static Func<A, Func<B, C>> Curry<A, B, C>(this Func<A, B, C> func)
        {
            return a => b => func(a, b);
        }

        /// <summary>
        /// Curries the specified function.
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <typeparam name="D"></typeparam>
        /// <param name="func">The function.</param>
        /// <returns></returns>
        public static Func<A, Func<B, Func<C, D>>> Curry<A, B, C, D>(this Func<A, B, C, D> func)
        {
            return a => b => c => func(a, b, c);
        }

        /// <summary>
        /// Caches the specified function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">The function.</param>
        /// <param name="interval">The interval.</param>
        /// <returns></returns>
        public static Func<T> Cache<T>(Func<T> func, int interval)
        {
            var cachedValue = func();
            var timeCached= DateTime.Now;

            Func<T> cachedFunc = () =>
            {
                if((DateTime.Now - timeCached).Seconds >= interval)
                {
                    timeCached = DateTime.Now;
                    cachedValue = func();
                }

                return cachedValue;
            };

            return cachedFunc;
        }
    }
}