using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace ExaminationSystem.Application.Services
{
    // Reads JWT claims from HttpContext — injected as Scoped
    
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

            var userIdString = _httpContextAccessor.HttpContext?
                .User?.FindFirst("UserId")?.Value;

            if (Guid.TryParse(userIdString, out Guid userId))
                Id = userId;
            else
                Id = Guid.Parse("019b280b-92cc-7715-8e04-f21087b2c9db");
        }

        public Guid? Id { get; set; }

        public string? Email
            => _httpContextAccessor.HttpContext?
                .User?.FindFirstValue(ClaimTypes.Email);

        public string? Role
            => _httpContextAccessor.HttpContext?
                .User?.FindFirstValue(ClaimTypes.Role);

        public bool IsAuthenticated
            => _httpContextAccessor.HttpContext?
                .User?.Identity?.IsAuthenticated ?? false;

    }

}
