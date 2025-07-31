using Core.Pagination;
using DataAccess.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Models.DTOs.Product;
using Models.Entities.Concrete;
using Models.ViewModels.Product;

namespace DataAccess.Repositories.Concrete;

public class ProductRepository(ApplicationDbContext context) : BaseRepository<Product>(context), IProductRepository
{
    public async Task<ProductDetailsForSellerDto?> GetProductDetailsDtosForSellerAsync(Guid shopId, Guid productId)
    {
        return await (from p in _dataContext.Products
                      join c in _dataContext.Categories on p.CategoryId equals c.Id
                      where p.Id == productId && p.ShopId == shopId && !p.IsDeleted && !c.IsDeleted
                      select new ProductDetailsForSellerDto
                      {
                          ProductId=p.Id,
                          CategoryName = c.Name,
                          ProductName = p.Name,
                          ImageUrl = p.ImageUrl,
                          Price = p.Price,
                          Color = p.Color,
                          Stock = p.Stock
                      })
                      .AsNoTracking()
                      .FirstOrDefaultAsync();
    }

    public async Task<PaginatedList<ProductListForSellerDto>> GetFilteredProductsForSellerAsync(ProductFilterForSellerViewModel model, Guid shopId)
    {
        var query = from p in _dataContext.Products
                    join c in _dataContext.Categories on p.CategoryId equals c.Id
                    where p.ShopId == shopId && !p.IsDeleted && !c.IsDeleted
                    select new ProductListForSellerDto
                    {
                        ProductId = p.Id,
                        ProductName = p.Name,
                        CategoryName = c.Name,
                        CategoryId = c.Id,
                        IsActive = p.IsActive,
                        ImageUrl = p.ImageUrl,
                        Price = p.Price,
                        Stock = p.Stock
                    };

        if (model.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == model.CategoryId.Value);

        if (model.IsActive.HasValue)
            query = query.Where(p => p.IsActive == model.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(model.SearchTerm))
        {
            var search = model.SearchTerm.Trim().ToLower();
            query = query.Where(p =>
                p.ProductName.ToLower().Contains(search) ||
                p.CategoryName.ToLower().Contains(search));
        }

        query = model.SortBy?.ToLower() switch
        {
            "price" => model.Descending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
            "stock" => model.Descending ? query.OrderByDescending(p => p.Stock) : query.OrderBy(p => p.Stock),
            "name" or _ => model.Descending ? query.OrderByDescending(p => p.ProductName) : query.OrderBy(p => p.ProductName)
        };

        var page = model.Page <= 0 ? 1 : model.Page;
        var pageSize = model.PageSize <= 0 ? 10 : model.PageSize;

        var totalItems = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();

        return new PaginatedList<ProductListForSellerDto>(items, totalItems, page, pageSize);
    }

    public async Task<ProductDetailsForCustomerDto?> GetProductDetailsDtosForCustomerAsync(Guid productId)
    {
        return await (from p in _dataContext.Products
                      join c in _dataContext.Categories on p.CategoryId equals c.Id
                      join s in _dataContext.Shops on p.ShopId equals s.Id
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
    }

    public async Task<PaginatedList<ProductListForCustomerDto>> GetFilteredProductDtosAsync(ProductFilterViewModel model)
    {
        var query = from p in _dataContext.Products
                    join c in _dataContext.Categories on p.CategoryId equals c.Id
                    join s in _dataContext.Shops on p.ShopId equals s.Id
                    where    !p.IsDeleted && p.IsActive && p.Stock > 0
                          && !c.IsDeleted && c.IsActive
                          && !s.IsDeleted && s.IsActive
                    select new ProductListForCustomerDto
                    {
                        ProductId = p.Id,
                        ProductName = p.Name,
                        CategoryName = c.Name,
                        ShopName = s.Name,
                        ImageUrl = p.ImageUrl,
                        Price = p.Price,
                        Stock = p.Stock,
                        Color = p.Color
                    };

        // 🔍 Filtreleme
        if (!string.IsNullOrWhiteSpace(model.CategoryName))
            query = query.Where(x => x.CategoryName == model.CategoryName);

        if (!string.IsNullOrWhiteSpace(model.Name))
            query = query.Where(x => x.ProductName.Contains(model.Name));

        if (!string.IsNullOrWhiteSpace(model.Color))
            query = query.Where(x => x.Color == model.Color);

        if (model.MinPrice.HasValue)
            query = query.Where(x => x.Price >= model.MinPrice.Value);

        if (model.MaxPrice.HasValue)
            query = query.Where(x => x.Price <= model.MaxPrice.Value);

        if (!string.IsNullOrWhiteSpace(model.SearchTerm))
        {
            var search = model.SearchTerm.Trim().ToLower();
            query = query.Where(x =>
                x.ProductName.ToLower().Contains(search) ||
                x.CategoryName.ToLower().Contains(search) ||
                x.ShopName.ToLower().Contains(search));
        }

        // 🔃 Sıralama
        switch (model.SortBy?.ToLower())
        {
            case "price":
                query = model.Descending
                    ? query.OrderByDescending(x => x.Price)
                    : query.OrderBy(x => x.Price);
                break;

            case "stock":
                query = model.Descending
                    ? query.OrderByDescending(x => x.Stock)
                    : query.OrderBy(x => x.Stock);
                break;

            case "name":
            default:
                query = model.Descending
                    ? query.OrderByDescending(x => x.ProductName)
                    : query.OrderBy(x => x.ProductName);
                break;
        }

        var totalItems = await query.CountAsync();

        // 🔄 Sayfalama + DTO projeksiyonu
        var items = await query
            .Skip((model.Page - 1) * model.PageSize)
            .Take(model.PageSize)
            .Select(x => new ProductListForCustomerDto
            {
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                CategoryName = x.CategoryName,
                ShopName = x.ShopName,
                ImageUrl = x.ImageUrl,
                Price = x.Price,
                Stock = x.Stock,
                Color = x.Color
            })
            .AsNoTracking()
            .ToListAsync();

        return new PaginatedList<ProductListForCustomerDto>(items, totalItems, model.Page, model.PageSize);
    }
}
