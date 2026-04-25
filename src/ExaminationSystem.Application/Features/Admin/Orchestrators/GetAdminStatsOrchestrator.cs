using ExaminationSystem.Application.Common.Results;
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
            var totalUsersTask =await _mediator.Send(new GetTotalUsersQuery(), ct);
            var activeUsersTodayTask =await _mediator.Send(new GetActiveUsersTodayQuery(), ct);
            var totalQuizzesTask = await _mediator.Send(new GetTotalQuizzesQuery(), ct);
            var totalAttemptsTask = await _mediator.Send(new GetTotalAttemptsQuery(), ct);
            var avgPassRateTask = await _mediator.Send(new GetAvgPassRateQuery(), ct);

            

            var  Result =  new GetAdminStatsResponse(
                TotalUsers:  totalUsersTask.Result,
                ActiveUsersToday:activeUsersTodayTask.Result,
                TotalQuizzes:  totalQuizzesTask.Result,
                TotalAttempts:  totalAttemptsTask.Result,
                AvgPassRate:  avgPassRateTask.Result);

            return RequestResult<GetAdminStatsResponse>.succeeded(Result, ResultCode.AdminStatsQueryFiredSuccessfully);
        }
    }

}
