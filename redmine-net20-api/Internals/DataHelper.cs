/*
   Copyright 2011 - 2016 Adrian Popescu.

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

namespace Redmine.Net.Api.Internals
{
    /// <summary>
    /// 
    /// </summary>
    internal static class DataHelper
    {
        /// <summary>
        /// Users the data.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="mimeFormat">The MIME format.</param>
        /// <returns></returns>
        public static string UserData(int userId, MimeFormat mimeFormat)
        {
            return mimeFormat == MimeFormat.Xml
                ? "<user_id>" + userId + "</user_id>"
                : "{\"user_id\":\"" + userId + "\"}";
        }
    }
}