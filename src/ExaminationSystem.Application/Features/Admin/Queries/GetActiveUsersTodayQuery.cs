using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Features.Admin.Queries
{
    // DEPENDS ON: POST /api/auth/login — writes LoginLogs on every successful login
    public record GetActiveUsersTodayQuery : IQuery<RequestResult<int>>;
    public class GetActiveUsersTodayQueryHandler :
        IRequestHandler<GetActiveUsersTodayQuery, RequestResult<int>>
    {
        private readonly IRepository<LoginLog> _loginLogRepo;

        public GetActiveUsersTodayQueryHandler(IRepository<LoginLog> loginLogRepo)
        {
            _loginLogRepo = loginLogRepo;
        }
        public async Task <RequestResult<int>> Handle(GetActiveUsersTodayQuery request, CancellationToken cancellationToken)
        {
            var today = DateTime.UtcNow.Date;

           var result = await _loginLogRepo.CountDistinctAsync(l => l.Success &&  l.CreatedAt >= today, l => l.UserId);
           return RequestResult<int>.succeeded(result, ResultCode.UsersLoginTodaySuccessfully);
        }
    }
}
