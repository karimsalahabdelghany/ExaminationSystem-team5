namespace ExaminationSystem.Application.Common.Helper.Pagination;

public record PaginationResult<T>(List<T> Items, int TotalCount,int TotalPages , int PageNumber, int PageSize);

