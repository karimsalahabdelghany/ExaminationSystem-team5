using ExaminationSystem.Domain.Entities;
using System.Linq.Expressions;

namespace ExaminationSystem.Application.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    IQueryable<T> GetAll();
    IQueryable<T> GetAll(Expression<Func<T, bool>> predicate);
    T Add(T entity);
    void AddRange(IEnumerable<T> entities);

    void Patch(
        T entity,
        CancellationToken ct = default,
        params Expression<Func<T, object>>[] updatedProperties);

    void Delete(T entity, CancellationToken ct = default);
    void Delete(T entity);
    void Update(T entity);
    Task<T?> GetByIdAsync(Guid id);
    Task<T?> GetByIdWithNoTracking(Guid id);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FindAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
    Task<int> CountAsync();
    // Pushes SELECT COUNT(DISTINCT col) to SQL — used for active_users_today handler
    Task<int> CountDistinctAsync<TKey>(
        Expression<Func<T, bool>> filter,
        Expression<Func<T, TKey>> selector,
        CancellationToken ct = default);

    void SaveInclude(T entity, params string[] includedProperties);


}
