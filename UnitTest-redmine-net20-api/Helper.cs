using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest_redmine_net20_api
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