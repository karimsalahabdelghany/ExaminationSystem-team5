using ExaminationSystem.Application.Features.User.Orchestrators;
using ExaminationSystem.Application.Responses;
using Microsoft.Extensions.Caching.Memory;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Features.User.Get_Dashboard.Queries.Caching
{
    public record GetStudentDashboardQuery(Guid StudentId) : IRequest<ApiResponse<GetStudentDashboardResponse>>;
    // Handler 
    // Cache key is per-student (user_id) — as required by backend note
    // Cache dashboard response for 60 seconds per user_id
    public class GetStudentDashboardQueryHandler
        : IRequestHandler<GetStudentDashboardQuery, ApiResponse<GetStudentDashboardResponse>>
    {
        private readonly IMediator _mediator;
        private readonly IMemoryCache _cache;

        private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(60);

        // Cache key includes StudentId — each student has their own cache entry
        private static string CacheKey(Guid studentId) => $"student_dashboard_{studentId}";

        public GetStudentDashboardQueryHandler(IMediator mediator,
            IMemoryCache cache)
        {
            _mediator = mediator;
            _cache = cache;
        }

        public async Task<ApiResponse<GetStudentDashboardResponse>> Handle(
            GetStudentDashboardQuery request,
            CancellationToken cancellationToken)
        {
            var cacheKey = CacheKey(request.StudentId);

            // Per-student cache check — different students never share cache entries
            if (_cache.TryGetValue(cacheKey, out GetStudentDashboardResponse? cached)
                && cached is not null)
                return ApiResponse<GetStudentDashboardResponse>.Success(cached);

            var dashboard = await _mediator.Send(new GetStudentDashboardOrchestrator(request.StudentId));

            if (dashboard is null)
                return ApiResponse<GetStudentDashboardResponse>.Failure("Could not load dashboard.");

            // Store per student_id for 60 seconds
            _cache.Set(cacheKey, dashboard, CacheDuration);

            return ApiResponse<GetStudentDashboardResponse>.Success(dashboard);
        }
    }
}