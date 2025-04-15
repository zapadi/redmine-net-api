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

#if !(NET20)
using System.Threading;
using System.Threading.Tasks;

namespace Redmine.Net.Api.Extensions;
#if !(NET45_OR_GREATER || NETCOREAPP)
internal static class SemaphoreSlimExtensions
{
    
    public static Task WaitAsync(this SemaphoreSlim semaphore, CancellationToken cancellationToken = default)
    {
        return Task.Factory.StartNew(() => semaphore.Wait(cancellationToken)
            , CancellationToken.None
            , TaskCreationOptions.None
            , TaskScheduler.Default);
    }
}
#endif
#endif
