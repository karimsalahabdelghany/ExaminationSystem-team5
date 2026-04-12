using ExaminationSystem.Domain.Entities;
using System.Linq.Expressions;

namespace ExaminationSystem.Application.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    IQueryable<T> GetAll();
    IQueryable<T> GetAll(Expression<Func<T, bool>> predicate);
    T Add(T entity);
    void AddRange(IEnumerable<T> entities);

    Task PatchAsync(
        T entity,
        CancellationToken ct = default,
        params Expression<Func<T, object>>[] updatedProperties);

    Task DeleteAsync(T entity, CancellationToken ct = default);
    void Update(T entity);
    Task<T?> GetByIdAsync(Guid id);
    Task<T?> GetByIdWithNoTracking(Guid id);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FindAsync(Expression<Func<T, bool>> predicate);
}
