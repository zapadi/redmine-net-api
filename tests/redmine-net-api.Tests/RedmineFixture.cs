using System.Diagnostics;
using Redmine.Net.Api;

namespace Padi.RedmineApi.Tests
{
	public class RedmineFixture
    {
        public RedmineCredentials Credentials { get; private set; }
	    public RedmineManager RedmineManager { get; set; }

	    public RedmineFixture ()
        {
            Credentials = TestHelper.GetApplicationConfiguration();
			SetMimeTypeXml();
			SetMimeTypeJson();
		}

		[Conditional("DEBUG_JSON")]
		private void SetMimeTypeJson()
		{
			RedmineManager = new RedmineManager(Credentials.Uri, Credentials.ApiKey, MimeFormat.Json);
		}

		[Conditional("DEBUG_XML")]
		private void SetMimeTypeXml()
		{
			RedmineManager = new RedmineManager(Credentials.Uri, Credentials.ApiKey);
		}
	}
}