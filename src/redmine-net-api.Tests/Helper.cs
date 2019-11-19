using System.Configuration;

namespace redmine.net.api.Tests
{
	internal static class Helper
	{
		public static string Uri { get; private set; }

		public static string ApiKey { get; private set; }

		public static string Username { get; private set; }

		public static string Password { get; private set; }

		static Helper()
		{
            Uri = "http://192.168.1.53:8089";

            ApiKey = "a96e35d02bc6a6dbe655b83a2f6db57b82df2dff";


            Username = "zapadi";
			Password = "1qaz2wsx";
		}
	}
}

