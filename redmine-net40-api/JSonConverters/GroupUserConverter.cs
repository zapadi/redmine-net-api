using System;
using System.Collections.Generic;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.JSonConverters
{
    internal class GroupUserConverter : IdentifiableNameConverter
    {
        #region Overrides of JavaScriptConverter

        public override IEnumerable<Type> SupportedTypes { get { return new List<Type>(new[] { typeof(GroupUser) }); } }

        #endregion
    }
}