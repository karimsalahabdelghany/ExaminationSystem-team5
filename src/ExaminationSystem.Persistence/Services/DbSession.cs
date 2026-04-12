using ExaminationSystem.Application.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace ExaminationSystem.Persistence.Services;

public class DbSession : IDbSession
{
    private readonly ApplicationContext _context;
    private IDbContextTransaction? _transaction;

    public DbSession(ApplicationContext context) => _context = context;

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);

    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        _transaction ??= await _context.Database
            .BeginTransactionAsync(ct);
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
}