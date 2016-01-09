using System.Collections.Generic;

namespace Redmine.Net.Api
{
    public class PaginatedObjects<T>
    {
        public List<T> Objects { get; set; }

        public int TotalCount { get; set; }
    }
}