using Core.Constants;
using Core.Pagination;
using DataAccess.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Models.DTOs;
using Models.Entities.Concrete;
using System.Linq.Expressions;

namespace DataAccess.Repositories.Concrete;

public class ShopRepository(ApplicationDbContext context) : BaseRepository<Shop>(context), IShopRepository
{
    public async Task<PaginatedList<ShopListDto>> GetShopDtosAsync(ShopStatus status, string? searchTerm, int page, int pageSize)
    {
        var filteredShops = _dataContext.Shops.Where(GetStatusFilter(status));

        var query = from shop in filteredShops
                    join seller in _dataContext.Sellers on shop.SellerId equals seller.Id

                    where (string.IsNullOrEmpty(searchTerm) ||
                        shop.Name.Contains(searchTerm) ||
                        seller.FirstName.Contains(searchTerm) ||
                        seller.LastName.Contains(searchTerm)
                          )
                    select new ShopListDto
                    {
                        SellerId = seller.Id,
                        Id = shop.Id,
                        Name = shop.Name,
                        SellerName = seller.FirstName + " " + seller.LastName,
                    };

        var totalItems = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedList<ShopListDto>(items, totalItems, page, pageSize);
    }
    public async Task<ShopDetailsDto> GetShopDetailsDtosAsync(Guid shopId)
    {
        var result = await (from shop in _dataContext.Shops
                            join seller in _dataContext.Sellers on shop.SellerId equals seller.Id
                            join user in _dataContext.Users on seller.Id equals user.Id

                            where shop.Id == shopId

                            select new ShopDetailsDto
                            {
                                SellerId = seller.Id,
                                Id = shop.Id,
                                Name = shop.Name,
                                SellerName = seller.FirstName + " " + seller.LastName,
                                Phone = seller.Phone,
                                TaxNumber = shop.TaxNumber,
                                Address = shop.Address,
                                CreatedAt = shop.CreatedAt,
                                IsActive = shop.IsActive,
                                Email = user.Email!
                            })
                    .FirstOrDefaultAsync();

        return result!;
    }
    public async Task<decimal> CountPendingShopsAsync(ShopStatus status)
    {
        return await _dataContext.Shops.Where(GetStatusFilter(ShopStatus.Pending))
            .LongCountAsync();
    }
    public async Task<decimal> CountActiveShopsAsync(ShopStatus status)
    {
        return await _dataContext.Shops.Where(GetStatusFilter(ShopStatus.Active))
                   .LongCountAsync();
    }
    public async Task<Guid?> GetActiveShopIdByUserIdAsync(Guid userId)
    {
        var shop = await _dataContext.Shops
          .Where(s => s.SellerId == userId && s.IsActive && !s.IsDeleted)
          .FirstOrDefaultAsync();

        return shop?.Id;
    }
    private static Expression<Func<Shop, bool>> GetStatusFilter(ShopStatus status)
    {
        return status switch
        {
            ShopStatus.Pending => shop => !shop.IsDeleted && !shop.IsActive && shop.UpdatedAt == null,
            ShopStatus.Active => shop => !shop.IsDeleted && shop.IsActive,
            ShopStatus.Inactive => shop => !shop.IsDeleted && !shop.IsActive && shop.UpdatedAt != null,
            ShopStatus.Deleted => shop => shop.IsDeleted,
            _ => shop => false
        };
    }
}