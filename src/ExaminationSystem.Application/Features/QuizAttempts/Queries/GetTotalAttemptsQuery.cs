using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Features.QuizAttempts.Queries
{
    public record GetTotalAttemptsQuery : IQuery<RequestResult<int>>
    {
    }
    public class GetTotalAttemptsQueryHandler : IRequestHandler<GetTotalAttemptsQuery, RequestResult<int>>
    {
        private readonly IRepository<QuizAttempt> _repository;

        public GetTotalAttemptsQueryHandler(IRepository<QuizAttempt> repository)
        {
            _repository = repository;
        }
        public async Task<RequestResult<int>> Handle(GetTotalAttemptsQuery request, CancellationToken cancellationToken)
        {
            var countAttempts = await _repository.CountAsync();
            return RequestResult<int>.succeeded(countAttempts, ResultCode.TotalAttemptsQuerySuccessfull);
        }

    }
}
