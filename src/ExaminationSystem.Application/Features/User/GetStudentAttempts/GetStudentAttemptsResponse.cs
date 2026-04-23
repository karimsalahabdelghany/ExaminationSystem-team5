using ExaminationSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Features.User.GetStudentAttempts
{
    public record GetStudentAttemptsResponse
(
    Guid AttemptId,
    string QuizTitle,
    decimal? Score,
    bool? Passed,
    QuizAttemptStatus Status,       
    DateTime? SubmittedAt
    );

}
