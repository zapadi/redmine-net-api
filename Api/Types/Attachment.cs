using System;

namespace Redmine.Net.Api.Types
{
    public class Attachment
    {
        public int Id { get; set; }

        public String FileName { get; set; }

        public int FileSize { get; set; }

        public String ContentType { get; set; }

        public String Description { get; set; }

        public String ContentUrl { get; set; }

        public User Author { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
