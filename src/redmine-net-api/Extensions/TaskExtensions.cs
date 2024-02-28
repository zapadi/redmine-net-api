/*
Copyright 2011 - 2023 Adrian Popescu

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

#if !(NET20)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Redmine.Net.Api.Extensions;

internal static class TaskExtensions
{
    public static T GetAwaiterResult<T>(this Task<T> task)
    {
        return task.GetAwaiter().GetResult();
    }
        
    public static TResult Synchronize<TResult>(Func<Task<TResult>> function)
    {
        return Task.Factory.StartNew(function, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default)
                   .Unwrap().GetAwaiter().GetResult();
    }

    public static void Synchronize(Func<Task> function)
    {
        Task.Factory.StartNew(function, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default)
            .Unwrap().GetAwaiter().GetResult();
    }  
}
#endif