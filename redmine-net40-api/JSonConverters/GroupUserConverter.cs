using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.JSonConverters
{
    internal class GroupUserConverter : IdentifiableNameConverter
    {
        #region Overrides of JavaScriptConverter

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary != null)
            {
                var userGroup = new GroupUser();

                userGroup.Id = dictionary.GetValue<int>("id");
                userGroup.Name = dictionary.GetValue<string>("name");

                return userGroup;
            }

            return null;
        }

        public override IEnumerable<Type> SupportedTypes { get { return new List<Type>(new[] { typeof(GroupUser) }); } }

        #endregion
    }
}