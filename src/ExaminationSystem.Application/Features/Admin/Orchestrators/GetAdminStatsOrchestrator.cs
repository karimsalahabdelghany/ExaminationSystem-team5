using ExaminationSystem.Application.Features.Admin.Queries;
using ExaminationSystem.Application.Features.AttemtResults.Queries;
using ExaminationSystem.Application.Features.QuizAttempts.Queries;
using ExaminationSystem.Application.Features.Quizzes.GetTotalQuizes;
using ExaminationSystem.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExaminationSystem.Application.Features.Admin.Orchestrators
{
    public record GetAdminStatsOrchestrator : IQuery<GetAdminStatsResponse>
    {
    }
    public class GetAdminStatsOrchestratorHandler : IRequestHandler<GetAdminStatsOrchestrator, GetAdminStatsResponse>
    {
        private readonly IMediator _mediator;

        public GetAdminStatsOrchestratorHandler(IMediator mediator)
            => _mediator = mediator;
        public async Task<GetAdminStatsResponse> Handle(GetAdminStatsOrchestrator request, CancellationToken ct )
        {
            // All sub-queries fired in parallel — each hits only its own table
            var totalUsersTask = _mediator.Send(new GetTotalUsersQuery(), ct);
            var activeUsersTodayTask = _mediator.Send(new GetActiveUsersTodayQuery(), ct);
            var totalQuizzesTask = _mediator.Send(new GetTotalQuizzesQuery(), ct);
            var totalAttemptsTask = _mediator.Send(new GetTotalAttemptsQuery(), ct);
            var avgPassRateTask = _mediator.Send(new GetAvgPassRateQuery(), ct);

            await Task.WhenAll(
                totalUsersTask,
                activeUsersTodayTask,
                totalQuizzesTask,
                totalAttemptsTask,
                avgPassRateTask);

            var activeUsersToday = await activeUsersTodayTask;
            var avgPassRate = await avgPassRateTask;

            return new GetAdminStatsResponse(
                TotalUsers: await totalUsersTask,
                ActiveUsersToday: activeUsersToday.Result,
                TotalQuizzes: await totalQuizzesTask,
                TotalAttempts: await totalAttemptsTask,
                AvgPassRate: avgPassRate.Result);
        }
    }

}
