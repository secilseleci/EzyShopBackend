using DataAccess.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Models.Entities.Concrete;
using System.Linq.Expressions;

namespace DataAccess.Repositories.Concrete;

public class CategoryRepository(ApplicationDbContext context) : BaseRepository<Category>(context), ICategoryRepository
{
    public async Task<IEnumerable<Category>> GetAllCategoriesAsync(Expression<Func<Category, bool>> predicate)
    {
        return await _dataContext.Categories
            .Where(predicate)
            .Where(x => !x.IsDeleted)
            .ToListAsync();
    }
}
