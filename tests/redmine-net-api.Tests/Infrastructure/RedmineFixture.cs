using System.Diagnostics;
using Redmine.Net.Api;
using Redmine.Net.Api.Authentication;
using Redmine.Net.Api.Serialization;

namespace Padi.DotNet.RedmineAPI.Tests.Infrastructure
{
	public sealed class RedmineFixture
    {
        public RedmineCredentials Credentials { get; }
	    public RedmineManager RedmineManager { get; private set; }

        private readonly RedmineManagerOptionsBuilder _redmineManagerOptionsBuilder;

	    public RedmineFixture ()
        {
            Credentials = TestHelper.GetApplicationConfiguration();

            _redmineManagerOptionsBuilder = new RedmineManagerOptionsBuilder()
                                            .WithHost(Credentials.Uri)
                                            .WithAuthentication(new RedmineApiKeyAuthentication(Credentials.ApiKey));
            
			SetMimeTypeXml();
			SetMimeTypeJson();
		}

		[Conditional("DEBUG_JSON")]
		private void SetMimeTypeJson()
		{
			RedmineManager = new RedmineManager(_redmineManagerOptionsBuilder.WithSerializationType(SerializationType.Json));
		}

		[Conditional("DEBUG_XML")]
		private void SetMimeTypeXml()
		{
			RedmineManager = new RedmineManager(_redmineManagerOptionsBuilder.WithSerializationType(SerializationType.Xml));
		}
	}
}