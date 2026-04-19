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
    public record GetStudentDashboardOrchestrator(Guid StudentId) : IQuery<GetStudentDashboardResponse>;

    public class StudentDashboardOrchestratorHandler : IRequestHandler<GetStudentDashboardOrchestrator, GetStudentDashboardResponse>
    {
        private readonly IMediator _mediator;

        public StudentDashboardOrchestratorHandler(IMediator mediator)
            => _mediator = mediator;

        public async Task<GetStudentDashboardResponse> Handle(GetStudentDashboardOrchestrator request, CancellationToken ct)
        {
            // All 3 sub-queries fired in parallel — independent of each other
            var diplomasTask = _mediator.Send(new GetEnrolledDiplomasQuery(request.StudentId), ct);
            var recentAttemptTask = _mediator.Send(new GetRecentQuizAttemptsQuery(request.StudentId), ct);
            var overallStatsTask = _mediator.Send(new GetOverallStatsQuery(request.StudentId), ct);

            await Task.WhenAll(diplomasTask, recentAttemptTask, overallStatsTask);

            var diplomas = await diplomasTask;
            var recent = await recentAttemptTask;
            var overall = await overallStatsTask;

            return new GetStudentDashboardResponse(
                EnrolledDiplomas: diplomas.Result ?? Enumerable.Empty<EnrolledDiplomasResponse>(),
                RecentQuizAttempts: recent.Result ?? Enumerable.Empty<GetRecentQuizAttemptsQueryResponse>(),
                OverallStats: overall.Result!);
        }
    
        
    }
}
