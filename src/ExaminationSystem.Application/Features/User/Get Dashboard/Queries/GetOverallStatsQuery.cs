using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Features.User.Get_Dashboard.Queries
{
    public record GetOverallStatsQuery (Guid studentid): IQuery<RequestResult<GetOverallStatsQueryResponse>>
    {
    }
    public class GetOverallStatsQueryHandler : IRequestHandler<GetOverallStatsQuery, RequestResult<GetOverallStatsQueryResponse>>
    {
        private readonly IRepository<QuizAttempt> _quizRepository;
        private readonly IRepository<AttemptResult> _attemptResultRepo;

        public GetOverallStatsQueryHandler(IRepository<QuizAttempt> quizrepository,IRepository<AttemptResult>AttemptResultRepo)
        {
            _quizRepository = quizrepository;
            _attemptResultRepo = AttemptResultRepo;
        }
        public async Task<RequestResult<GetOverallStatsQueryResponse>> Handle(GetOverallStatsQuery request, CancellationToken cancellationToken)
        {
            //  1-TotalQuizzesTaken
            //  2- avg score
            //  3 - pass rate
            // Query 1: total attempts — from QuizAttempts table
            var totalTakenTask = _quizRepository.CountAsync(
                a => a.UserId == request.studentid);

            // Query 2: avg score + pass rate — from AttemptResults table
            // Single round trip using GroupBy aggregation at SQL level
            var scoreStatsTask = _attemptResultRepo
                .GetAll(r => r.Attempt.UserId == request.studentid)
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    AvgScore = g.Average(r => r.Score),
                    TotalScored = g.Count(),
                    TotalPassed = g.Sum(r => r.Passed ? 1 : 0)
                })
                .FirstOrDefaultAsync(cancellationToken);

            //await Task.WhenAll(totalTakenTask, scoreStatsTask);

            var totalTaken = await totalTakenTask;
            var scoreStats = await scoreStatsTask;

            var avgScore = scoreStats is null ? 0m : Math.Round(scoreStats.AvgScore, 2);
            var passRate = scoreStats is null || scoreStats.TotalScored == 0
                ? 0m
                : Math.Round((decimal)scoreStats.TotalPassed / scoreStats.TotalScored * 100, 2);

            var result = new GetOverallStatsQueryResponse(
                TotalQuizzesTaken: totalTaken,
                AvgScore: avgScore,
                PassRate: passRate);
            return RequestResult<GetOverallStatsQueryResponse>.succeeded(result, ResultCode.OverAllStatsProgressQuerySucessful);
        }

    }
    
}
