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

using System;
using Newtonsoft.Json;
using Padi.RedmineApi.Extensions;

namespace Padi.RedmineApi.Serialization.Json;

/// <summary>
/// 
/// </summary>
public sealed class JsonObject : IDisposable
{
    private readonly bool _hasRoot;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="root"></param>
    public JsonObject(JsonWriter writer, string? root = null)
    {
        Writer = writer;
        Writer.WriteStartObject();

        if (root.IsNullOrWhiteSpace())
        {
            return;
        }
            
        _hasRoot = true;
        Writer.WritePropertyName(root);
        Writer.WriteStartObject();
    }

    private JsonWriter Writer { get; }

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        Writer.WriteEndObject();
        if (_hasRoot)
        {
            Writer.WriteEndObject();
        }
    }
}