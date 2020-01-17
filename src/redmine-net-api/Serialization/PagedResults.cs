using System.Collections.Generic;

namespace Redmine.Net.Api.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class PagedResults<TOut> where TOut: class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="total"></param>
        /// <param name="offset"></param>
        /// <param name="pageSize"></param>
        public PagedResults(IEnumerable<TOut> items, int total, int offset, int pageSize)
        {
            Items = items;
            TotalItems = total;
            Offset = offset;
            PageSize = pageSize;

            if (pageSize > 0)
            {
                CurrentPage = offset / pageSize + 1;

                TotalPages = total / pageSize + 1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// 
        /// </summary>
        public int Offset { get; }

        /// <summary>
        /// 
        /// </summary>
        public int CurrentPage { get; }

        /// <summary>
        /// 
        /// </summary>
        public int TotalItems { get; }

        /// <summary>
        /// 
        /// </summary>
        public int TotalPages { get; }
       
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<TOut> Items { get; }
    }
}