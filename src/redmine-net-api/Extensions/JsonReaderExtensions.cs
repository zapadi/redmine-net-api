/*
   Copyright 2011 - 2022 Adrian Popescu

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
using Newtonsoft.Json;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Serialization;

namespace Redmine.Net.Api.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class JsonExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static int ReadAsInt(this JsonReader reader)
        {
            return reader.ReadAsInt32().GetValueOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static bool ReadAsBool(this JsonReader reader)
        {
            return reader.ReadAsBoolean().GetValueOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="readInnerArray"></param>
        /// <returns></returns>
        public static List<T> ReadAsCollection<T>(this JsonReader reader, bool readInnerArray = false) where T : class
        {
            var isJsonSerializable = typeof(IJsonSerializable).IsAssignableFrom(typeof(T));

            if (!isJsonSerializable)
            {
                throw new RedmineException($"Entity of type '{typeof(T)}' should implement IJsonSerializable.");
            }

            var col = new List<T>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndArray)
                {
                    break;
                }

                if (readInnerArray)
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        break;
                    }
                }

                if (reader.TokenType == JsonToken.StartArray)
                {
                    continue;
                }

                var entity = Activator.CreateInstance<T>();

                ((IJsonSerializable)entity).ReadJson(reader);

                var des = entity;

                col.Add(des);
            }

            return col;
        }
    }
}