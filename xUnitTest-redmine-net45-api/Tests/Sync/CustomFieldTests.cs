using Xunit;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;

namespace xUnitTestredminenet45api
{
	[Collection("RedmineCollection")]
	public class CustomFieldTests
	{
		private const int NUMBER_OF_CUSTOM_FIELDS = 10;
		private const bool ISSUE_CUSTOM_FIELD_EXISTS = true;

	    private readonly RedmineFixture fixture;
		public CustomFieldTests (RedmineFixture fixture)
		{
			this.fixture = fixture;
		}

		[Fact]
		public void RedmineCustomFields_ShouldGetAllCustomFields()
		{
			var customFields = fixture.RedmineManager.GetObjects<CustomField>(null);

			Assert.NotNull(customFields);
			Assert.True(customFields.Count == NUMBER_OF_CUSTOM_FIELDS, "Custom fields count != "+NUMBER_OF_CUSTOM_FIELDS);
			Assert.All (customFields, cf => Assert.IsType<CustomField> (cf));

			Assert.True(customFields.Exists(cf => cf.CustomizedType == RedmineKeys.ISSUE) == ISSUE_CUSTOM_FIELD_EXISTS, "Customized type check not valid.");
		}
	}
}