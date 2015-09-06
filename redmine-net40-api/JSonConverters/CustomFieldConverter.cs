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

                customField.Id = dictionary.GetValue<int>("id");
                customField.Name = dictionary.GetValue<string>("name");
                customField.CustomizedType = dictionary.GetValue<string>("customized_type");
                customField.FieldFormat = dictionary.GetValue<string>("field_format");
                customField.Regexp = dictionary.GetValue<string>("regexp");
                customField.MinLength = dictionary.GetValue<int?>("min_length");
                customField.MaxLength = dictionary.GetValue<int?>("max_length");
                customField.IsRequired = dictionary.GetValue<bool>("is_required");
                customField.IsFilter = dictionary.GetValue<bool>("is_filter");
                customField.Searchable = dictionary.GetValue<bool>("searchable");
                customField.Multiple = dictionary.GetValue<bool>("multiple");
                customField.DefaultValue = dictionary.GetValue<string>("default_value");
                customField.Visible = dictionary.GetValue<bool>("visible");
                customField.PossibleValues = dictionary.GetValueAsCollection<CustomFieldPossibleValue>("possible_values");
                customField.Trackers = dictionary.GetValueAsCollection<TrackerCustomField>("trackers");
                customField.Roles = dictionary.GetValueAsCollection<CustomFieldRole>("roles");


                return customField;
            }

            return null;
        }

        public override IEnumerable<Type> SupportedTypes { get { return new List<Type>(new[] { typeof(CustomField) }); } }

        #endregion
    }
}