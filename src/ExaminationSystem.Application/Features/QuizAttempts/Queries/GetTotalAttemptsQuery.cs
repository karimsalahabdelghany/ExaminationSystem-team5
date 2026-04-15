using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Features.QuizAttempts.Queries
{
    public record GetTotalAttemptsQuery : IQuery<int>
    {
    }
    public class GetTotalAttemptsQueryHandler : IRequestHandler<GetTotalAttemptsQuery, int>
    {
        private readonly IRepository<QuizAttempt> _repository;

        public GetTotalAttemptsQueryHandler(IRepository<QuizAttempt> repository)
        {
            _repository = repository;
        }
        public async Task<int> Handle(GetTotalAttemptsQuery request, CancellationToken cancellationToken)
         => await _repository.CountAsync();
        
    }
}
