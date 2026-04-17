using ExaminationSystem.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Features.Admin.Queries
{
    public record GetTotalUsersQuery : IQuery<int>;
    public class GetTotalUsersQueryHandler : IRequestHandler<GetTotalUsersQuery, int>
    {
        private readonly UserManager<Domain.Entities.AppUser> _userManager;

        public GetTotalUsersQueryHandler(UserManager<Domain.Entities.AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<int> Handle(
            GetTotalUsersQuery request,
            CancellationToken cancellationToken)
            => await  _userManager.Users.CountAsync<ExaminationSystem.Domain.Entities.AppUser>();
    }

}
