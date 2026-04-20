using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Features.Admin.Queries;
using ExaminationSystem.Application.Features.AttemtResults.Queries;
using ExaminationSystem.Application.Features.QuizAttempts.Queries;
using ExaminationSystem.Application.Features.Quizzes.GetTotalQuizes;
using ExaminationSystem.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Features.Admin.Orchestrators
{
    public record GetAdminStatsOrchestrator : IQuery<RequestResult<GetAdminStatsResponse>>
    {
    }
    public class GetAdminStatsOrchestratorHandler : IRequestHandler<GetAdminStatsOrchestrator,RequestResult<GetAdminStatsResponse>>
    {
        private readonly IMediator _mediator;

        public GetAdminStatsOrchestratorHandler(IMediator mediator)
            => _mediator = mediator;
        public async Task<RequestResult<GetAdminStatsResponse>> Handle(GetAdminStatsOrchestrator request, CancellationToken ct )
        {
            // All sub-queries fired in parallel — each hits only its own table
            var totalUsersTask = _mediator.Send(new GetTotalUsersQuery(), ct);
            var activeUsersTodayTask = _mediator.Send(new GetActiveUsersTodayQuery(), ct);
            var totalQuizzesTask = _mediator.Send(new GetTotalQuizzesQuery(), ct);
            var totalAttemptsTask = _mediator.Send(new GetTotalAttemptsQuery(), ct);
            var avgPassRateTask = _mediator.Send(new GetAvgPassRateQuery(), ct);

            

            var  Result =  new GetAdminStatsResponse(
                TotalUsers: await totalUsersTask,
                ActiveUsersToday: await activeUsersTodayTask,
                TotalQuizzes: await totalQuizzesTask,
                TotalAttempts: await totalAttemptsTask,
                AvgPassRate: await avgPassRateTask);

            return RequestResult<GetAdminStatsResponse>.succeeded(Result, ResultCode.AdminStatsQueryFiredSuccessfully);
        }
    }

}
