using System.Diagnostics;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [XmlRoot(RedmineKeys.TIME_ENTRY_ACTIVITY)]
    public sealed class ProjectTimeEntryActivity : IdentifiableName
    {
        /// <summary>
        /// 
        /// </summary>
        public ProjectTimeEntryActivity() { }

        internal ProjectTimeEntryActivity(int id, string name)
            : base(id, name)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string DebuggerDisplay => $"[{nameof(ProjectTimeEntryActivity)}: {ToString()}]";

    }
}