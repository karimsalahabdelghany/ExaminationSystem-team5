using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace ExaminationSystem.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationContext _context;
    private readonly Dictionary<Type, object> _repositories = new();
    private bool _disposed;

    public UnitOfWork(ApplicationContext context) => _context = context;

    // Expose for behavior to use directly
    public bool HasActiveTransaction
        => _context.Database.CurrentTransaction is not null;

    public IRepository<T> Repository<T>() where T : BaseEntity, new()
    {
        var type = typeof(T);
        if (!_repositories.TryGetValue(type, out var repo))
        {
            repo = new Repository<T>(_context);
            _repositories[type] = repo;
        }
        return (IRepository<T>)repo;
    }

    // Stages all tracked changes — behavior decides when to call this
    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        try
        {
            return await _context.SaveChangesAsync(ct);
        }
        catch (Exception)
        {
            throw; // interceptors already translated the exception
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            await _context.DisposeAsync();
            _disposed = true;
        }
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default)
    {
        return _context.Database
                .BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);
    }
}