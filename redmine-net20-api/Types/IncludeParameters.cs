namespace Redmine.Net.Api.Types
{
    using System;
    [Flags]
    public enum IncludeParameters
    {
        none = 0,
        children = 1 << 0,   // 1
        attachments = 1 << 1,   // 2
        relations = 1 << 2,   // 4
        changesets = 1 << 3,   // 8
        journals = 1 << 4,   // 16
        watchers = 1 << 5    // 32
    }
    public static class IncludeParametersExtensions
    {
        public static bool IsSet(this IncludeParameters source, IncludeParameters flag)
        {
            return (source & flag) == flag;
        }
    }
}