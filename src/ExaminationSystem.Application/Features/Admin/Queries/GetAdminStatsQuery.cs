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
            if (_cache.TryGetValue(CacheKey, out GetAdminStatsResponse? cached) && cached is not null)
                return RequestResult<GetAdminStatsResponse>.succeeded(cached,ResultCode.AdminStatsDataAlreadyCashedinMemory);

            GetAdminStatsResponse StatsDashboard = await _mediator.Send(new GetAdminStatsOrchestrator());

            if (StatsDashboard is null)
                return RequestResult<GetAdminStatsResponse>.Failure(null!,ResultCode.AdminStatsDataNotFound);

            // Cache result for 5 minutes 
            _cache.Set(CacheKey, StatsDashboard, CacheDuration);

            return RequestResult<GetAdminStatsResponse>.succeeded(StatsDashboard,ResultCode.AdminStatsQueryFiredSuccessfully);
        }

    }
}
