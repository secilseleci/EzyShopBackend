using Core.Constants;
using Core.Pagination;
using Models.DTOs.Product;
using Models.Entities.Concrete;

namespace DataAccess.Repositories.Abstract;

public interface IProductRepository : IBaseRepository<Product>
{
    Task<PaginatedList<ProductListForSellerDto>> GetProductDtosAsync(ProductStatus status, Guid shopId, string? searchTerm, int page, int pageSize);
    Task<ProductDetailsForSellerDto> GetProductDetailsDtosForSellerAsync(Guid shopId, Guid productId);
    Task<PaginatedList<ProductListForCustomerDto>> GetFilteredProductDtosAsync(ProductFilterViewModel model);
    Task<ProductDetailsForCustomerDto> GetProductDetailsDtosForCustomerAsync(Guid productId);

}
