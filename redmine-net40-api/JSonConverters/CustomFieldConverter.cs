using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.JSonConverters
{
    internal class CustomFieldConverter : IdentifiableNameConverter
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
                customField.PossibleValues =
                    dictionary.GetValueAsCollection<CustomFieldPossibleValue>(RedmineKeys.POSSIBLE_VALUES);
                customField.Trackers = dictionary.GetValueAsCollection<TrackerCustomField>(RedmineKeys.TRACKERS);
                customField.Roles = dictionary.GetValueAsCollection<CustomFieldRole>(RedmineKeys.ROLES);


                return customField;
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
            get { return new List<Type>(new[] {typeof(CustomField)}); }
        }

        #endregion
    }
}