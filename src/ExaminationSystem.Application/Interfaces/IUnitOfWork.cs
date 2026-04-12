using ExaminationSystem.Domain.Entities;

namespace ExaminationSystem.Application.Interfaces;

public interface IUnitOfWork
{
    IRepository<T> Repository<T>() where T : BaseEntity, new();
    ValueTask DisposeAsync();
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    bool HasActiveTransaction { get; }
}
