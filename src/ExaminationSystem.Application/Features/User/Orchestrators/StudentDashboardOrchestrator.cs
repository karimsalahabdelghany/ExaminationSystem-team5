using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Features.Enrollments.Queries;
using ExaminationSystem.Application.Features.User.Get_Dashboard.Queries;
using ExaminationSystem.Application.Features.Users.Get_Dashboard.Queries;
using ExaminationSystem.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExaminationSystem.Application.Features.User.Orchestrators
{
    public record GetStudentDashboardOrchestrator(Guid StudentId) : IQuery<RequestResult<GetStudentDashboardResponse>>;

    public class StudentDashboardOrchestratorHandler : IRequestHandler<GetStudentDashboardOrchestrator, RequestResult<GetStudentDashboardResponse>>
    {
        private readonly IMediator _mediator;

        public StudentDashboardOrchestratorHandler(IMediator mediator)
            => _mediator = mediator;

        public async Task<RequestResult<GetStudentDashboardResponse>> Handle(GetStudentDashboardOrchestrator request,
            CancellationToken ct)
        {
            var diplomasTask = _mediator.Send(new GetEnrolledDiplomasQuery(request.StudentId), ct);
            var recentAttemptTask = _mediator.Send(new GetRecentQuizAttemptsQuery(request.StudentId), ct);
            var overallStatsTask = _mediator.Send(new GetOverallStatsQuery(request.StudentId), ct);


            var diplomasResult = await diplomasTask;
            var attemptsResult = await recentAttemptTask;
            var statsResult = await overallStatsTask;

            var Result = new GetStudentDashboardResponse(
                EnrolledDiplomas: diplomasResult.Result,
                RecentQuizAttempts: attemptsResult.Result,
                OverallStats: statsResult.Result);

            return RequestResult<GetStudentDashboardResponse>
                .succeeded(Result, ResultCode.StudentsDashoardQuerySucessfull);
        }
    
        
    }
}
