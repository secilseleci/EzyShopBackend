using DataAccess.Repositories.Abstract;
using Models.Entities.Concrete;

namespace DataAccess.Repositories.Concrete;

public class CategoryRepository(ApplicationDbContext context) : BaseRepository<Category>(context), ICategoryRepository
{
}
