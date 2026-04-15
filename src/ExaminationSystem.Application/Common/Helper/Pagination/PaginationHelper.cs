using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Application.Common.Helper.Pagination;

public static class PaginationHelper
{
    public static async Task<PaginationResult<T>> PaginateAsync<T>(
      this IQueryable<T> query,
      int? pageNumber,
      int? pageSize,
      CancellationToken cancellationToken)
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        // Validate and sanitize page size (default: 10, min: 1, max: 100)
        const int defaultPageSize = 10;
        const int maxPageSize = 100;

        int validatedPageSize = pageSize ?? defaultPageSize;

        if (validatedPageSize <= 0)
            validatedPageSize = defaultPageSize;

        if (validatedPageSize > maxPageSize)
            validatedPageSize = maxPageSize;

        // Validate and sanitize page number (default: 1, min: 1)
        int validatedPageNumber = pageNumber ?? 1;

        if (validatedPageNumber <= 0)
            validatedPageNumber = 1;

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Calculate total pages
        int totalPages = totalCount == 0
            ? 1
            : (int)Math.Ceiling(totalCount / (double)validatedPageSize);

        // Adjust page number if it exceeds total pages
        if (validatedPageNumber > totalPages && totalPages > 0)
            validatedPageNumber = totalPages;

        // Get paginated items
        var paginatedItems = await query
            .Skip((validatedPageNumber - 1) * validatedPageSize)
            .Take(validatedPageSize)
            .ToListAsync(cancellationToken);

        // Return with enhanced metadata
        return new PaginationResult<T>(
            paginatedItems,
            totalCount,
            totalPages,
            validatedPageNumber,
            validatedPageSize
        );
    }
}
