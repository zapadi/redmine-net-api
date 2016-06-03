using System.Diagnostics;
using Redmine.Net.Api;

namespace xUnitTestredminenet45api
{
    public class RedmineFixture
    {
        public RedmineFixture()
        {
            SetMimeTypeXML();
            SetMimeTypeJSON();
        }

        public RedmineManager Manager { get; private set; }

        [Conditional("JSON")]
        private void SetMimeTypeJSON()
        {
            Manager = new RedmineManager(Helper.Uri, Helper.ApiKey, MimeFormat.JSON);
        }

        [Conditional("XML")]
        private void SetMimeTypeXML()
        {
            Manager = new RedmineManager(Helper.Uri, Helper.ApiKey);
        }
    }
}