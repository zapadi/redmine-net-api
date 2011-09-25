using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redmine.Net.Api.Types
{
    public class Query : IdentifiableName
    {
       /// <summary>
        /// 
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ProjectId { get; set; }
    }
}