/*
Copyright 2011 - 2025 Adrian Popescu

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

#if (NET40_OR_GREATER || NET)
using System;
using System.Collections.Generic;
using System.Linq;
#if !NET40
using System.Runtime.CompilerServices;
#endif
using System.Threading;
using System.Threading.Tasks;

namespace Padi.RedmineApi.Internals;

internal static class TaskExtensions
{
#if !NET40
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public static T GetAwaiterResult<T>(this Task<T> task)
    {
        return task.GetAwaiter().GetResult();
    }
    
    public static TResult Synchronize<TResult>(Func<Task<TResult>> function)
    {
        return Task.Factory.StartNew(function, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default)
                   .Unwrap()
                   .GetAwaiter()
                   .GetResult();
    }

    public static void Synchronize(Func<Task> function)
    {
        Task.Factory.StartNew(function, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default)
            .Unwrap()
            .GetAwaiter()
            .GetResult();
    }  
    
    #if (NET40)
    public static Task<TResult[]> WhenAll<TResult>(IEnumerable<Task<TResult>> tasks, CancellationToken cancellationToken = default)
    {
        var clone = tasks.ToArray();
      //TODO: it must be improved by using TaskCompletionSource
        var task = Task.Factory.StartNew(() =>
        {
            Task.WaitAll(clone, cancellationToken);
            return clone.Select(t => t.Result).ToArray();
        }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);

        return task; 
    }
    #endif
}
#endif