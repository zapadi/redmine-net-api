/*
   Copyright 2011 - 2017 Adrian Popescu.

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
using System.Web.Script.Serialization;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.JSonConverters
{
    internal class CustomFieldPossibleValueConverter : IdentifiableNameConverter
    {
        #region Overrides of JavaScriptConverter

        /// <summary>
        /// Deserializes the specified dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="type">The type.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns></returns>
        public override object Deserialize(IDictionary<string, object> dictionary, Type type,
            JavaScriptSerializer serializer)
        {
            if (dictionary != null)
            {
                var entity = new CustomFieldPossibleValue();

                entity.Value = dictionary.GetValue<string>(RedmineKeys.VALUE);

                return entity;
            }

            return null;
        }

        /// <summary>
        /// Serializes the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="serializer">The serializer.</param>
        /// <returns></returns>
        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            return null;
        }

        /// <summary>
        /// Gets the supported types.
        /// </summary>
        /// <value>
        /// The supported types.
        /// </value>
        public override IEnumerable<Type> SupportedTypes
        {
            get { return new List<Type>(new[] {typeof(CustomFieldPossibleValue)}); }
        }

        #endregion
    }
}