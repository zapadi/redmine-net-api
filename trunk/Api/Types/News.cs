using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redmine.Net.Api.Types
{
    public class News
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Project Project { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Summary { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreatedOn { get; set; }
    }
}
