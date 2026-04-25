using ExaminationSystem.Application.Common.Options;
using ExaminationSystem.Application.Common.Results;
using ExaminationSystem.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ExaminationSystem.Application.Features.Auth.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : ICommand<RequestResult<RefreshTokenResponse>>;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RequestResult<RefreshTokenResponse>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenServies _tokenServies;
    private readonly IUnitOfWork _unitOfWork;
    private readonly JwtOptions _jwtOptions;

    public RefreshTokenCommandHandler(
        UserManager<AppUser> userManager,
        ITokenServies tokenServies,
        IUnitOfWork unitOfWork,
        IOptions<JwtOptions> jwtOptions)
    {
        _userManager = userManager;
        _tokenServies = tokenServies;
        _unitOfWork = unitOfWork;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<RequestResult<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var hash = _tokenServies.HashRefreshToken(request.RefreshToken);

        var stored = await _unitOfWork.Repository<Domain.Entities.RefreshToken>()
            .FindAsync(t => t.TokenHash == hash);

        if (stored is null)
            return RequestResult<RefreshTokenResponse>.Failure(null!, ResultCode.RefreshTokenInvalid);

        if (stored.IsRevoked)
            return RequestResult<RefreshTokenResponse>.Failure(null!, ResultCode.RefreshTokenRevoked);

        if (stored.ExpiresAt <= DateTime.UtcNow)
            return RequestResult<RefreshTokenResponse>.Failure(null!, ResultCode.RefreshTokenExpired);

        var user = await _userManager.FindByIdAsync(stored.UserId.ToString());
        if (user is null || user.Status != AccountStatus.Active)
            return RequestResult<RefreshTokenResponse>.Failure(null!, ResultCode.RefreshTokenInvalid);

        stored.IsRevoked = true;
        _unitOfWork.Repository<Domain.Entities.RefreshToken>().Update(stored);

        var accessToken = await _tokenServies.CreateToken(user);
        var rawRefresh = _tokenServies.GenerateRefreshToken();
        var refreshHash = _tokenServies.HashRefreshToken(rawRefresh);

        var refreshExpiry = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenDays);
        var newToken = new Domain.Entities.RefreshToken(user.Id, refreshHash, refreshExpiry);
        newToken.Id = Guid.CreateVersion7();
        _unitOfWork.Repository<Domain.Entities.RefreshToken>().Add(newToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var accessExpiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenMinutes);
        var response = new RefreshTokenResponse(accessToken, rawRefresh, accessExpiresAt);

        return RequestResult<RefreshTokenResponse>.succeeded(response, ResultCode.TokenRefreshedSuccessfully);
    }
}
