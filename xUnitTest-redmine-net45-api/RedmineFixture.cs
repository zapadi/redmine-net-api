using System;
using System.Diagnostics;
using Redmine.Net.Api;

namespace xUnitTestredminenet45api
{
	public class RedmineFixture
	{
		public RedmineManager redmineManager;

		public RedmineFixture ()
		{
			SetMimeTypeXML();
			SetMimeTypeJSON();
		}

		[Conditional("JSON")]
		private void SetMimeTypeJSON()
		{
			redmineManager = new RedmineManager(Helper.Uri, Helper.ApiKey, MimeFormat.json);
		}

		[Conditional("XML")]
		private void SetMimeTypeXML()
		{
			redmineManager = new RedmineManager(Helper.Uri, Helper.ApiKey);
		}
	}
}

