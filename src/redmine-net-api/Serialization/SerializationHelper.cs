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
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Serialization.Json;
using Redmine.Net.Api.Serialization.Xml;

namespace Redmine.Net.Api.Serialization
{
    /// <summary>
    /// Provides helper methods for serializing user-related data for communication with the Redmine API.
    /// </summary>
    internal static class SerializationHelper
    {
        /// <summary>
        /// Serializes the user ID into a format suitable for communication with the Redmine API,
        /// based on the specified serializer type.
        /// </summary>
        /// <param name="userId">The ID of the user to be serialized.</param>
        /// <param name="redmineSerializer">The serializer used to format the user ID (e.g., XML or JSON).</param>
        /// <returns>A serialized representation of the user ID.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the provided serializer is not recognized or supported.
        /// </exception>
        public static string SerializeUserId(int userId, IRedmineSerializer redmineSerializer)
        {
            return redmineSerializer switch
            {
                XmlRedmineSerializer => $"<user_id>{userId.ToInvariantString()}</user_id>",
                JsonRedmineSerializer => $"{{\"user_id\":\"{userId.ToInvariantString()}\"}}",
                _ => throw new ArgumentOutOfRangeException(nameof(redmineSerializer), redmineSerializer, null)
            };
        }
        
        public static void EnsureDeserializationInputIsNotNullOrWhiteSpace(string input, string paramName, Type type)
        {
            if (input.IsNullOrWhiteSpace())
            {
                throw new RedmineSerializationException($"Could not deserialize null or empty input for type '{type.Name}'.", paramName);
            }
        }
    }
}