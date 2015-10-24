using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.JSonConverters
{
    internal class CustomFieldConverter : IdentifiableNameConverter
    {
        #region Overrides of JavaScriptConverter

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary != null)
            {
                var customField = new CustomField();

                customField.Id = dictionary.GetValue<int>(RedmineKeys.ID);
                customField.Name = dictionary.GetValue<string>(RedmineKeys.NAME);
                customField.CustomizedType = dictionary.GetValue<string>(RedmineKeys.CUSTOMIZED_TYPE);
                customField.FieldFormat = dictionary.GetValue<string>(RedmineKeys.FIELD_FORMAT);
                customField.Regexp = dictionary.GetValue<string>(RedmineKeys.REGEXP);
                customField.MinLength = dictionary.GetValue<int?>(RedmineKeys.MIN_LENGTH);
                customField.MaxLength = dictionary.GetValue<int?>(RedmineKeys.MAX_LENGTH);
                customField.IsRequired = dictionary.GetValue<bool>(RedmineKeys.IS_REQUIRED);
                customField.IsFilter = dictionary.GetValue<bool>(RedmineKeys.IS_FILTER);
                customField.Searchable = dictionary.GetValue<bool>(RedmineKeys.SEARCHABLE);
                customField.Multiple = dictionary.GetValue<bool>(RedmineKeys.MULTIPLE);
                customField.DefaultValue = dictionary.GetValue<string>(RedmineKeys.DEFAULT_VALUE);
                customField.Visible = dictionary.GetValue<bool>(RedmineKeys.VISIBLE);
                customField.PossibleValues = dictionary.GetValueAsCollection<CustomFieldPossibleValue>(RedmineKeys.POSSIBLE_VALUES);
                customField.Trackers = dictionary.GetValueAsCollection<TrackerCustomField>(RedmineKeys.TRACKERS);
                customField.Roles = dictionary.GetValueAsCollection<CustomFieldRole>(RedmineKeys.ROLES);


                return customField;
            }

            return null;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer) { return null; }

        public override IEnumerable<Type> SupportedTypes { get { return new List<Type>(new[] { typeof(CustomField) }); } }

        #endregion
    }
}