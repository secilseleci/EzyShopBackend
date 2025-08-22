using Models.Entities.Abstract;
using System.Linq.Expressions;

namespace DataAccess.Repositories.Abstract;
public interface IBaseRepository<T> where T : class, IBaseEntity, IAuditable
{
    Task<T?> GetByIdAsync(Guid id);
    Task<T?> GetAsync(Expression<Func<T, bool>> predicate);

    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
   
    Task<int> CreateAsync(T entity);
    Task<int> CreateRangeAsync(IEnumerable<T> entities);

    Task<int> SoftDeleteAsync(Guid Id);
    Task<int> SoftDeleteRangeAsync(IEnumerable<T> entities);

    Task<int> UpdateAsync(T entity);
    Task<int> UpdateRangeAsync(IEnumerable<T> entities);
}
