using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Features.Enrollments.Queries;
using ExaminationSystem.Application.Features.User.Get_Dashboard.Queries;
using ExaminationSystem.Application.Features.Users.Get_Dashboard.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Features.User.Orchestrators
{
    public record GetStudentDashboardResponse
    (
        IEnumerable<EnrolledDiplomasResponse> EnrolledDiplomas,
        IEnumerable<GetRecentQuizAttemptsQueryResponse> RecentQuizAttempts,
        GetOverallStatsQueryResponse OverallStats
    );
}
