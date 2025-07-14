using Models.Entities.Concrete;

namespace DataAccess.Repositories.Abstract;

public interface ISellerRepository : IBaseRepository<Seller>
{
    Task<Seller?> GetActiveSellerByUserIdAsync(Guid userId);
}