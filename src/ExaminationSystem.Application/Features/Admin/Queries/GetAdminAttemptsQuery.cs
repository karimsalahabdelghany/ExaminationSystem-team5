using ExaminationSystem.Application.Common.Helper.Pagination;
using ExaminationSystem.Application.Common.Results;

namespace ExaminationSystem.Application.Features.Admin.Queries
{
    public record GetAdminAttemptsQuery(

    PaginationParams Pagination,
    Guid? QuizId = null,        // optional filter
    Guid? StudentId = null,     // optional filter
    string? SortBy = null,      // submitted_at | score | status  (default: submitted_at)
    string? Order = null        // asc | desc  (default: desc)
    ) : IRequest<RequestResult<PaginatedResult<GetAdminAttemptsResponse>>>;

    // Handler 
    public class GetAdminAttemptsQueryHandler
    : IRequestHandler<GetAdminAttemptsQuery, RequestResult<PaginatedResult<GetAdminAttemptsResponse>>>
    {
        private readonly IRepository<QuizAttempt> _quizAttemptrepo;

        public GetAdminAttemptsQueryHandler(IRepository<QuizAttempt> QuizAttemptrepo)
        {
            _quizAttemptrepo = QuizAttemptrepo;
        }

        public async Task<RequestResult<PaginatedResult<GetAdminAttemptsResponse>>> Handle(
            GetAdminAttemptsQuery request,
            CancellationToken cancellationToken)
        {
            var query = _quizAttemptrepo.GetAll(); //  admin sees all

            // Optional filter: ?quiz_id=
            if (request.QuizId.HasValue)
                query = query.Where(a => a.QuizId == request.QuizId.Value);

            // Optional filter: ?student_id=
            if (request.StudentId.HasValue)
                query = query.Where(a => a.UserId == request.StudentId.Value);

            // Apply sort: ?sort_by= & ?order=
            var orderedQuery = ApplySorting(query, request.SortBy, request.Order);

            var result = await orderedQuery
                .Select(a => new GetAdminAttemptsResponse(
                    AttemptId: a.Id,
                    StudentId: a.UserId,
                    StudentName: a.Student.FullName,
                    QuizTitle: a.Quiz.Title,
                    Score: a.Result != null ? a.Result.Score : null,
                    Status: a.Status,
                    SubmittedAt: a.SubmittedAt
                ))
                .ToPagedAsync(request.Pagination, cancellationToken); ;

            return RequestResult<PaginatedResult<GetAdminAttemptsResponse>>.succeeded(result, ResultCode.TotalAttemptsQuerySuccessfull);
        }

        // Default sort: submitted_at DESC
        private static IQueryable<QuizAttempt> ApplySorting(IQueryable<QuizAttempt> query, string? sortBy, string? order)
        {
            var isAscending = string.Equals(order, "asc", StringComparison.OrdinalIgnoreCase);

            return sortBy?.ToLowerInvariant() switch
            {
                "score" => isAscending
                    ? query.OrderBy(a => a.Result!.Score)
                    : query.OrderByDescending(a => a.Result!.Score),
                "status" => isAscending
                    ? query.OrderBy(a => a.Status)
                    : query.OrderByDescending(a => a.Status),
                _ => isAscending
                    ? query.OrderBy(a => a.SubmittedAt ?? a.StartTime)
                    : query.OrderByDescending(a => a.SubmittedAt ?? a.StartTime)
            };
        }
    }

}
