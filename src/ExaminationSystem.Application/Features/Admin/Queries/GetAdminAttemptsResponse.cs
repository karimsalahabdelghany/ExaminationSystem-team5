using ExaminationSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Features.Admin.Queries
{
    public record GetAdminAttemptsResponse
    (
    Guid AttemptId,
    Guid StudentId,
    string StudentName,
    string QuizTitle,
    decimal? Score,
    QuizAttemptStatus Status,
    DateTime? SubmittedAt
    );
}
