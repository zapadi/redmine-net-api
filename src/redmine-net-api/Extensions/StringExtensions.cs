namespace Redmine.Net.Api.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string value)
        {
#if NET20
            if (value == null)
            {
                return true;
            }

            for (int index = 0; index < value.Length; ++index)
            {
                if (!char.IsWhiteSpace(value[index]))
                {
                    return false;
                }
            }
            return true;
#else
            return string.IsNullOrWhiteSpace(value);
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maximumLength"></param>
        /// <returns></returns>
        public static string Truncate(this string text, int maximumLength)
        {
            if (!text.IsNullOrWhiteSpace())
            {
                if (text.Length > maximumLength)
                {
                    text = text.Substring(0, maximumLength);
                }
            }

            return text;
        }

        /// <summary>
        /// Lower case based on invariant culture.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToLowerInv(this string text)
        {
            return text.IsNullOrWhiteSpace() ? text : text.ToLowerInvariant();
        }
    }
}