using System.Globalization;

namespace Redmine.Net.Api
{
    /// <summary>
    /// 
    /// </summary>
    internal static class SerializationHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mimeFormat"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string SerializeUserId(int userId, MimeFormat mimeFormat)
        {
            return mimeFormat == MimeFormat.Xml
                ? $"<user_id>{userId.ToString(CultureInfo.InvariantCulture)}</user_id>"
                : $"{{\"user_id\":\"{userId.ToString(CultureInfo.InvariantCulture)}\"}}";
        }
    }
}