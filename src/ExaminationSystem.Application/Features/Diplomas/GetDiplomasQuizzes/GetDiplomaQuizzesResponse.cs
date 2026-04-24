using ExaminationSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Features.Diplomas.GetDiplomasQuizzes
{
    public record GetDiplomaQuizzesResponse
    (
    Guid Id,
    string Title,
    int DurationMinutes,
    int AttemptCount,  
    decimal? LastScore,     
    QuizStatus Status
    );

}
