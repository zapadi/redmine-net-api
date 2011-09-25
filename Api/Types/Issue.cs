using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redmine.Net.Api.Types
{
    public class Issue
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public int Id { get; set; }

        public Project Project { get; set; }

        public IdentifiableName Tracker { get; set; }

        public IdentifiableName Status { get; set; }

        public IdentifiableName Priority { get; set; }

        public User Author { get; set; }

        public string Category { get; set; }

        public string Subject { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime DueDate { get; set; }

        public int DoneRatio { get; set; }

        public float EstimatedHours { get; set; }

        public ICollection<CustomField> CustomFields { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}