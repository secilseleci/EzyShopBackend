using Core.Constants;
using Core.Pagination;
using Models.DTOs.Product;
using Models.Entities.Concrete;
using Models.ViewModels.Product;

namespace DataAccess.Repositories.Abstract;

public interface IProductRepository : IBaseRepository<Product>
{
    Task<PaginatedList<ProductListForSellerDto>> GetFilteredProductsForSellerAsync(ProductFilterForSellerViewModel model, Guid shopId);
    Task<ProductDetailsForSellerDto> GetProductDetailsDtosForSellerAsync(Guid shopId, Guid productId);
    Task<PaginatedList<ProductListForCustomerDto>> GetFilteredProductDtosAsync(ProductFilterViewModel model);
    Task<ProductDetailsForCustomerDto> GetProductDetailsDtosForCustomerAsync(Guid productId);
 
}
