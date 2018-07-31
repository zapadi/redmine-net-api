using System;
using System.Collections.Generic;
using System.Text;

namespace Redmine.Net.Api.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace(this string data)
        {
#if NET20
            return string.IsNullOrEmpty(data.Trim());
#else
            return string.IsNullOrWhiteSpace(data);
#endif
        }        
    }
}
