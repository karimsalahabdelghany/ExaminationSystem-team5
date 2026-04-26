using ExaminationSystem.Application.Common.Options;
using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ExaminationSystem.Application.Features.Auth.Login;

public record LoginCommand(string Email, string Password) : ICommand<RequestResult<LoginResponse>>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, RequestResult<LoginResponse>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenServies _tokenServies;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JwtOptions _jwtOptions;

    public LoginCommandHandler(
        UserManager<AppUser> userManager,
        ITokenServies tokenServies,
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        IOptions<JwtOptions> jwtOptions)
    {
        _userManager = userManager;
        _tokenServies = tokenServies;
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<RequestResult<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return RequestResult<LoginResponse>.Failure(null!, ResultCode.InvalidCredentials);

        if (user.LockedUntil.HasValue && user.LockedUntil.Value > DateTime.UtcNow)
            return RequestResult<LoginResponse>.Failure(null!, ResultCode.AccountLockedTemporarily);

        if (user.LockedUntil.HasValue && user.LockedUntil.Value <= DateTime.UtcNow)
        {
            user.FailedLoginAttempts = 0;
            user.LockedUntil = null;
        }

        var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "unknown";

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
        {
            user.FailedLoginAttempts++;
            if (user.FailedLoginAttempts >= 5)
                user.LockedUntil = DateTime.UtcNow.AddMinutes(15);

            await _userManager.UpdateAsync(user);

            var failedLog = new LoginLog(user.Id, ipAddress, false);
            failedLog.Id = Guid.CreateVersion7();
            _unitOfWork.Repository<LoginLog>().Add(failedLog);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<LoginResponse>.Failure(null!, ResultCode.InvalidCredentials);
        }

        if (user.Status == AccountStatus.Locked)
            return RequestResult<LoginResponse>.Failure(null!, ResultCode.AccountLockedByAdmin);

        if (user.Status != AccountStatus.Active)
            return RequestResult<LoginResponse>.Failure(null!, ResultCode.AccountNotActive);

        user.FailedLoginAttempts = 0;
        user.LockedUntil = null;

        var accessToken = await _tokenServies.CreateToken(user);
        var rawRefresh = _tokenServies.GenerateRefreshToken();
        var refreshHash = _tokenServies.HashRefreshToken(rawRefresh);

        var refreshExpiry = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenDays);
        var refreshToken = new RefreshToken(user.Id, refreshHash, refreshExpiry);
        refreshToken.Id = Guid.CreateVersion7();
        _unitOfWork.Repository<RefreshToken>().Add(refreshToken);

        var successLog = new LoginLog(user.Id, ipAddress, true);
        successLog.Id = Guid.CreateVersion7();
        _unitOfWork.Repository<LoginLog>().Add(successLog);

        await _userManager.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var accessExpiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenMinutes);
        var response = new LoginResponse(accessToken, rawRefresh, accessExpiresAt);

        return RequestResult<LoginResponse>.succeeded(response, ResultCode.LoginSucceeded);
    }
}
