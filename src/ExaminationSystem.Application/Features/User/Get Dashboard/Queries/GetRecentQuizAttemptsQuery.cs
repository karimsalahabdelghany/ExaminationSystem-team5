using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Application.Responses;
using ExaminationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Features.Users.Get_Dashboard.Queries
{
    public record GetRecentQuizAttemptsQuery (Guid StudentId) : IQuery<RequestResult<IEnumerable<GetRecentQuizAttemptsQueryResponse>>>
    {
    }
    public class GetRecentQuizAttemptsQueryHandler : IRequestHandler<GetRecentQuizAttemptsQuery, RequestResult<IEnumerable<GetRecentQuizAttemptsQueryResponse>>>
    {
        private readonly IRepository<QuizAttempt> _attemptRepo;

        public GetRecentQuizAttemptsQueryHandler(IRepository<QuizAttempt> attemptRepo)
            => _attemptRepo = attemptRepo;

        public async Task<RequestResult<IEnumerable<GetRecentQuizAttemptsQueryResponse>>> Handle(
            GetRecentQuizAttemptsQuery request,
            CancellationToken cancellationToken)
        {

            // Returns last 5 attempts sorted by StartTime DESC
            // Score/Passed are null when attempt is still in_progress
            var result = await _attemptRepo

                .GetAll(a => a.UserId == request.StudentId)
                .OrderByDescending(a => a.StartTime)
                .Take(5)                         // recent = last 5
                .Select(a => new GetRecentQuizAttemptsQueryResponse(
                    AttemptId: a.Id,
                    QuizId: a.QuizId,
                    QuizTitle: a.Quiz.Title,
                    QuizAttemptResultStatus: a.Status,
                    Score: a.Result != null ? a.Result.Score : null,
                    Passed: a.Result != null ? a.Result.Passed : null,
                    StartTime: a.StartTime,
                    SubmittedAt: a.SubmittedAt
                ))
                .ToListAsync(cancellationToken);

            return RequestResult<IEnumerable<GetRecentQuizAttemptsQueryResponse>>.succeeded(result,ResultCode.RecentQuizAttemptsloadedSuccessfuly);
        }
    }
}
