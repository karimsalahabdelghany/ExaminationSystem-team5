using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace ExaminationSystem.Persistence.Services;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationContext _context;
    private IDbContextTransaction? _transaction;
    private readonly Dictionary<Type, object> _repositories = new();

    public UnitOfWork(ApplicationContext context) => _context = context;

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);

    public async Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken ct = default)
    {
        _transaction ??= await _context.Database
            .BeginTransactionAsync(isolationLevel, ct);
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
        await _transaction!.CommitAsync(ct);
        await DisposeTransactionAsync();
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        await _transaction!.RollbackAsync(ct);
        await DisposeTransactionAsync();
    }

    private async Task DisposeTransactionAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeTransactionAsync();
        await _context.DisposeAsync();
    }

    public IRepository<T> Repository<T>() where T : BaseEntity
    {
        var type = typeof(T);
        if (!_repositories.TryGetValue(type, out var repo))
        {
            repo = new Repository<T>(_context);
            _repositories[type] = repo;
        }
        return (IRepository<T>)repo;
    }
}