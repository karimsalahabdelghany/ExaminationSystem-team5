using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Features.Admin.Queries
{
    public record GetTotalUsersQuery :IQuery<RequestResult<int>>;
    public class GetTotalUsersQueryHandler : IRequestHandler<GetTotalUsersQuery, RequestResult<int>>
    {
        private readonly UserManager<Domain.Entities.AppUser> _userManager;

        public GetTotalUsersQueryHandler(UserManager<Domain.Entities.AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<RequestResult<int>> Handle(
            GetTotalUsersQuery request,
            CancellationToken cancellationToken)
        {
            var Count = await _userManager.Users.CountAsync<ExaminationSystem.Domain.Entities.AppUser>(cancellationToken);
           return RequestResult<int>.succeeded(Count, ResultCode.GetTotalUsersQuerySuccessed);
        }
    }

}
