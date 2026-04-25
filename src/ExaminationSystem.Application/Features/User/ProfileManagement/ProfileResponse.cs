namespace ExaminationSystem.Application.Features.User.ProfileManagement;

public record ProfileResponse(
    Guid Id,
    string FullName,
    string Email,
    string? PendingEmail,
    string? PhoneNumber,
    string? ProfileImageUrl,
    string Status
);
