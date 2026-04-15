using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Features.Admin.Queries
{
    // DEPENDS ON: POST /api/auth/login — writes LoginLogs on every successful login
    public record GetActiveUsersTodayQuery : IQuery<int>;
    public class GetActiveUsersTodayQueryHandler :
        IRequestHandler<GetActiveUsersTodayQuery, int>
    {
        private readonly IRepository<LoginLog> _loginLogRepo;

        public GetActiveUsersTodayQueryHandler(IRepository<LoginLog> loginLogRepo)
        {
            _loginLogRepo = loginLogRepo;
        }
        public async Task<int> Handle(GetActiveUsersTodayQuery request, CancellationToken cancellationToken)
        {
            var today = DateTime.UtcNow.Date;

           return await _loginLogRepo.CountDistinctAsync(l => l.Success &&  l.CreatedAt >= today, l => l.UserId);
        }
    }
}
