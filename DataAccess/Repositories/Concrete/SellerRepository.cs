using DataAccess.Repositories.Abstract;
using Models.Entities.Concrete;

namespace DataAccess.Repositories.Concrete;

public class SellerRepository(ApplicationDbContext context) : BaseRepository<Seller>(context), ISellerRepository
{
    public async Task<Seller?> GetActiveSellerByUserIdAsync(Guid userId)
    {
        var result = await GetWhereAsync(s => s.Id == userId && s.IsActive);      
            return result?.FirstOrDefault();
    }
}