using Core.Constants;
using Core.Pagination;
using Models.DTOs;
using Models.Entities.Concrete;
using Models.ViewModels.Product;

namespace DataAccess.Repositories.Abstract;

public interface IProductRepository : IBaseRepository<Product>
{
    Task<PaginatedList<ProductListForSellerDto>> GetProductDtosAsync(ProductStatus status, Guid shopId, string? searchTerm, int page, int pageSize);
    Task<ProductDetailsForSellerDto> GetProductDetailsDtosForSellerAsync(Guid shopId, Guid productId);
    Task<PaginatedList<ProductListForCustomerDto>> GetFilteredProductDtosAsync(ProductFilterViewModel model);
    Task<ProductDetailsForCustomerDto> GetProductDetailsDtosForCustomerAsync(Guid productId);

}
