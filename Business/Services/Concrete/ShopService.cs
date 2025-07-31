using AutoMapper;
using Business.Services.Abstract;
using Business.Services.Results;
using Core.Constants;
using Core.Interfaces;
using Core.Pagination;
using Core.Utilities.Results;
using DataAccess.Repositories.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Models.DTOs;
using Models.DTOs.Auth;
using Models.DTOs.Shop;
using Models.Entities.Concrete;
using Models.Identity;

namespace Business.Services.Concrete;

public class ShopService : BaseService, IShopService
{
    private readonly IShopRepository _shopRepo;
    private readonly ISellerRepository _sellerRepo;

    private readonly UserManager<AppUser> _userManager;


    public ShopService(
      IMapper mapper,
      IConfiguration config,
      ICurrentUserService currentUserService,
      UserManager<AppUser> userManager,
      IShopRepository shopRepo,
      ISellerRepository sellerRepo
       ) : base(mapper, config, currentUserService)
    {
        _shopRepo = shopRepo;
        _sellerRepo = sellerRepo;
        _userManager = userManager;

    }

    #region list
    public async Task<IDataResult<PaginatedList<ShopListDto>>> GetShopsAsync(ShopStatus status, string? searchTerm, int page, int pageSize)
    {
        var result = await _shopRepo.GetShopDtosAsync(status, searchTerm, page, pageSize);

        return new SuccessDataResult<PaginatedList<ShopListDto>>(result);
    }
    public async Task<IDataResult<ShopDetailsDto>> GetShopDetailsAsync(Guid shopId)
    {
        var result = await _shopRepo.GetShopDetailsDtosAsync(shopId);

        if (result == null)
        {
            return new ErrorDataResult<ShopDetailsDto>(message: Messages.ShopNotFound);
        }

        return new SuccessDataResult<ShopDetailsDto>(data: result);
    }

    #endregion

    #region Delete
    public async Task<IResult> DeleteShopAsync(Guid shopId, Guid sellerId)
    {
        var validation = await ValidateShopApplication(shopId, sellerId);

        if (!validation.Success)
        {
            return validation;
        }

        var shop = validation.Data.Shop;
        var seller = validation.Data.Seller;
        var user = validation.Data.User;

        if (!IsShopInactive(shop))
            return new ErrorResult(Messages.NotAllowedDelete);

        if (IsShopDeleted(shop))
            return new ErrorDataResult<ShopValidationResult>(Messages.ShopAlreadyDeleted);

        user.IsDeleted = true;

        // Update and delete all
        var deleteResult = await DeleteAllAsync(shop, seller, user);
        if (!deleteResult.Success)
            return deleteResult;

        return new SuccessResult(Messages.DeleteSuccess);
    }
    #endregion

    #region Validate
    private async Task<IDataResult<ShopValidationResult>> ValidateShopApplication(Guid shopId, Guid sellerId)
    {
        var shop = await _shopRepo.GetByIdAsync(shopId);
        if (shop == null || shop.IsDeleted)
            return new ErrorDataResult<ShopValidationResult>(Messages.ShopNotFound);

        if (shop.SellerId != sellerId)
            return new ErrorDataResult<ShopValidationResult>(Messages.ShopSellerMismatch);

        var seller = await _sellerRepo.GetByIdAsync(sellerId);

        if (seller == null || seller.IsDeleted)
            return new ErrorDataResult<ShopValidationResult>(Messages.SellerNotFound);

        var user = await _userManager.FindByIdAsync(seller.Id.ToString());

        if (user == null)
            return new ErrorDataResult<ShopValidationResult>(Messages.UserNotFound);

        return new SuccessDataResult<ShopValidationResult>(new ShopValidationResult
        {
            Shop = shop,
            Seller = seller,
            User = user
        });
    }

    #endregion

    #region private
    private static bool IsShopPending(Shop shop) =>
       !shop.IsDeleted && !shop.IsActive && shop.UpdatedAt == null;

    private static bool IsShopActive(Shop shop) =>
        !shop.IsDeleted && shop.IsActive;

    private static bool IsShopInactive(Shop shop) =>
        !shop.IsDeleted && !shop.IsActive && shop.UpdatedAt != null;

    private static bool IsShopDeleted(Shop shop) =>
        shop.IsDeleted;

    private async Task<IResult> UpdateAllAsync(Shop shop, Seller seller, AppUser user)
    {
        using var transaction = await _shopRepo.BeginTransactionAsync();

        var shopResult = await _shopRepo.UpdateAsync(shop);
        if (shopResult <= 0)
            return new ErrorResult(Messages.UpdateError);

        var sellerResult = await _sellerRepo.UpdateAsync(seller);
        if (sellerResult <= 0)
            return new ErrorResult(Messages.UpdateError);

        var userResult = await _userManager.UpdateAsync(user);
        if (!userResult.Succeeded)
            return new ErrorResult(Messages.UpdateError);

        await transaction.CommitAsync();

        return new SuccessResult();
    }

    private async Task<IResult> DeleteAllAsync(Shop shop, Seller seller, AppUser user)
    {
        using var transaction = await _shopRepo.BeginTransactionAsync();

        var shopResult = await _shopRepo.SoftDeleteAsync(shop.Id);
        if (shopResult <= 0)
            return new ErrorResult(Messages.DeleteError);

        var sellerResult = await _sellerRepo.SoftDeleteAsync(seller.Id);
        if (sellerResult <= 0)
            return new ErrorResult(Messages.DeleteError);

        var userResult = await _userManager.UpdateAsync(user);
        if (!userResult.Succeeded)
            return new ErrorResult(Messages.UpdateError);

        await transaction.CommitAsync();

        return new SuccessResult();
    }

    #endregion

    public async Task<IResult> ActivateShopBySellerIdAsync(Guid sellerId)
    {
        var shop = await _shopRepo.GetAsync(s => s.SellerId == sellerId);
        if (shop == null)
            return new ErrorResult(Messages.ShopNotFound);

        shop.IsActive = true;
        var result = await _shopRepo.UpdateAsync(shop);

        return result > 0
            ? new SuccessResult()
            : new ErrorResult(Messages.UpdateError);
    }

    public async Task<DataResult<Shop>> CreateShopAsync(RegisterSellerDto model, Guid sellerId)
    {
        var existingShop = await _shopRepo.ExistsAsync(s => s.SellerId == sellerId && !s.IsDeleted);

        if (existingShop)
        {
            return new ErrorDataResult<Shop>(message: Messages.AlreadyExistsShop);
        }

        var shop = Mapper.Map<Shop>(model);

        shop.SellerId = sellerId;
        shop.CreatedBy = model.FullName;
        shop.IsActive = false;

        var createShopResult = await _shopRepo.CreateAsync(shop);
        if (createShopResult <= 0)
        {
            return new ErrorDataResult<Shop>(message: Messages.CreateError);
        }

        return new SuccessDataResult<Shop>(data: shop);
    }

    public async Task<IDataResult<Guid>> GetActiveShopIdByUserIdAsync(Guid userId)
    {
        var shopId = await _shopRepo.GetActiveShopIdByUserIdAsync(userId);
        if (!shopId.HasValue)
            return new ErrorDataResult<Guid>(message: Messages.ShopNotFound);

        return new SuccessDataResult<Guid>(shopId.Value);
    }

    public async Task<IDataResult<Guid>> GetShopIdByUserIdAsync(Guid userId)
    {
        var shopId = await _shopRepo.GetShopIdByUserIdAsync(userId);
        if (!shopId.HasValue)
            return new ErrorDataResult<Guid>(message: Messages.ShopNotFound);

        return new SuccessDataResult<Guid>(shopId.Value);
    }

    public async Task<bool> IsShopExistsAsync(string name, string taxNumber)
    {
        return await _shopRepo.ExistsAsync(s =>
            !s.IsDeleted &&
            (s.Name.ToLower().Trim() == name.ToLower().Trim()
            || s.TaxNumber.Trim() == taxNumber.Trim()));
    }
}

