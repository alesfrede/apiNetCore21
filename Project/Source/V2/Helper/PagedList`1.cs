using System;
using System.Collections.Generic;
using System.Linq;

namespace Api213.V2.Helper
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedList<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{T}"/> class.
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        public PagedList(IQueryable<T> source, int pageNumber, int pageSize)
        {
            TotalItems = source.Count();
            PageNumber = pageNumber;
            PageSize = pageSize;
            List = source
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        public int TotalItems { get; }

        /// <summary>
        /// 
        /// </summary>
        public int PageNumber { get; }

        /// <summary>
        /// 
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// 
        /// </summary>
        public List<T> List { get; }

        /// <summary>
        /// 
        /// </summary>
        public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);

        /// <summary>
        /// 
        /// </summary>
        public bool HasPreviousPage => PageNumber > 1;

        /// <summary>
        /// 
        /// </summary>
        public bool HasNextPage => PageNumber < TotalPages;

        /// <summary>
        /// 
        /// </summary>
        public int NextPageNumber => HasNextPage ? PageNumber + 1 : TotalPages;

        /// <summary>
        /// 
        /// </summary>
        public int PreviousPageNumber => HasPreviousPage ? PageNumber - 1 : 1;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public PagingHeader GetHeader()
        {
            return new PagingHeader(
                TotalItems, 
                PageNumber,
                PageSize, 
                TotalPages);
        }
    }
}
