using ExaminationSystem.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Features.Admin.Queries
{
    public record GetAdminStatsResponse(
         int TotalUsers,
         int ActiveUsersToday,
         int TotalQuizzes,
         int TotalAttempts,
         decimal AvgPassRate
        );
   
}
