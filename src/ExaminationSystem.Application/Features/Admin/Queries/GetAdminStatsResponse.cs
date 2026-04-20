using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Features.Admin.Queries
{
    public record GetAdminStatsResponse(
         RequestResult<int> TotalUsers,
         RequestResult<int> ActiveUsersToday,
         int TotalQuizzes,
         int TotalAttempts,
         RequestResult<decimal> AvgPassRate
        );
   
}
