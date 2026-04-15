using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Features.Enrollments.Queries
{
    public record EnrolledDiplomasResponse(
         Guid DiplomaId,
         string Title,
         string Description,
         int TotalQuizzes,
         int CompletedQuizzes,
         decimal ProgressPercent
    );
    
}
