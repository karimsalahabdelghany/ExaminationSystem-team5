using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Entities;
using ExaminationSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Features.Quizzes.GetTotalQuizes
{
    public record GetTotalQuizzesQuery : IQuery<RequestResult<int>>
    {
    }
    public class GetTotalQuizzesQueryHandler
     : IRequestHandler<GetTotalQuizzesQuery, RequestResult<int>>
    {
        private readonly IRepository<Quiz> _quizRepo;

        public GetTotalQuizzesQueryHandler(IRepository<Quiz> quizRepo)
            => _quizRepo = quizRepo;

        public async Task<RequestResult<int>> Handle(
            GetTotalQuizzesQuery request,
            CancellationToken cancellationToken)
        {
            var count = await _quizRepo.CountAsync(
                q => q.Status == QuizStatus.Published);

            return RequestResult<int>.succeeded(
                count,
                ResultCode.TotalQuzizesQuerySucessfull
            );
        }
    }

}
