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
using Redmine.Net.Api.Serialization.Json;
using Redmine.Net.Api.Serialization.Xml;

namespace Redmine.Net.Api.Serialization;

/// <summary>
/// Factory for creating RedmineSerializer instances
/// </summary>
internal static class RedmineSerializerFactory
{
    /// <summary>
    /// Creates an instance of an IRedmineSerializer based on the specified serialization type.
    /// </summary>
    /// <param name="type">The type of serialization, either Xml or Json.</param>
    /// <returns>
    /// An instance of a serializer that implements the IRedmineSerializer interface.
    /// </returns>
    /// <exception cref="NotImplementedException">
    /// Thrown when the specified serialization type is not supported.
    /// </exception>
    public static IRedmineSerializer CreateSerializer(SerializationType type)
    {
        return type switch
        {
            SerializationType.Xml => new XmlRedmineSerializer(),
            SerializationType.Json => new JsonRedmineSerializer(),
            _ => throw new NotImplementedException($"No serializer has been implemented for the serialization type: {type}")
        };
    }
}