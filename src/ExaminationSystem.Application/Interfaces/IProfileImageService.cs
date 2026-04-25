namespace ExaminationSystem.Application.Interfaces;

public interface IProfileImageService
{
    Task<string> SaveAsync(Guid userId, byte[] fileBytes, string extension, CancellationToken cancellationToken);
    Task DeleteAsync(string? imageUrl, CancellationToken cancellationToken);
}
