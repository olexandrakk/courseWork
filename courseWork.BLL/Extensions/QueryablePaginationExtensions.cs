using courseWork.BLL.Common.Requests.Pagination;
using Microsoft.EntityFrameworkCore;

namespace courseWork.BLL.Extensions
{
    public static class QueryablePaginationExtensions
    {
        public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
            this IQueryable<T> query,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            if (page < 1)
                page = 1;

            if (pageSize < 1)
                pageSize = 10;

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<T>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
    }
}
