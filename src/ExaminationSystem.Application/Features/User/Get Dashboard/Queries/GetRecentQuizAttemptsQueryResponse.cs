using ExaminationSystem.Domain.Entities;
using ExaminationSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Features.Users.Get_Dashboard.Queries
{
    public record GetRecentQuizAttemptsQueryResponse(
        Guid AttemptId,
        Guid QuizId,
        string QuizTitle,
        QuizAttemptStatus QuizAttemptResultStatus,
        decimal? Score ,
        bool? Passed ,
        DateTime StartTime,
        DateTime? SubmittedAt
    );
    
}
