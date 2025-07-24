using Core.Constants;
using Core.Pagination;
using DataAccess.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Models.DTOs.Product;
using Models.Entities.Concrete;
using System.Linq.Expressions;

namespace DataAccess.Repositories.Concrete;

public class ProductRepository(ApplicationDbContext context) : BaseRepository<Product>(context), IProductRepository
{
    public async Task<PaginatedList<ProductListForSellerDto>> GetProductDtosAsync(ProductStatus status, Guid shopId, string? searchTerm, int page, int pageSize)
    {

        var filteredProducts = _dataContext.Products.Where(GetStatusFilter(status, shopId));

        var query =
            from product in filteredProducts
            join category in _dataContext.Categories on product.CategoryId equals category.Id

            where
            (string.IsNullOrEmpty(searchTerm) ||
                product.Name.Contains(searchTerm) ||
                category.Name.Contains(searchTerm)
             )
            select new ProductListForSellerDto
            {
                ProductId = product.Id,
                CategoryId = category.Id,
                ProductName = product.Name,
                CategoryName = category.Name,
                ImageUrl = product.ImageUrl,
            };

        var totalItems = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedList<ProductListForSellerDto>(items, totalItems, page, pageSize);
    }
    public async Task<ProductDetailsForSellerDto> GetProductDetailsDtosForSellerAsync(Guid shopId, Guid productId)
    {

        var result = await (from p in _dataContext.Products
                            join c in _dataContext.Categories
                                on p.CategoryId equals c.Id
                            where p.Id == productId && p.ShopId == shopId
                            select new ProductDetailsForSellerDto
                            {
                                CategoryName = c.Name,
                                ProductName = p.Name,
                                ImageUrl = p.ImageUrl,
                                Price = p.Price,
                                Color = p.Color,
                                Stock = p.Stock
                            })
                  .FirstOrDefaultAsync();

        return result!;
    }
    public async Task<PaginatedList<ProductListForCustomerDto>> GetFilteredProductDtosAsync(ProductFilterViewModel model)
    {
        var productFilter = GetAvailableForCustomerFilter();
        var productQuery = _dataContext.Products.Where(productFilter);

        var categoryQuery = _dataContext.Categories.AsQueryable();
        if (!string.IsNullOrEmpty(model.Category))
            categoryQuery = categoryQuery.Where(c => c.Name == model.Category);

        var query = from p in productQuery
                    join c in categoryQuery on p.CategoryId equals c.Id
                    join s in _dataContext.Shops on p.ShopId equals s.Id
                    where s.IsActive &&
                       (string.IsNullOrEmpty(model.Name) || p.Name.Contains(model.Name))
                    && (string.IsNullOrEmpty(model.Category) || c.Name == model.Category)
                    && (string.IsNullOrEmpty(model.Color) || p.Color == model.Color)
                    && (!model.MinPrice.HasValue || p.Price >= model.MinPrice)
                    && (!model.MaxPrice.HasValue || p.Price <= model.MaxPrice)
                    select new ProductListForCustomerDto
                    {
                        ProductId = p.Id,
                        ShopName = s.Name,
                        CategoryName = c.Name,
                        ProductName = p.Name,
                        ImageUrl = p.ImageUrl,
                        Price = p.Price,
                        Color = p.Color,
                        Stock = p.Stock
                    };

        var totalItems = await query.CountAsync();
        var items = await query
            .Skip((model.Page - 1) * model.PageSize)
            .Take(model.PageSize)
            .AsNoTracking()
            .ToListAsync();

        return new PaginatedList<ProductListForCustomerDto>(items, totalItems, model.Page, model.PageSize);
    }
    public async Task<ProductDetailsForCustomerDto> GetProductDetailsDtosForCustomerAsync(Guid productId)
    {
        var result = await (from p in _dataContext.Products
                            join c in _dataContext.Categories
                                on p.CategoryId equals c.Id
                            join s in _dataContext.Shops
                                on p.ShopId equals s.Id
                            where p.Id == productId
                            select new ProductDetailsForCustomerDto
                            {
                                ProductId = p.Id,
                                CategoryName = c.Name,
                                ProductName = p.Name,
                                ShopName = s.Name,
                                ImageUrl = p.ImageUrl,
                                Price = p.Price,
                                Color = p.Color,
                                Stock = p.Stock
                            })
                  .AsNoTracking()
                  .FirstOrDefaultAsync();

        return result!;
    }
    private static Expression<Func<Product, bool>> GetAvailableForCustomerFilter()
    {
        return p => !p.IsDeleted && p.IsActive && p.Stock > 0;
    }
    private static Expression<Func<Product, bool>> GetStatusFilter(ProductStatus status, Guid shopId)
    {
        return status switch
        {
            ProductStatus.Available => product => !product.IsDeleted && product.IsActive && product.ShopId == shopId && product.Stock > 0,
            ProductStatus.SoldOut => product => !product.IsDeleted && !product.IsActive && product.ShopId == shopId && product.Stock <= 0,
            _ => product => false
        };
    }

}