using System.Diagnostics;
using Redmine.Net.Api;
using Redmine.Net.Api.Serialization;

namespace Padi.DotNet.RedmineAPI.Tests.Infrastructure.Fixtures
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
                                            .WithHost(Credentials.Uri ?? "localhost")
                                            .WithApiKeyAuthentication(Credentials.ApiKey);
            
			SetMimeTypeXml();
			SetMimeTypeJson();
            
            RedmineManager = new RedmineManager(_redmineManagerOptionsBuilder);
		}

		[Conditional("DEBUG_JSON")]
		private void SetMimeTypeJson()
		{
            _redmineManagerOptionsBuilder.WithSerializationType(SerializationType.Json);
		}

		[Conditional("DEBUG_XML")]
		private void SetMimeTypeXml()
		{
            _redmineManagerOptionsBuilder.WithSerializationType(SerializationType.Xml);
		}
	}
}