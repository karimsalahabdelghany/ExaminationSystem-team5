using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Common.Helper.Pagination
{
    public class PaginatedResult <T>
    {
        public IEnumerable<T> Data { get; init; } = Enumerable.Empty<T>();
        public int Page { get; init; }
        public int PerPage { get; init; }
        public int Total { get; init; }
        public int TotalPages { get; init; }

        public static PaginatedResult<T> Create(
            IEnumerable<T> data,
            int total,
            int page,
            int perPage)
            => new()
            {
                Data = data,
                Total = total,
                Page = page,
                PerPage = perPage,
                TotalPages = (int)Math.Ceiling((double)total / perPage)
            };
    }
}
