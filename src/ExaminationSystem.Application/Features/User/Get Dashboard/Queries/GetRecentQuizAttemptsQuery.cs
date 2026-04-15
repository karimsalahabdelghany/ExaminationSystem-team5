using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Application.Responses;
using ExaminationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Features.Users.Get_Dashboard.Queries
{
    public record GetRecentQuizAttemptsQuery (Guid StudentId) : IQuery<IEnumerable<GetRecentQuizAttemptsQueryResponse>>
    {
    }
    public class GetRecentQuizAttemptsQueryHandler : IRequestHandler<GetRecentQuizAttemptsQuery, IEnumerable<GetRecentQuizAttemptsQueryResponse>>
    {
        private readonly IRepository<QuizAttempt> _attemptRepo;

        public GetRecentQuizAttemptsQueryHandler(IRepository<QuizAttempt> attemptRepo)
            => _attemptRepo = attemptRepo;

        public async Task<IEnumerable<GetRecentQuizAttemptsQueryResponse>> Handle(
            GetRecentQuizAttemptsQuery request,
            CancellationToken cancellationToken)
        {

            // Single query — joins Quiz + AttemptResult eagerly
            // Returns last 5 attempts sorted by StartTime DESC
            // Score/Passed are null when attempt is still in_progress
            var result = await _attemptRepo

                .GetAll(a => a.UserId == request.StudentId)
                .Include(a => a.Quiz)
                .Include(a => a.Result)          // AttemptResult (1-to-1, may be null)
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

            return result;
        }
    }
}
