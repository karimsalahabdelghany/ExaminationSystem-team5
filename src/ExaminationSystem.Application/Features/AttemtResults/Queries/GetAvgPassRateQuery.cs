using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Features.AttemtResults.Queries
{
    public record GetAvgPassRateQuery : IQuery<RequestResult<decimal>>
    {
    }
    public class GetAvgPassRateQueryHandler : IRequestHandler<GetAvgPassRateQuery, RequestResult<decimal>>
    {
        private readonly IRepository<AttemptResult> _attemptresultRepo;

        public GetAvgPassRateQueryHandler(IRepository<AttemptResult> AttemptresultRepo)
        {

            _attemptresultRepo = AttemptresultRepo;
        }

        public async Task<RequestResult<decimal>> Handle(
            GetAvgPassRateQuery request,
            CancellationToken cancellationToken)
        {
            var totalTask = _attemptresultRepo.CountAsync(t => t.Passed);
            var passedTask = _attemptresultRepo.CountAsync();

           /* await Task.WhenAll(totalTask, passedTask);*/        // in parellel

            var total = await totalTask;
            var passed = await passedTask;

            if (total == 0)
            {
                return RequestResult<decimal>.Failure(0m, ResultCode.AvgPassRateFailed);
            }

            var avg = Math.Round((decimal)passed / total * 100, 2);

            return RequestResult<decimal>.succeeded(avg, ResultCode.AvgPassRateSuccessed);
        }
    }
}


    


