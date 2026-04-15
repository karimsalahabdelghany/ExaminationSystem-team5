using ExaminationSystem.Application.Interfaces;

namespace ExaminationSystem.API.Services;

public sealed class UtcDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
