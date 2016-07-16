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

using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.JSonConverters
{
    internal class UserConverter : JavaScriptConverter
    {
        #region Overrides of JavaScriptConverter

        /// <summary>
        ///     When overridden in a derived class, converts the provided dictionary into an object of the specified type.
        /// </summary>
        /// <param name="dictionary">
        ///     An <see cref="T:System.Collections.Generic.IDictionary`2" /> instance of property data stored
        ///     as name/value pairs.
        /// </param>
        /// <param name="type">The type of the resulting object.</param>
        /// <param name="serializer">The <see cref="T:System.Web.Script.Serialization.JavaScriptSerializer" /> instance.</param>
        /// <returns>
        ///     The deserialized object.
        /// </returns>
        public override object Deserialize(IDictionary<string, object> dictionary, Type type,
            JavaScriptSerializer serializer)
        {
            if (dictionary != null)
            {
                var user = new User();
                user.Login = dictionary.GetValue<string>(RedmineKeys.LOGIN);
                user.Id = dictionary.GetValue<int>(RedmineKeys.ID);
                user.FirstName = dictionary.GetValue<string>(RedmineKeys.FIRSTNAME);
                user.LastName = dictionary.GetValue<string>(RedmineKeys.LASTNAME);
                user.Email = dictionary.GetValue<string>(RedmineKeys.MAIL);
                user.AuthenticationModeId = dictionary.GetValue<int?>(RedmineKeys.AUTH_SOURCE_ID);
                user.CreatedOn = dictionary.GetValue<DateTime?>(RedmineKeys.CREATED_ON);
                user.LastLoginOn = dictionary.GetValue<DateTime?>(RedmineKeys.LAST_LOGIN_ON);
                user.ApiKey = dictionary.GetValue<string>(RedmineKeys.API_KEY);
                user.Status = dictionary.GetValue<UserStatus>(RedmineKeys.STATUS);
                user.MustChangePassword = dictionary.GetValue<bool>(RedmineKeys.MUST_CHANGE_PASSWD);
                user.CustomFields = dictionary.GetValueAsCollection<IssueCustomField>(RedmineKeys.CUSTOM_FIELDS);
                user.Memberships = dictionary.GetValueAsCollection<Membership>(RedmineKeys.MEMBERSHIPS);
                user.Groups = dictionary.GetValueAsCollection<UserGroup>(RedmineKeys.GROUPS);

                return user;
            }
            return null;
        }

        /// <summary>
        ///     When overridden in a derived class, builds a dictionary of name/value pairs.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="serializer">The object that is responsible for the serialization.</param>
        /// <returns>
        ///     An object that contains key/value pairs that represent the object’s data.
        /// </returns>
        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var entity = obj as User;

            var result = new Dictionary<string, object>();

            if (entity != null)
            {
                result.Add(RedmineKeys.LOGIN, entity.Login);
                result.Add(RedmineKeys.FIRSTNAME, entity.FirstName);
                result.Add(RedmineKeys.LASTNAME, entity.LastName);
                result.Add(RedmineKeys.MAIL, entity.Email);
                result.Add(RedmineKeys.PASSWORD, entity.Password);
                result.Add(RedmineKeys.MUST_CHANGE_PASSWD, entity.MustChangePassword.ToString().ToLowerInvariant());
                result.WriteValueOrEmpty(entity.AuthenticationModeId, RedmineKeys.AUTH_SOURCE_ID);
                result.WriteArray(RedmineKeys.CUSTOM_FIELDS, entity.CustomFields, new IssueCustomFieldConverter(),
                    serializer);

                var root = new Dictionary<string, object>();
                root[RedmineKeys.USER] = result;
                return root;
            }
            return result;
        }

        /// <summary>
        ///     When overridden in a derived class, gets a collection of the supported types.
        /// </summary>
        public override IEnumerable<Type> SupportedTypes
        {
            get { return new List<Type>(new[] {typeof(User)}); }
        }

        #endregion
    }
}