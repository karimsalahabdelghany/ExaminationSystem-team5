using ExaminationSystem.Domain.Entities;
using System.Linq.Expressions;

namespace ExaminationSystem.Application.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    IQueryable<T> Get();
    IQueryable<T> Get(Expression<Func<T, bool>> predicate);
    T Add(T entity);
    void Update(T entity);
    Task<T?> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FindAsync(Expression<Func<T, bool>> predicate);
}
