using DataAccess.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Models.Entities.Abstract;
using System.Linq.Expressions;

namespace DataAccess.Repositories.Concrete;
public class BaseRepository<T> : IBaseRepository<T> where T : class, IBaseEntity, IAuditable, new()
{
    protected readonly ApplicationDbContext _dataContext;
    protected readonly DbSet<T> _dbSet;

    public BaseRepository(ApplicationDbContext context)
    {
        _dataContext = context;
        _dbSet = _dataContext.Set<T>();
    }
    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
    }
    public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(x => !x.IsDeleted).FirstOrDefaultAsync(predicate);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.Where(x => !x.IsDeleted).ToListAsync();
    }
    public async Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).Where(x => !x.IsDeleted).ToListAsync();
    }
    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(x => !x.IsDeleted).AnyAsync(predicate);
    }

    public async Task<int> CreateAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return await _dataContext.SaveChangesAsync();

    }
    public async Task<int> CreateRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
        return await _dataContext.SaveChangesAsync();
    }

    public async Task<int> SoftDeleteAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity is null)
            return -1;

        _dbSet.Remove(entity);
        return await _dataContext.SaveChangesAsync();

    }
    public async Task<int> SoftDeleteRangeAsync(IEnumerable<T> entities)
    {
        foreach (var entity in entities)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.IsActive = false;
            _dbSet.Update(entity);
        }
        return await _dataContext.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(T entity)
    {
        _dataContext.Update(entity);
        return await _dataContext.SaveChangesAsync();
    }
    public async Task<int> UpdateRangeAsync(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
        return await _dataContext.SaveChangesAsync();
    }

}