using ExaminationSystem.Application.Common.Helper.Pagination;
using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Features.User.GetStudentAttempts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Features.User.GetStudentAttempts
{
    public record GetStudentAttemptsQuery(
     PaginationParams Pagination,
     Guid? QuizId = null,   
     Guid? DiplomaId = null   
    ) : IRequest<RequestResult<PaginatedResult<GetStudentAttemptsResponse>>>;

}
public class GetStudentAttemptsQueryHandler
    : IRequestHandler<GetStudentAttemptsQuery, RequestResult<PaginatedResult<GetStudentAttemptsResponse>>>
{
    private readonly IRepository<QuizAttempt> _repository;
    private readonly ICurrentUser _currentUser;

    public GetStudentAttemptsQueryHandler(IRepository<QuizAttempt> repository,ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<RequestResult<PaginatedResult<GetStudentAttemptsResponse>>> Handle(
        GetStudentAttemptsQuery request,
        CancellationToken cancellationToken)
    {
        var query =_repository
            .GetAll(a => a.UserId == _currentUser.Id)
            .AsNoTracking();

        // Optional filter: 
        if (request.QuizId.HasValue)
            query = query.Where(a => a.QuizId == request.QuizId.Value);

        if (request.DiplomaId.HasValue)
            query = query.Where(a => a.Quiz.DiplomaId == request.DiplomaId.Value);

        var result = await query
            .OrderByDescending(a => a.SubmittedAt ?? a.StartTime)

            .Select(a => new GetStudentAttemptsResponse(
                AttemptId: a.Id,
                QuizTitle: a.Quiz.Title,
                Score: a.Result != null ? a.Result.Score : null,
                Passed: a.Result != null ? a.Result.Passed : null,
                Status: a.Status,
                SubmittedAt: a.SubmittedAt
            ))
            .ToPagedAsync(request.Pagination, cancellationToken);

        return RequestResult<PaginatedResult<GetStudentAttemptsResponse>>.succeeded(result, ResultCode.RecentQuizAttemptsloadedSuccessfuly);
    }
}
