namespace Redmine.Net.Api.Internals
{
    internal static class DataHelper
    {
        public static string UserData(int userId, MimeFormat mimeFormat)
        {
            return mimeFormat == MimeFormat.XML
                ? "<user_id>" + userId + "</user_id>"
                : "{\"user_id\":\"" + userId + "\"}";
        }
    }
}