using Core.Constants;
using Core.Pagination;
using Core.Utilities.Results;
using Models.DTOs.Product;
using Models.ViewModels.Product;

namespace Business.Services.Abstract;

public interface IProductService
{
    #region Seller
    Task<IResult> CreateProductAsync(CreateProductDto model);
    Task<IResult> UpdateProductAsync(UpdateProductDto model);
    Task<IResult> DeleteProductAsync(Guid productId);
    Task<IResult> DeactivateProductAsync(Guid productId);
    Task<IResult> ReactivateProductAsync(Guid productId, int stock);
    Task<IDataResult<PaginatedList<ProductListForSellerDto>>> GetProductsAsync(ProductFilterForSellerViewModel model);
    Task<IDataResult<ProductDetailsForSellerDto>> GetProductDetailsForSellerAsync(Guid productId);
    #endregion


    #region Customer
    Task<IDataResult<PaginatedList<ProductListForCustomerDto>>> GetFilteredProductsAsync(ProductFilterViewModel model);
    Task<IDataResult<ProductDetailsForCustomerDto>> GetProductDetailsForCustomerAsync(Guid productId);
    #endregion  
 

}
