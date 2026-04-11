using ExaminationSystem.Application.Interfaces;
using System.Linq.Expressions;

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
        entity.Id = Guid.CreateVersion7();
        _dbSet.Add(entity);
        return entity;
    }

    public IQueryable<T> Get()
    {
        return _dbSet;
    }

    public IQueryable<T> Get(Expression<Func<T, bool>> predicate)
    {
        return _dbSet.Where(predicate);
    }
}
