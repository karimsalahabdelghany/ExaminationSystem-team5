using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Entities;
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
        private readonly UserManager<User> _userManager;

        public GetTotalUsersQueryHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<int> Handle(
            GetTotalUsersQuery request,
            CancellationToken cancellationToken)
            => await  _userManager.Users.CountAsync<User>();
    }

}
