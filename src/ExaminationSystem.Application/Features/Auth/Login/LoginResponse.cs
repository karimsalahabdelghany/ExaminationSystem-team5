namespace ExaminationSystem.Application.Features.Auth.Login;

public record LoginResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt
);
