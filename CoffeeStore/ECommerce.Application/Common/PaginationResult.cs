using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Common
{
    public class PaginationResult<TEntity>
    {
        public PaginationResult(int pageIndex, int pageSize, int count, IReadOnlyList<TEntity> data)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            Count = count;
            Data = data;
        }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }
        public IReadOnlyList<TEntity> Data { get; set; }
    }
}
