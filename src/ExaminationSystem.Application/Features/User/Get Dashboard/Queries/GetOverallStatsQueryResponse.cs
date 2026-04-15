using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Features.User.Get_Dashboard.Queries
{
    public record GetOverallStatsQueryResponse(
         int TotalQuizzesTaken,
         decimal AvgScore,          
         decimal PassRate          
        );
       
    
}
