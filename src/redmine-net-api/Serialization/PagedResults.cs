/*
   Copyright 2011 - 2022 Adrian Popescu

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

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

            if (pageSize <= 0)
            {
                return;
            }

            CurrentPage = offset / pageSize + 1;

            TotalPages = total / pageSize + 1;
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