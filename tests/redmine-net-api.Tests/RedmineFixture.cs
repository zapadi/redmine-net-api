
using System.Diagnostics;
using Redmine.Net.Api;

namespace redmine.net.api.Tests
{
	public class RedmineFixture
	{
	    public RedmineManager RedmineManager { get; set; }

	    public RedmineFixture ()
		{
			SetMimeTypeXML();
			SetMimeTypeJSON();
		}

		[Conditional("JSON")]
		private void SetMimeTypeJSON()
		{
			RedmineManager = new RedmineManager(Helper.Uri, Helper.ApiKey, MimeFormat.Json);
		}

		[Conditional("XML")]
		private void SetMimeTypeXML()
		{
			RedmineManager = new RedmineManager(Helper.Uri, Helper.ApiKey);
		}
	}
}