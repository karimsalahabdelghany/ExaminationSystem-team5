using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Entities;
using ExaminationSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Features.Quizzes.GetTotalQuizes
{
    public record GetTotalQuizzesQuery : IQuery<int>
    {
    }
    // DEPENDS ON: POST /api/admin/quizzes and  PATCH /api/admin/quizzes/:id/publish
    public class GetTotalQuizzesQueryHandler : IRequestHandler<GetTotalQuizzesQuery, int>
    {
        private readonly IRepository<Quiz> _quizRepo;

        public GetTotalQuizzesQueryHandler(IRepository<Quiz> quizRepo)
            => _quizRepo = quizRepo;

        public async Task<int> Handle(
            GetTotalQuizzesQuery request,
            CancellationToken cancellationToken)
            => await _quizRepo.CountAsync(
                q => q.Status == QuizStatus.Published);

    }

}
