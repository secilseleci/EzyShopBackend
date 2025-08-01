using Models.Entities.Concrete;

namespace DataAccess.Repositories.Abstract;

public interface IShopRepository : IBaseRepository<Shop>
{
    Task<Guid?> GetActiveShopIdByUserIdAsync(Guid userId);
    Task<Guid?> GetShopIdByUserIdAsync(Guid userId);
}
