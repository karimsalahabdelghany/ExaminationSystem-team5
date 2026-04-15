using ExaminationSystem.Domain.Entities;
using ExaminationSystem.Domain.Interfaces;

using System.Data;

namespace ExaminationSystem.Application.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
    IRepository<T> Repository<T>() where T : BaseEntity;
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken ct = default);
    Task CommitAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);
}
