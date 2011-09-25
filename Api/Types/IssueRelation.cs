using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redmine.Net.Api.Types
{
    public class IssueRelation
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int IssueId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int IssueToId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Delay { get; set; }
    }
}
