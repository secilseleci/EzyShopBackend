using AutoMapper;
using Business.Services.Abstract;
using Core.Constants;
using Core.Interfaces;
using Core.Pagination;
using Core.Utilities.Results;
using DataAccess.Repositories.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Models.DTOs.Product;
using Models.Entities.Concrete;
using Models.Identity;
using Models.ViewModels.Product;

namespace Business.Services.Concrete;

public class ProductService : BaseService, IProductService
{
    private readonly IShopService _shopService;
    private readonly IProductRepository _productRepo;
    private readonly ICategoryRepository _categoryRepo;
    private readonly UserManager<AppUser> _userManager;

    public ProductService(
      IMapper mapper,
      IConfiguration config,
      UserManager<AppUser> userManager,
      RoleManager<AppRole> roleManager,
      ICurrentUserService currentUserService,
      IProductRepository productRepo,
      ICategoryRepository categoryRepo,
      IShopService shopService) : base(mapper, config, currentUserService)
    {
        _userManager = userManager;
        _productRepo = productRepo;
        _shopService = shopService;
        _categoryRepo = categoryRepo;
    }

    #region Seller
    public async Task<IResult> CreateProductAsync(CreateProductDto model)
    {
        //Login check
        if (!CurrentUserService.UserId.HasValue)
            return new ErrorResult(Messages.LoginUnauthorized);

        //Role check
        if (CurrentUserService.Role != CustomRoles.Seller)
            return new ErrorResult(Messages.UnauthorizedAccess);

        //Active Shop check 
        var shopId = await _shopService.GetActiveShopIdByUserIdAsync(CurrentUserService.UserId.Value);

        if (!shopId.Success)
            return new ErrorResult(Messages.ShopNotFound);

        //Category check
        var categoryExists = await _categoryRepo.ExistsAsync(c => c.Id == model.CategoryId && !c.IsDeleted);
        if (!categoryExists)
            return new ErrorResult(Messages.CategoryNotFound);

        //Product check
        if (await _productRepo.ExistsAsync(p =>
        p.Name.ToLower() == model.Name.ToLower()
        && p.ShopId == shopId.Data
        && !p.IsDeleted))
        {
            return new ErrorResult(Messages.AlreadyExists);
        }

        //Data mapping
        var product = Mapper.Map<Product>(model);
        product.ShopId = shopId.Data;

        //Insert
        var createResult = await _productRepo.CreateAsync(product);

        return createResult <= 0
             ? new ErrorResult(message: Messages.CreateError)
             : new SuccessResult(message: Messages.CreateSuccess);
    }
    public async Task<IResult> UpdateProductAsync(UpdateProductDto model)
    {
        //Login check
        if (!CurrentUserService.UserId.HasValue)
            return new ErrorResult(Messages.LoginUnauthorized);

        //Role check
        if (CurrentUserService.Role != CustomRoles.Seller)
            return new ErrorResult(Messages.UnauthorizedAccess);

        //Active Shop check 
        var shopId = await _shopService.GetActiveShopIdByUserIdAsync(CurrentUserService.UserId.Value);

        if (!shopId.Success)
            return new ErrorResult(Messages.ShopNotFound);

        //Category check
        var categoryExists = await _categoryRepo.ExistsAsync(c => c.Id == model.CategoryId && !c.IsDeleted);
        if (!categoryExists)
            return new ErrorResult(Messages.CategoryNotFound);

        //Product check
        var product = await _productRepo.GetAsync(p =>
             p.Id == model.Id &&
             p.ShopId == shopId.Data &&
             !p.IsDeleted);

        if (product is null)
            return new ErrorResult(Messages.ProductNotFound);

        //Data check
        var alreadyExists = await _productRepo.ExistsAsync(p =>
        p.Id != model.Id &&
        p.Name.ToLower() == model.Name.ToLower() &&
        p.ShopId == shopId.Data &&
        !p.IsDeleted);

        if (alreadyExists)
            return new ErrorResult(Messages.AlreadyExists);

        //Data mapping
        product.Name = model.Name;
        product.Price = model.Price;
        product.Stock = model.Stock;
        product.Color = model.Color;
        product.CategoryId = model.CategoryId;
        product.ImageUrl = model.ImageUrl;

        //Update
        var updateResult = await _productRepo.UpdateAsync(product);

        return updateResult > 0
            ? new SuccessResult(Messages.UpdateSuccess)
            : new ErrorResult(Messages.UpdateError);
    }
    public async Task<IResult> DeleteProductAsync(Guid productId)
    {
        // Login check
        if (!CurrentUserService.UserId.HasValue)
            return new ErrorResult(Messages.LoginUnauthorized);

        // Role check
        if (CurrentUserService.Role != CustomRoles.Seller)
            return new ErrorResult(Messages.UnauthorizedAccess);

        // Active Shop check 
        var shopId = await _shopService.GetActiveShopIdByUserIdAsync(CurrentUserService.UserId.Value);
        if (!shopId.Success)
            return new ErrorResult(Messages.ShopNotFound);

        // Product check  
        var product = await _productRepo.GetAsync(p =>
             p.Id == productId &&
             p.ShopId == shopId.Data &&
             !p.IsDeleted);

        if (product is null)
            return new ErrorResult(Messages.ProductNotFound);
        if (product.IsActive && product.Stock > 0)
            return new ErrorResult(Messages.DeleteProductError);

        // Soft delete
        var result = await _productRepo.SoftDeleteAsync(productId);
        return result > 0
            ? new SuccessResult(Messages.DeleteSuccess)
            : new ErrorResult(Messages.DeleteError);
    }
    public async Task<IDataResult<ProductDetailsForSellerDto>> GetProductDetailsForSellerAsync(Guid productId)
    {
        //Login check
        if (!CurrentUserService.UserId.HasValue)
            return new ErrorDataResult<ProductDetailsForSellerDto>(Messages.LoginUnauthorized);

        //Role check
        if (CurrentUserService.Role != CustomRoles.Seller)
            return new ErrorDataResult<ProductDetailsForSellerDto>(Messages.UnauthorizedAccess);

        //Shop check 
        var shopId = await _shopService.GetShopIdByUserIdAsync(CurrentUserService.UserId.Value);

        if (!shopId.Success)
            return new ErrorDataResult<ProductDetailsForSellerDto>(Messages.ShopNotFound);

        //Product check
        var result = await _productRepo.GetProductDetailsDtosForSellerAsync(shopId.Data, productId);

        if (result == null)
        {
            return new ErrorDataResult<ProductDetailsForSellerDto>(message: Messages.ProductNotFound);
        }

        return new SuccessDataResult<ProductDetailsForSellerDto>(data: result);
    }

    public async Task<IResult> DeactivateProductAsync(Guid productId)
    {
        //Login check
        if (!CurrentUserService.UserId.HasValue)
            return new ErrorResult(Messages.LoginUnauthorized);

        //Role check
        if (CurrentUserService.Role != CustomRoles.Seller)
            return new ErrorResult(Messages.UnauthorizedAccess);

        //Active Shop check 
        var shopId = await _shopService.GetActiveShopIdByUserIdAsync(CurrentUserService.UserId.Value);

        if (!shopId.Success)
            return new ErrorResult(Messages.ShopNotFound);

        //Product check
        var product = await _productRepo.GetByIdAsync(productId);
        if (product is null)
            return new ErrorResult(Messages.ProductNotFound);

        if (product.ShopId != shopId.Data)
            return new ErrorResult(Messages.ProductUnauthorized);

        //Update
        product.IsActive = false;
        var result = await _productRepo.UpdateAsync(product);

        return result > 0
            ? new SuccessResult(Messages.DeactivateProductSuccess)
            : new ErrorResult(Messages.DeactivateProductError);
    }
    public async Task<IResult> ReactivateProductAsync(Guid productId, int stock)
    {
        //Login check
        if (!CurrentUserService.UserId.HasValue)
            return new ErrorResult(Messages.LoginUnauthorized);

        //Role check
        if (CurrentUserService.Role != CustomRoles.Seller)
            return new ErrorResult(Messages.UnauthorizedAccess);

        //Active Shop check 
        var shopId = await _shopService.GetActiveShopIdByUserIdAsync(CurrentUserService.UserId.Value);

        if (!shopId.Success)
            return new ErrorResult(Messages.ShopNotFound);


        //Product check
        var product = await _productRepo.GetByIdAsync(productId);
        if (product is null)
            return new ErrorResult(Messages.ProductNotFound);

        if (product.ShopId != shopId.Data)
            return new ErrorResult(Messages.UnauthorizedAccess);

        // Stock check
        if (stock <= 0)
            return new ErrorResult(Messages.InsufficientStock);

        //Update
        product.Stock = stock;
        product.IsActive = true;

        var result = await _productRepo.UpdateAsync(product);

        return result > 0
            ? new SuccessResult(Messages.ReactivateProductSuccess)
            : new ErrorResult(Messages.ReactivateProductError);
    }

    public async Task<IDataResult<PaginatedList<ProductListForSellerDto>>> GetProductsAsync(ProductFilterForSellerViewModel model)
    {
        // Login Check
        if (!CurrentUserService.UserId.HasValue)
            return new ErrorDataResult<PaginatedList<ProductListForSellerDto>>(Messages.LoginUnauthorized);

        // Role check
        if (CurrentUserService.Role != CustomRoles.Seller)
            return new ErrorDataResult<PaginatedList<ProductListForSellerDto>>(Messages.UnauthorizedAccess);

        // Shop check 
        var shopId = await _shopService.GetShopIdByUserIdAsync(CurrentUserService.UserId.Value);
        if (!shopId.Success)
            return new ErrorDataResult<PaginatedList<ProductListForSellerDto>>(Messages.ShopNotFound);

        // Repository 
        var result = await _productRepo.GetFilteredProductsForSellerAsync(model, shopId.Data);

        return new SuccessDataResult<PaginatedList<ProductListForSellerDto>>(result);
    }

    #endregion

    #region Customer

    public async Task<IDataResult<ProductDetailsForCustomerDto>> GetProductDetailsForCustomerAsync(Guid productId)
    {
        //Login check
        if (!CurrentUserService.UserId.HasValue)
            return new ErrorDataResult<ProductDetailsForCustomerDto>(Messages.LoginUnauthorized);

        //Role check
        if (CurrentUserService.Role != CustomRoles.Customer)
            return new ErrorDataResult<ProductDetailsForCustomerDto>(Messages.UnauthorizedAccess);

        //Product check
        if (!await IsProductAvailableForCustomer(productId))
            return new ErrorDataResult<ProductDetailsForCustomerDto>(Messages.ProductNotFound);

        //Repository
        var result = await _productRepo.GetProductDetailsDtosForCustomerAsync(productId);
        if (result is null)
            return new ErrorDataResult<ProductDetailsForCustomerDto>(Messages.ProductNotFound);

        return new SuccessDataResult<ProductDetailsForCustomerDto>(result);
    }
    private async Task<bool> IsProductAvailableForCustomer(Guid productId)
    {
        var product = await _productRepo.GetByIdAsync(productId);

        if (product == null)
            return false;

        if (product.IsDeleted)
            return false;

        if (!product.IsActive)
            return false;

        if (product.Stock <= 0)
            return false;

        return true;
    }

    #endregion


    public async Task<IDataResult<PaginatedList<ProductListForCustomerDto>>> GetFilteredProductsAsync(ProductFilterViewModel model)
    {
        var result = await _productRepo.GetFilteredProductDtosAsync(model);
        return new SuccessDataResult<PaginatedList<ProductListForCustomerDto>>(result);
    }
}