using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Features.Admin.Queries;
using ExaminationSystem.Application.Features.User.Orchestrators;
using ExaminationSystem.Application.Responses;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ExaminationSystem.Application.Features.User.Get_Dashboard.Queries.Caching
{
    public record GetStudentDashboardQuery() : IRequest<RequestResult<GetStudentDashboardResponse>>;
    // Handler 
    // Cache key is per-student (user_id) — as required by backend note
    // Cache dashboard response for 60 seconds per user_id
    public class GetStudentDashboardQueryHandler
        : IRequestHandler<GetStudentDashboardQuery, RequestResult<GetStudentDashboardResponse>>
    {
        private readonly IMediator _mediator;
        private readonly IMemoryCache _cache;
        private readonly ICurrentUser _currentUser;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(60);

        // Cache key includes StudentId — each student has their own cache entry
        private static string CacheKey(Guid studentId) => $"student_dashboard_{studentId}";

        public GetStudentDashboardQueryHandler(IMediator mediator,
            IMemoryCache cache ,ICurrentUser currentUser)
        {
            _mediator = mediator;
            _cache = cache;
            _currentUser = currentUser;
        }

        public async Task<RequestResult<GetStudentDashboardResponse>> Handle(
            GetStudentDashboardQuery request,
            CancellationToken cancellationToken)
        {
            if (!_currentUser.TryGetUserId(out var studentId))
                return RequestResult<GetStudentDashboardResponse>
                    .Failure(null, ResultCode.InvalidCredentials);

            var cacheKey = CacheKey(studentId);

            if (_cache.TryGetValue(cacheKey, out GetStudentDashboardResponse? cached)
                && cached is not null)
            {
                return RequestResult<GetStudentDashboardResponse>.succeeded(
                    cached,
                    ResultCode.StudentStatsDataAlreadyCachedinMemory);
            }

            var dashboard = await _mediator.Send(new GetStudentDashboardOrchestrator(studentId), cancellationToken);

            if (!dashboard.Success)
                return RequestResult<GetStudentDashboardResponse>
                .Failure(dashboard.Result, ResultCode.StudentsDashoardQueryFalied);

            _cache.Set(cacheKey, dashboard.Result, CacheDuration); 
            return RequestResult<GetStudentDashboardResponse>.succeeded(dashboard.Result,dashboard.Code);

        }
    }
}