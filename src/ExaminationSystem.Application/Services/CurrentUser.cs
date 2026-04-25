using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ExaminationSystem.Application.Services
{
    // Reads JWT claims from HttpContext — injected as Scoped

    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? Id
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                if (user is null) return null;

                var claim = user.FindFirst("user_id")?.Value
                            ?? user.FindFirst("UserId")?.Value
                            ?? user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                            ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (Guid.TryParse(claim, out var id))
                    return id;

                return null;
            }
        }

        public string? Email
            => _httpContextAccessor.HttpContext?
                .User?.FindFirstValue(ClaimTypes.Email);

        public string? Role
            => _httpContextAccessor.HttpContext?
                .User?.FindFirstValue(ClaimTypes.Role);

        public bool IsInRole(string role)
            => !string.IsNullOrWhiteSpace(role)
               && (_httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false);

        public bool IsAuthenticated
            => _httpContextAccessor.HttpContext?
                .User?.Identity?.IsAuthenticated ?? false;

        public bool TryGetUserId(out Guid userId)
        {
            userId = Guid.Empty;
            var id = Id;
            if (!id.HasValue)
                return false;

            userId = id.Value;
            return true;
        }

        public Guid GetRequiredUserId()
        {
            if (!TryGetUserId(out var userId))
                throw new UnauthorizedAccessException("Current user is not authenticated or user id claim is missing.");

            return userId;
        }

    }

}
