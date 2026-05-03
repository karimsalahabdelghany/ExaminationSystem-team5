using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ExaminationSystem.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly ApplicationContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(ApplicationContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public T Add(T entity)
    {
        if (entity.Id == Guid.Empty)
        {
            entity.Id = Guid.CreateVersion7();
        }
        _dbSet.Add(entity);
        return entity;
    }

    public void AddRange(IEnumerable<T> entities)
    {
        _dbSet.AddRange(entities);
    }

    public IQueryable<T> GetAll()
    {
        return _dbSet;
    }

    public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate)
    {
        return _dbSet.Where(predicate);
    }

    public void Patch(
       T entity,
       CancellationToken ct = default,
       params Expression<Func<T, object>>[] updatedProperties)
    {
        _dbSet.Attach(entity);
        var entry = _context.Entry(entity);
        foreach (var prop in updatedProperties)
            entry.Property(prop).IsModified = true;
    }

    public void Delete(T entity, CancellationToken ct = default)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
            _dbSet.Attach(entity);
        _dbSet.Remove(entity);
    }

    public void Update(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    public Task<T?> GetByIdWithNoTracking(Guid id)
    {
        return _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
    }

    public void SaveInclude(T entity, params string[] includedProperties)
    {
        var localEntity = _dbSet.Local.FirstOrDefault(e => e.Id == entity.Id);
        EntityEntry entry;
        if (localEntity == null)
        {
            _dbSet.Attach(entity);
            entry = _context.Entry(entity);
        }
        else
        {
            entry = _context.Entry(localEntity);
            _context.Entry(localEntity).CurrentValues.SetValues(entity);
        }
        foreach (var property in entry.Properties)
        {
            if (property.Metadata.IsPrimaryKey())
                continue;
            property.IsModified = includedProperties.Contains(property.Metadata.Name);
        }
    }

    public void Delete(T entity)
    {
        entity.DeletedAt = DateTime.UtcNow;
        entity.IsDeleted = true;
        SaveInclude(entity, nameof(entity.DeletedAt), nameof(entity.IsDeleted));
    }
    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
    {
        return predicate is null
            ? await _dbSet.CountAsync(ct)
            : await _dbSet.CountAsync(predicate, ct);
    }
    public async Task<int> CountAsync()
    {
        return await _dbSet.CountAsync();
    }
    // Translates to: SELECT COUNT(DISTINCT [selector]) FROM [T] WHERE [filter]
    // Used by GetActiveUsersTodayQueryHandler — never loads rows into memory
    public async Task<int> CountDistinctAsync<TKey>(
        Expression<Func<T, bool>> filter,
        Expression<Func<T, TKey>> selector,
        CancellationToken ct = default)
        => await _dbSet
               .Where(filter)
               .Select(selector)
               .Distinct()
               .CountAsync(ct);
}

