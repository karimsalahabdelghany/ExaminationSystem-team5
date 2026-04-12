using ExaminationSystem.Application.Interfaces;
using System.Linq.Expressions;

namespace ExaminationSystem.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity, new()
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
        entity.Id = Guid.CreateVersion7();
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

    public async Task PatchAsync(
       T entity,
       CancellationToken ct = default,
       params Expression<Func<T, object>>[] updatedProperties)
    {
        _dbSet.Attach(entity);
        var entry = _context.Entry(entity);
        foreach (var prop in updatedProperties)
            entry.Property(prop).IsModified = true;
        await _context.SaveChangesAsync(ct); // ← interceptors fire automatically
    }

    public async Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
            _dbSet.Attach(entity);
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync(ct); // ← interceptors fire automatically
    }
}
