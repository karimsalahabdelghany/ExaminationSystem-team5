using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Features.Diplomas.GetDiplomas
{
    public record GetDiplomasResponse
    (
      Guid Id,
      string Title,
      string? Description,
      int QuizCount,
      decimal StudentProgress  
    );
}
