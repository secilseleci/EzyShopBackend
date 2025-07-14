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
using Microsoft.Extensions.Logging;
using Models.DTOs;
using Models.Entities.Concrete;
using Models.Identity;

namespace Business.Services.Concrete;

public class ShopService : BaseService, IShopService
{
    private readonly IShopRepository _shopRepo;
    private readonly ISellerRepository _sellerRepo;
    private readonly IProductRepository _productRepo;

    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly ILogger<ShopService>? _logger;

    public ShopService(
      IMapper mapper,
      IConfiguration config,
      ICurrentUserService currentUserService,
      UserManager<AppUser> userManager,
      IShopRepository shopRepo,
      ISellerRepository sellerRepo,
      IEmailService emailService,
      IProductRepository productRepo,
       ILogger<ShopService> logger) : base(mapper, config, currentUserService)
    {
        _shopRepo = shopRepo;
        _sellerRepo = sellerRepo;
        _userManager = userManager;
        _emailService = emailService;
        _productRepo = productRepo;
        _logger = logger;
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

    #region Approve
    public async Task<IResult> ApproveShopAsync(Guid shopId, Guid sellerId)
    {
        var validation = await ValidateShopApplication(shopId, sellerId);
        if (!validation.Success)
        {
            return validation;
        }

        var shop = validation.Data.Shop;
        var seller = validation.Data.Seller;
        var user = validation.Data.User;

        if (!IsShopPending(shop))
            return new ErrorDataResult<ShopValidationResult>(Messages.ShopAlreadyProcessed);

        // Update values
        shop.IsActive = true;
        seller.IsActive = true;
        user.EmailConfirmed = true;

        // Update all
        var updateResult = await UpdateAllAsync(shop, seller, user);
        if (!updateResult.Success)
            return updateResult;

        //Send email
        var emailResult = await _emailService.SendSellerApprovedEmail(user.Email!, seller.FirstName, shop.Name);
        if (!emailResult)
            _logger?.LogWarning(LogMessages.EmailFailed, user.Email);

        return new SuccessResult(Messages.ApprovedSuccess);
    }

    #endregion

    #region Reject
    public async Task<IResult> RejectShopAsync(Guid shopId, Guid sellerId)
    {
        var validation = await ValidateShopApplication(shopId, sellerId);

        if (!validation.Success)
        {
            return validation;
        }

        var shop = validation.Data.Shop;
        var seller = validation.Data.Seller;
        var user = validation.Data.User;

        if (!IsShopPending(shop))
            return new ErrorDataResult<ShopValidationResult>(Messages.ShopAlreadyProcessed);


        user.IsDeleted = true;

        // Update and delete all
        var deleteResult = await DeleteAllAsync(shop, seller, user);
        if (!deleteResult.Success)
            return deleteResult;

        //Send email
        var emailResult = await _emailService.SendSellerRejectedEmail(user.Email!, seller.FirstName, shop.Name);

        if (!emailResult)
            _logger?.LogWarning(LogMessages.EmailFailed, user.Email);

        return new SuccessResult(Messages.RejectedSuccess);
    }

    #endregion

    #region Deactivate
    public async Task<IResult> DeactivateShopAsync(Guid shopId, Guid sellerId)
    {
        var validation = await ValidateShopApplication(shopId, sellerId);

        if (!validation.Success)
        {
            return validation;
        }
        var shop = validation.Data.Shop;
        var seller = validation.Data.Seller;
        var user = validation.Data.User;

        if (!IsShopActive(shop))
            return new ErrorDataResult<ShopValidationResult>(Messages.ShopAlreadyInactive);

        shop.IsActive = false;
        seller.IsActive = false;
        
        var products = await _productRepo.GetWhereAsync(p => p.ShopId == shop.Id && !p.IsDeleted && p.IsActive);
        foreach (var product in products)
        {
            product.IsActive = false;
        }

        // update all
        using var transaction = await _shopRepo.BeginTransactionAsync();
         
        if (products.Any())
        {
            var updateResult = await _productRepo.UpdateRangeAsync(products);
            if (updateResult <= 0)
                return new ErrorResult(Messages.UpdateError);
        }

        if (await _shopRepo.UpdateAsync(shop) <= 0)
            return new ErrorResult(Messages.UpdateError);

        if (await _sellerRepo.UpdateAsync(seller) <= 0)
            return new ErrorResult(Messages.UpdateError);

        await transaction.CommitAsync();

        //Send email
        var emailResult = await _emailService.SendSellerDeactivatedEmail(user.Email!, seller.FirstName, shop.Name);
        if (!emailResult)
            _logger?.LogWarning(LogMessages.EmailFailed, user.Email);

        return new SuccessResult(Messages.DeactivateShopSuccess);
    }
    #endregion

    #region Reactivate
    public async Task<IResult> ReactivateShopAsync(Guid shopId, Guid sellerId)
    {
        var validation = await ValidateShopApplication(shopId, sellerId);
        if (!validation.Success)
        {
            return validation;
        }
        var shop = validation.Data.Shop;
        var seller = validation.Data.Seller;

        if (!IsShopInactive(shop))
            return new ErrorDataResult<ShopValidationResult>(Messages.ShopAlreadyActive);

        shop.IsActive = true;
        seller.IsActive = true;

        var products = await _productRepo.GetWhereAsync(p => p.ShopId == shop.Id && !p.IsDeleted && !p.IsActive);
        foreach (var product in products)
        {
            product.IsActive = true;
        }
       
        
        // update all
        using var transaction = await _shopRepo.BeginTransactionAsync();

        if(products.Any())
{
            var updateResult = await _productRepo.UpdateRangeAsync(products);
            if (updateResult <= 0)
                return new ErrorResult(Messages.UpdateError);
        }

        if (await _shopRepo.UpdateAsync(shop) <= 0)
            return new ErrorResult(Messages.UpdateError);

        if (await _sellerRepo.UpdateAsync(seller) <= 0)
            return new ErrorResult(Messages.UpdateError);

        await transaction.CommitAsync();

        return new SuccessResult(Messages.ReactivateSuccess);
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

    #region Count
    public async Task<decimal> CountPendingShopsAsync()
    {
        return await _shopRepo.CountPendingShopsAsync(ShopStatus.Pending);
    }

    public async Task<decimal> CountActiveShopsAsync()
    {
        return await _shopRepo.CountActiveShopsAsync(ShopStatus.Active);
    }
    #endregion

    public async Task<IDataResult<Guid>> GetActiveShopIdByUserIdAsync(Guid userId)
    {
        var shopId = await _shopRepo.GetActiveShopIdByUserIdAsync(userId);
        if (!shopId.HasValue)
            return new ErrorDataResult<Guid>(message: Messages.ShopNotFound);

        return new SuccessDataResult<Guid>(shopId.Value);
    }

}
