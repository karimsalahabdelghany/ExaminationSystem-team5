namespace ExaminationSystem.Application.Interfaces;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
