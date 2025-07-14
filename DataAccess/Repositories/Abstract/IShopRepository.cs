using Core.Constants;
using Core.Pagination;
using Models.DTOs;
using Models.Entities.Concrete;

namespace DataAccess.Repositories.Abstract;

public interface IShopRepository : IBaseRepository<Shop>
{
    Task<PaginatedList<ShopListDto>> GetShopDtosAsync(ShopStatus status, string? searchTerm, int page, int pageSize);

    Task<ShopDetailsDto> GetShopDetailsDtosAsync(Guid shopId);

    Task<decimal> CountPendingShopsAsync(ShopStatus status);
    Task<decimal> CountActiveShopsAsync(ShopStatus status);
    Task<Guid?> GetActiveShopIdByUserIdAsync(Guid userId);

}
