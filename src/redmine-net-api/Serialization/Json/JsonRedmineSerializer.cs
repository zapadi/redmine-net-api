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
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Redmine.Net.Api.Common;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Serialization.Json.Extensions;

namespace Redmine.Net.Api.Serialization.Json
{
    internal sealed class JsonRedmineSerializer : IRedmineSerializer
    {
        private static void EnsureJsonSerializable<T>()
        {
            if (!typeof(IJsonSerializable).IsAssignableFrom(typeof(T)))
            {
                throw new RedmineException($"Entity of type '{typeof(T)}' should implement IJsonSerializable.");
            }
        }

        public T Deserialize<T>(string jsonResponse) where T : new()
        {
            SerializationHelper.EnsureDeserializationInputIsNotNullOrWhiteSpace(jsonResponse, nameof(jsonResponse), typeof(T));

            EnsureJsonSerializable<T>();

            using var stringReader = new StringReader(jsonResponse);
            using var jsonReader = new JsonTextReader(stringReader);
            var obj = Activator.CreateInstance<T>();

            if (jsonReader.Read() && jsonReader.Read())
            {
                ((IJsonSerializable)obj).ReadJson(jsonReader);
            }

            return obj;
        }

        public PagedResults<T> DeserializeToPagedResults<T>(string jsonResponse) where T : class, new()
        {
            SerializationHelper.EnsureDeserializationInputIsNotNullOrWhiteSpace(jsonResponse, nameof(jsonResponse), typeof(T));

            using var sr = new StringReader(jsonResponse);
            using var reader = new JsonTextReader(sr);
            var total = 0;
            var offset = 0;
            var limit = 0;
            List<T> list = null;

            while (reader.Read())
            {
                if (reader.TokenType != JsonToken.PropertyName)
                {
                    continue;
                }

                switch (reader.Value)
                {
                    case RedmineKeys.TOTAL_COUNT:
                        total = reader.ReadAsInt32().GetValueOrDefault();
                        break;
                    case RedmineKeys.OFFSET:
                        offset = reader.ReadAsInt32().GetValueOrDefault();
                        break;
                    case RedmineKeys.LIMIT:
                        limit = reader.ReadAsInt32().GetValueOrDefault();
                        break;
                    default:
                        list = reader.ReadAsCollection<T>();
                        break;
                }
            }

            return new PagedResults<T>(list, total, offset, limit);
        }

        #pragma warning disable CA1822
        public int Count<T>(string jsonResponse) where T : class, new()
        {
            SerializationHelper.EnsureDeserializationInputIsNotNullOrWhiteSpace(jsonResponse, nameof(jsonResponse), typeof(T));

            using var sr = new StringReader(jsonResponse);
            using var reader = new JsonTextReader(sr);
            var total = 0;

            while (reader.Read())
            {
                if (reader.TokenType != JsonToken.PropertyName)
                {
                    continue;
                }

                if (reader.Value is RedmineKeys.TOTAL_COUNT)
                {
                    total = reader.ReadAsInt32().GetValueOrDefault();
                    return total;
                }
            }

            return total;
        }
        #pragma warning restore CA1822
        
        public string Format { get; } = RedmineConstants.JSON;
        
        public string ContentType { get; } = RedmineConstants.CONTENT_TYPE_APPLICATION_JSON;

        public string Serialize<T>(T entity) where T : class
        {
            if (entity == null)
            {
                throw new RedmineSerializationException($"Could not serialize null of type {typeof(T).Name}", nameof(entity));
            }
            
            EnsureJsonSerializable<T>();

            if (entity is not IJsonSerializable jsonSerializable)
            {
                throw new RedmineException($"Entity of type '{typeof(T)}' should implement IJsonSerializable.");
            }

            var stringBuilder = new StringBuilder();

            using var sw = new StringWriter(stringBuilder);
            using var writer = new JsonTextWriter(sw);
            //writer.Formatting = Formatting.Indented;
            writer.DateFormatHandling = DateFormatHandling.IsoDateFormat;

            jsonSerializable.WriteJson(writer);

            var json = stringBuilder.ToString();
                    
            stringBuilder.Length = 0;

            return json;
        }
    }
}