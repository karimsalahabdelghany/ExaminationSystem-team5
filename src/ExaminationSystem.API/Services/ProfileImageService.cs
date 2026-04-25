using ExaminationSystem.Application.Interfaces;

namespace ExaminationSystem.API.Services;

public class ProfileImageService(IWebHostEnvironment environment) : IProfileImageService
{
    private const string UploadFolder = "uploads/profile-images";

    public async Task<string> SaveAsync(Guid userId, byte[] fileBytes, string extension, CancellationToken cancellationToken)
    {
        var safeExtension = extension.StartsWith('.') ? extension : $".{extension}";
        var fileName = $"{userId:N}_{Guid.NewGuid():N}{safeExtension.ToLowerInvariant()}";
        var relativePath = Path.Combine(UploadFolder, fileName).Replace("\\", "/");
        var rootPath = environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot");
        var fullDirectory = Path.Combine(rootPath, UploadFolder);
        Directory.CreateDirectory(fullDirectory);

        var fullFilePath = Path.Combine(rootPath, relativePath);
        await File.WriteAllBytesAsync(fullFilePath, fileBytes, cancellationToken);

        return "/" + relativePath;
    }

    public Task DeleteAsync(string? imageUrl, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return Task.CompletedTask;

        var relativePath = imageUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
        var rootPath = environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot");
        var fullFilePath = Path.Combine(rootPath, relativePath);

        if (File.Exists(fullFilePath))
            File.Delete(fullFilePath);

        return Task.CompletedTask;
    }
}
