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

using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    public enum IssueRelationType
    {
        #pragma warning disable CS0618 // Use of internal enumeration value is allowed here to have a fallback
        /// <summary>
        /// Fallback value for deserialization purposes in case the deserialization fails. Do not use to create new relations!
        /// </summary>
        Undefined = 0,
        #pragma warning restore CS0618
        /// <summary>
        /// 
        /// </summary>
        Relates = 1,

        /// <summary>
        /// 
        /// </summary>
        Duplicates,

        /// <summary>
        /// 
        /// </summary>
        Duplicated,

        /// <summary>
        /// 
        /// </summary>
        Blocks,

        /// <summary>
        /// 
        /// </summary>
        Blocked,

        /// <summary>
        /// 
        /// </summary>
        Precedes,

        /// <summary>
        /// 
        /// </summary>
        Follows,

        /// <summary>
        /// 
        /// </summary>
      
        [XmlEnum(RedmineKeys.COPIED_TO)]
        CopiedTo,

        /// <summary>
        /// 
        /// </summary>
        [XmlEnum(RedmineKeys.COPIED_FROM)]
        CopiedFrom
    }
}