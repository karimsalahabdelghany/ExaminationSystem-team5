using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Common.Helper.Pagination
{
    public static class PaginationExtensions
    {
        
        public static async Task<PaginatedResult<T>> ToPagedAsync<T>(
            this IQueryable<T> query,
            PaginationParams pagination,
            CancellationToken ct = default)
        {
            var countTask =await query.CountAsync(ct);
            var totalPages =  countTask == 0 ? 1: (int)Math.Ceiling( countTask / (double)pagination.PerPage);

            // Adjust page number if it exceeds total pages
            var page = pagination.Page > totalPages
                ? totalPages : pagination.Page;

            var skip = (page - 1) * pagination.PerPage;

            // Get paginated items
            var dataTask = await query
                .Skip(skip)
                .Take(pagination.PerPage)
                .ToListAsync(ct);

            return PaginatedResult<T>.Create(
                data:  dataTask,
                total:  countTask,
                page: pagination.Page,
                perPage: pagination.PerPage);
        }
    }
}
