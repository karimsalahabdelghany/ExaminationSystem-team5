using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Features.Admin.Orchestrators;
using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Application.Responses;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Features.Admin.Queries
{
    public record GetAdminStatsQuery : IQuery<RequestResult<GetAdminStatsResponse>>
    {
    }
    public class GetAdminStatsQueryHandler
    : IRequestHandler<GetAdminStatsQuery, RequestResult<GetAdminStatsResponse>>
    {
        private readonly IMediator _mediator;
        private readonly IMemoryCache _cache;

        private const string CacheKey = "admin_stats";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

        public GetAdminStatsQueryHandler(IMediator mediator,
            IMemoryCache cache)
        {
            _mediator = mediator;
            _cache = cache;
        }

        public async Task <RequestResult<GetAdminStatsResponse>> Handle(
            GetAdminStatsQuery request,
            CancellationToken cancellationToken)
        {
            // Serve from cache if still valid — avoids DB hit on every request
            if (_cache.TryGetValue(CacheKey, out RequestResult<GetAdminStatsResponse>? cached) && cached is not null)
                return cached;

            var statsDashboard = await _mediator.Send (new GetAdminStatsOrchestrator(),
             cancellationToken);

            if (!statsDashboard.Success)
                return RequestResult<GetAdminStatsResponse>
                .Failure(statsDashboard.Result, statsDashboard.Code);


            var response = RequestResult<GetAdminStatsResponse>
                .succeeded(statsDashboard.Result, statsDashboard.Code);

            // Cache and read the same type to preserve cache hits.
            _cache.Set(CacheKey, response, CacheDuration);
            return response;


        }

    }
}
