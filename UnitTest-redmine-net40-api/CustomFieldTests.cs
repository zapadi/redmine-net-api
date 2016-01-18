using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest_redmine_net40_api
{
    [TestClass]
    public class CustomFieldTests
    {
        private RedmineManager redmineManager;

        private const int NUMBER_OF_CUSTOM_FIELDS = 4;
        private const bool ISSUE_CUSTOM_FIELD_EXISTS = true;

        [TestInitialize]
        public void Initialize()
        {
            SetMimeTypeJSON();
            SetMimeTypeXML();
        }

        [Conditional("JSON")]
        private void SetMimeTypeJSON()
        {
            redmineManager = new RedmineManager(Helper.Uri, Helper.ApiKey, MimeFormat.json);
        }

        [Conditional("XML")]
        private void SetMimeTypeXML()
        {
            redmineManager = new RedmineManager(Helper.Uri, Helper.ApiKey, MimeFormat.xml);
        }
 
        [TestMethod]
        public void RedmineCustomFields_ShouldGetAllCustomFields()
        {
            var customFields = redmineManager.GetObjects<CustomField>(null);

            Assert.IsNotNull(customFields, "Get custom fields returned null.");
            Assert.IsTrue(customFields.Count == NUMBER_OF_CUSTOM_FIELDS, "Custom fields count != "+NUMBER_OF_CUSTOM_FIELDS);
            CollectionAssert.AllItemsAreNotNull(customFields, "Custom fields list contains null items.");
            CollectionAssert.AllItemsAreUnique(customFields, "Custom fields items are not unique.");
            CollectionAssert.AllItemsAreInstancesOfType(customFields, typeof(CustomField), "Not all items are of type custom fields.");

            Assert.IsTrue(customFields.Exists(cf => cf.CustomizedType == RedmineKeys.ISSUE) == ISSUE_CUSTOM_FIELD_EXISTS, "Customized type check not valid.");
        }
    }
}
