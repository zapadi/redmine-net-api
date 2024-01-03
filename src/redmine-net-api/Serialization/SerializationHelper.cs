using System.Globalization;

namespace Redmine.Net.Api.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    internal static class SerializationHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="redmineSerializer"></param>
        /// <returns></returns>
        public static string SerializeUserId(int userId, IRedmineSerializer redmineSerializer)
        {
            return redmineSerializer is XmlRedmineSerializer
                ? $"<user_id>{userId.ToString(CultureInfo.InvariantCulture)}</user_id>"
                : $"{{\"user_id\":\"{userId.ToString(CultureInfo.InvariantCulture)}\"}}";
        }
    }
}