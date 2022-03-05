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
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

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
      
        [XmlEnum("copied_to")]
        CopiedTo,

        /// <summary>
        /// 
        /// </summary>
        [XmlEnum("copied_from")]
        CopiedFrom
    }

    // /// <inheritdoc />
    // public class IssueRelationTypeConverter : JsonConverter
    // {
    //     /// <inheritdoc />
    //     public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    //     {
    //         IssueRelationType messageTransportResponseStatus = (IssueRelationType) value;
    //
    //         switch (messageTransportResponseStatus)
    //         {
    //             case IssueRelationType.Undefined:
    //                 break;
    //             case IssueRelationType.Relates:
    //                 writer.WriteValue("relates");
    //                 break;
    //             case IssueRelationType.Duplicates:
    //                 writer.WriteValue("duplicates");
    //                 break;
    //             case IssueRelationType.Duplicated:
    //                 writer.WriteValue("duplicated");
    //                 break;
    //             case IssueRelationType.Blocks:
    //                 writer.WriteValue("blocks");
    //                 break;
    //             case IssueRelationType.Blocked:
    //                 writer.WriteValue("blocked");
    //                 break;
    //             case IssueRelationType.Precedes:
    //                 writer.WriteValue("precedes");
    //                 break;
    //             case IssueRelationType.Follows:
    //                 writer.WriteValue("follows");
    //                 break;
    //             case IssueRelationType.CopiedTo:
    //                 writer.WriteValue("copied_to");
    //                 break;
    //             case IssueRelationType.CopiedFrom:
    //                 writer.WriteValue("copied_from");
    //                 break;
    //             default:
    //                 throw new ArgumentOutOfRangeException();
    //         }
    //     }
    //
    //     /// <inheritdoc />
    //     public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    //     {
    //         var enumString = (string) reader.Value;
    //         switch (enumString)
    //         {
    //             case "relates":
    //             case "duplicates":
    //             case "duplicated":
    //             case "blocks":
    //             case "blocked":
    //             case "precedes":
    //             case "follows":
    //                 return Enum.Parse(typeof(IssueRelationType), enumString, true);
    //             case "copied_to":
    //                 return IssueRelationType.CopiedTo;
    //             case "copied_from":
    //                 return IssueRelationType.CopiedFrom;
    //             default:
    //                 throw new ArgumentOutOfRangeException();
    //         }
    //     }
    //
    //     /// <inheritdoc />
    //     public override bool CanConvert(Type objectType)
    //     {
    //         return objectType == typeof(string);
    //     }
    // }
}