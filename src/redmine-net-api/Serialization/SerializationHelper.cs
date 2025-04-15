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

using System.Globalization;

namespace Redmine.Net.Api.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    internal static class SerializationHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="redmineSerializer"></param>
        /// <returns></returns>
        public static string SerializeUserId(int userId, IRedmineSerializer redmineSerializer)
        {
            return redmineSerializer is XmlRedmineSerializer
                ? $"<user_id>{userId.ToString(CultureInfo.InvariantCulture)}</user_id>"
                : $"{{\"user_id\":\"{userId.ToString(CultureInfo.InvariantCulture)}\"}}";
        }
    }
}