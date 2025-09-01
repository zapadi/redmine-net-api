#if NET20
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
namespace System;

/// <summary>Defines a provider for progress updates.</summary>
/// <typeparam name="T">The type of progress update value.</typeparam>
public interface IProgress<in T>
{
  /// <summary>Reports a progress update.</summary>
  /// <param name="value">The value of the updated progress.</param>
  void Report(T value);
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class Progress<T> : IProgress<T>
{
    private readonly Action<T> _handler;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handler"></param>
    public Progress(Action<T> handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void Report(T value)
    {
        _handler(value);
    }
}
#endif