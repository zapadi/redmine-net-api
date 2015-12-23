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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Redmine.Net.Api.Types;
using System.Linq;
using System.Threading.Tasks;

namespace Redmine.Net.Api
{

	public static class TaskExtensions{

	    public static async Task<TResult> MapAsync<TSource, TResult>(this Task<TSource> @this,
	        Func<TSource, Task<TResult>> fn)
	    {
	       return await fn(await @this);
	    }

	    public static async Task<TResult> MapAsync<TSource, TResult>(this TSource @this, Func<TSource, Task<TResult>> fn)
	    {
	        return await fn(@this);
	    }

	    public static async Task<TResult> MapAsync<TSource, TResult>(this Task<TSource> @this, Func<TSource, TResult> fn)
	    {
	        return fn(await @this);
	    }


	}

	public static class FunctionalExtnsions{

		/// <summary>
		/// The Tee extension method takes it’s name from the corresponding UNIX command which is used in command pipelines to cause a side-effect with a given input and return the original value. 
		/// </summary>
		/// <param name="this">This.</param>
		/// <param name="action">Action.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T Tee<T>(this T @this, Action<T> action){
			action (@this);
			return @this;
		}

	    public static TResult Map<TSource, TResult>(this TSource @this, Func<TSource, TResult> fn)
	    {
	        return fn(@this);
	    }
	}

	public static class DisposableHelper{
		public static TResult Using<TResource, TResult>(Func<TResource> resourceFactory, Func<TResource, TResult> fn)
			where TResource : IDisposable
		{
			using (var resource = resourceFactory()) return fn(resource);
		}

//		public static async Task<TResult> Using<TResource, TResult>(Task<Func<TResource>> resourceFactory, Func<TResource, TResult> fn)
//			where TResource : IDisposable
//		{
//			using (var resource = resourceFactory()) return fn(await resource);
//		}
	}
}