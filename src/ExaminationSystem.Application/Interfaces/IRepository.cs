using ExaminationSystem.Domain.Entities;
using System.Linq.Expressions;

namespace ExaminationSystem.Application.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    IQueryable<T> Get();
    IQueryable<T> Get(Expression<Func<T, bool>> predicate);
    T Add(T entity);
}
