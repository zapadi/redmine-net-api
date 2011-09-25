using System;

namespace Redmine.Net.Api.Types
{
    public class Version
    {
        public int Id { get; set; }
        public Project Project { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public String Status { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
