using ExaminationSystem.Domain.Entities;
using System.Linq.Expressions;

namespace ExaminationSystem.Application.Interfaces;

public interface IRepository<T> where T : BaseEntity, new()
{
    IQueryable<T> GetAll();
    IQueryable<T> GetAll(Expression<Func<T, bool>> predicate);
    void Add(T entity);
    void AddRange(IEnumerable<T> entities);

    Task PatchAsync(
        T entity,
        CancellationToken ct = default,
        params Expression<Func<T, object>>[] updatedProperties);

    Task DeleteAsync(T entity, CancellationToken ct = default);
}
