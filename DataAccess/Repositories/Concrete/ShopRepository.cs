using DataAccess.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Models.Entities.Concrete;

namespace DataAccess.Repositories.Concrete;
public class ShopRepository(ApplicationDbContext context) : BaseRepository<Shop>(context), IShopRepository
{
    public async Task<Guid?> GetActiveShopIdByUserIdAsync(Guid userId)
    {
        var shop = await _dataContext.Shops
                 .Where(s => s.SellerId == userId && s.IsActive && !s.IsDeleted)
                 .FirstOrDefaultAsync();

        return shop?.Id;
    }
    public async Task<Guid?> GetShopIdByUserIdAsync(Guid userId)
    {
        var shop = await _dataContext.Shops
                        .Where(s => s.SellerId == userId && !s.IsDeleted)
                        .FirstOrDefaultAsync();

        return shop?.Id;
    }
}