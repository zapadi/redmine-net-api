using System.Configuration;

namespace xUnitTestredminenet45api
{
	internal static class Helper
	{
		public static string Uri { get; private set; }

		public static string ApiKey { get; private set; }

		public static string Username { get; private set; }

		public static string Password { get; private set; }

		static Helper()
		{
			Uri = ConfigurationManager.AppSettings["uri"];
			ApiKey = ConfigurationManager.AppSettings["apiKey"];

			Username = ConfigurationManager.AppSettings["username"];
			Password = ConfigurationManager.AppSettings["password"];
		}
	}
}

