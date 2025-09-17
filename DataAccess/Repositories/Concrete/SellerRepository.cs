using Core.Pagination;
using DataAccess.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Models.DTOs.Seller;
using Models.Entities.Concrete;
using System.Numerics;

namespace DataAccess.Repositories.Concrete;
public class SellerRepository(ApplicationDbContext context) : BaseRepository<Seller>(context), ISellerRepository
{
    public async Task<PaginatedList<SellerListItemDto>> GetFilteredSellerListAsync(SellerFilterDto filter)
    {
        var query = from s in _dataContext.Sellers
                    join sh in _dataContext.Shops
                        on s.Id equals sh.SellerId
                    where !s.IsDeleted
                    select new SellerListItemDto
                    {
                        SellerId = s.Id,
                        FirstName = s.FirstName,
                        LastName = s.LastName,
                        Phone = s.Phone,
                        Status = s.Status,
                        CreatedDate = s.CreatedAt,
                        ShopName = sh.Name
                    };

        // Search
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            query = query.Where(x =>
                x.FirstName.Contains(filter.SearchTerm) ||
                x.LastName.Contains(filter.SearchTerm) ||
                x.Phone.Contains(filter.SearchTerm) ||
                x.ShopName.Contains(filter.SearchTerm));
        }

        // Status Filter
        if (filter.Status.HasValue)
        {
            query = query.Where(x => x.Status == filter.Status.Value);
        }

        // Sorting
        query = filter.SortBy?.ToLower() switch
        {
            "firstname" => filter.Descending ? query.OrderByDescending(x => x.FirstName) : query.OrderBy(x => x.FirstName),
            "lastname" => filter.Descending ? query.OrderByDescending(x => x.LastName) : query.OrderBy(x => x.LastName),
            "createddate" or _ => filter.Descending ? query.OrderByDescending(x => x.CreatedDate) : query.OrderBy(x => x.CreatedDate)
        };

        // Pagination
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .AsNoTracking()
            .ToListAsync();

        return new PaginatedList<SellerListItemDto>(items, totalCount, filter.Page, filter.PageSize);
    }
    public async Task<SellerProfileDto?> GetOwnProfileAsync(Guid sellerId)
    {
        var result = await (from seller in _dataContext.Sellers
                            join user in _dataContext.Users on seller.Id equals user.Id
                            join shop in _dataContext.Shops on seller.Id equals shop.SellerId
                            join plan in _dataContext.SubscriptionPlans on seller.SubscriptionPlanId equals plan.Id
                            where seller.Id == sellerId && !seller.IsDeleted
                            select new SellerProfileDto
                            {
                                SubscriptionPlanId = seller.SubscriptionPlanId,
                                SubscriptionPlanName = plan.Name,
                                SellerId = seller.Id,
                                FirstName = seller.FirstName,
                                LastName = seller.LastName,
                                Phone = user.PhoneNumber!,
                                Email = user.Email!,
                                Address = shop.Address,
                                ShopName=shop.Name,
                                TaxNumber=shop.TaxNumber,
                                CreatedDate = seller.CreatedAt,
                                DeletedDate = seller.DeletedAt,
                                UpdatedDate = seller.UpdatedAt
                            })
                           .AsNoTracking()
                           .FirstOrDefaultAsync();

        return result;
    }
}