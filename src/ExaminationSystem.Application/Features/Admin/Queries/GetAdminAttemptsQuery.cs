using ExaminationSystem.Application.Common.Helper.Pagination;
using ExaminationSystem.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Features.Admin.Queries
{
    public record GetAdminAttemptsQuery(
    
    PaginationParams Pagination,
    Guid? QuizId = null,   // optional filter
    Guid? StudentId = null    // optional filter
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

            var result = await query
                .OrderByDescending(a => a.SubmittedAt ?? a.StartTime)
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

            return RequestResult<PaginatedResult<GetAdminAttemptsResponse>>.succeeded(result,ResultCode.TotalAttemptsQuerySuccessfull);
        }
    }

}
