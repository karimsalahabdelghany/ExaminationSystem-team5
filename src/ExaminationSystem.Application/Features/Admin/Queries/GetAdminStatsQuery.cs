using ExaminationSystem.Application.Features.Admin.Orchestrators;
using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Application.Responses;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Features.Admin.Queries
{
    public record GetAdminStatsQuery : IQuery<ApiResponse<GetAdminStatsResponse>>
    {
    }
    public class GetAdminStatsQueryHandler
    : IRequestHandler<GetAdminStatsQuery, ApiResponse<GetAdminStatsResponse>>
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

        public async Task<ApiResponse<GetAdminStatsResponse>> Handle(
            GetAdminStatsQuery request,
            CancellationToken cancellationToken)
        {
            // Serve from cache if still valid — avoids DB hit on every request
            if (_cache.TryGetValue(CacheKey, out GetAdminStatsResponse? cached) && cached is not null)
                return ApiResponse<GetAdminStatsResponse>.Success(cached);

            GetAdminStatsResponse StatsDashboard = await _mediator.Send(new GetAdminStatsOrchestrator());

            if (StatsDashboard is null)
                return ApiResponse<GetAdminStatsResponse>.Failure("Can't retrieve stats!");

            // Cache result for 5 minutes 
            _cache.Set(CacheKey, StatsDashboard, CacheDuration);

            return ApiResponse<GetAdminStatsResponse>.Success(StatsDashboard);

        }

    }
}
