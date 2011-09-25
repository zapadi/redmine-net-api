using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redmine.Net.Api.Types
{
    public class Project : IdentifiableName
    {
        /// <summary>
        /// 
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime UpdatedOn { get; set; }
    }
}