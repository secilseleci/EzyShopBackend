using Core.Pagination;
using Core.Utilities.Results;
using Microsoft.EntityFrameworkCore.Storage;
using Models.Entities.Abstract;
using System.Linq.Expressions;

namespace DataAccess.Repositories.Abstract;

public interface IBaseRepository<T> where T : class, IBaseEntity, IAuditable
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate);

    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task<PaginatedList<TResult>> GetPaginatedAsync<TResult>(IQueryable<TResult> query, int page, int pageSize);
     
    Task<int> CreateAsync(T entity);
    Task<int> CreateRangeAsync(IEnumerable<T> entities);

    Task<int> SoftDeleteAsync(Guid Id);
    Task<int> SoftDeleteRangeAsync(IEnumerable<T> entities);
  
    Task<int> UpdateAsync(T entity);
    Task<int> UpdateRangeAsync(IEnumerable<T> entities);
}
