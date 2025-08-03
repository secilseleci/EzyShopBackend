using Core.Pagination;
using Models.DTOs.Seller;
using Models.Entities.Concrete;

namespace DataAccess.Repositories.Abstract;

public interface ISellerRepository : IBaseRepository<Seller>
{
    Task<Seller?> GetActiveSellerByUserIdAsync(Guid userId);
    Task<PaginatedList<SellerListItemDto>> GetFilteredSellerListAsync(SellerFilterDto filter);
    Task<SellerProfileDto?> GetOwnProfileAsync(Guid sellerId);
}