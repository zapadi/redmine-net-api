/*
   Copyright 2011 - 2015 Adrian Popescu

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
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.JSonConverters
{
    internal class UserConverter : JavaScriptConverter
    {
        #region Overrides of JavaScriptConverter

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary != null)
            {
                User user = new User();
                user.Login = dictionary.GetValue<string>("login");
                user.Id = dictionary.GetValue<int>("id");
                user.FirstName = dictionary.GetValue<string>("firstname");
                user.LastName = dictionary.GetValue<string>("lastname");
                user.Email = dictionary.GetValue<string>("mail");
                user.AuthenticationModeId = dictionary.GetValue<int?>("auth_source_id");
                user.CreatedOn = dictionary.GetValue<DateTime?>("created_on");
                user.LastLoginOn = dictionary.GetValue<DateTime?>("last_login_on");
                user.ApiKey = dictionary.GetValue<string>("api_key");
                user.Status = dictionary.GetValue<UserStatus>("status");
                user.CustomFields = dictionary.GetValueAsCollection<IssueCustomField>("custom_fields");
                user.Memberships = dictionary.GetValueAsCollection<Membership>("memberships");
                user.Groups = dictionary.GetValueAsCollection<UserGroup>("groups");

                return user;
            }
            return null;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var entity = obj as User;
            var root = new Dictionary<string, object>();
            var result = new Dictionary<string, object>();

            if (entity != null)
            {
                result.Add("login", entity.Login);
                result.Add("firstname", entity.FirstName);
                result.Add("lastname", entity.LastName);
                result.Add("mail", entity.Email);
                result.Add("password", entity.Password);
                result.WriteIfNotDefaultOrNull(entity.AuthenticationModeId, "auth_source_id");

                if (entity.CustomFields != null)
                {
                    serializer.RegisterConverters(new[] { new IssueCustomFieldConverter() });
                    result.Add("custom_fields", entity.CustomFields.ToArray());
                }

                root["user"] = result;
                return root;
            }
            return result;
        }

        public override IEnumerable<Type> SupportedTypes { get { return new List<Type>(new[] { typeof(User) }); } }

        #endregion
    }
}
