using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Features.AttemtResults.Queries
{
    public record GetAvgPassRateQuery : IQuery<decimal>
    {
    }
    public class GetAvgPassRateQueryHandler : IRequestHandler<GetAvgPassRateQuery, decimal>
    {
        private readonly IRepository<AttemptResult> _resultRepo;

        public GetAvgPassRateQueryHandler(IRepository<AttemptResult> resultRepo)
            => _resultRepo = resultRepo;

        public async Task<decimal> Handle(
            GetAvgPassRateQuery request,
            CancellationToken cancellationToken)
        {
            var totalTask = _resultRepo.CountAsync(t => t.Passed);
            var passedTask = _resultRepo.CountAsync();

            await Task.WhenAll(totalTask, passedTask);        // in parellel

            var total = await totalTask;
            var passed = await passedTask;

            return total == 0
                ? 0m
                : Math.Round((decimal)passed / total * 100, 2);
        }
    }
}
