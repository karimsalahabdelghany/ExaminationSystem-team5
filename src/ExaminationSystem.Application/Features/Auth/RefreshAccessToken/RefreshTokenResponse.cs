namespace ExaminationSystem.Application.Features.Auth.RefreshAccessToken;

public record RefreshTokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt
);
