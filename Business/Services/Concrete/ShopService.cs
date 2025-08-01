using AutoMapper;
using Business.Services.Abstract;
using Core.Constants;
using Core.Interfaces;
using Core.Utilities.Results;
using DataAccess.Repositories.Abstract;
using Microsoft.Extensions.Configuration;
using Models.DTOs.Auth;
using Models.Entities.Concrete;

namespace Business.Services.Concrete;

public class ShopService : BaseService, IShopService
{
    private readonly IShopRepository _shopRepo;

    public ShopService(
      IMapper mapper,
      IConfiguration config,
      ICurrentUserService currentUserService,
      IShopRepository shopRepo) : base(mapper, config, currentUserService)
    {
        _shopRepo = shopRepo;

    }
    #region Create Shop
    public async Task<DataResult<Shop>> CreateShopAsync(RegisterSellerDto model, Guid sellerId)
    {
        var existingShopResult = await CheckShopExistsAsync(model.ShopName, model.TaxNumber);

        if (!existingShopResult.Success)
        {
            return new ErrorDataResult<Shop>(existingShopResult.Message);
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
    #endregion

    #region Delete Shop
    public async Task<IResult> DeleteShopAsync(Guid sellerId)
    {
        var shop = await _shopRepo.GetAsync(s => s.SellerId == sellerId && !s.IsDeleted);
        if (shop == null)
            return new ErrorResult(Messages.ShopNotFound);

        var deleteResult = await _shopRepo.SoftDeleteAsync(shop.Id);

        return deleteResult > 0
            ? new SuccessResult(Messages.DeleteSuccess)
            : new ErrorResult(Messages.DeleteError);
    }
  
    #endregion
   
    #region Activate Shop
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
    #endregion

    #region Deactivate Shop
    public async Task<IResult> DeactivateShopBySellerIdAsync(Guid sellerId)
    {
        var shop = await _shopRepo.GetAsync(s => s.SellerId == sellerId);
        if (shop == null)
            return new ErrorResult(Messages.ShopNotFound);

        if (!shop.IsActive)
            return new ErrorResult(Messages.ShopAlreadyInactive);

        shop.IsActive = false;
        var result = await _shopRepo.UpdateAsync(shop);

        return result > 0
            ? new SuccessResult()
            : new ErrorResult(Messages.UpdateError);
    }
    #endregion

    #region Find Active Shop By UserId
    public async Task<IDataResult<Guid>> GetActiveShopIdByUserIdAsync(Guid userId)
    {
        var shopId = await _shopRepo.GetActiveShopIdByUserIdAsync(userId);
        if (!shopId.HasValue)
            return new ErrorDataResult<Guid>(message: Messages.ShopNotFound);

        return new SuccessDataResult<Guid>(shopId.Value);
    }
    #endregion

    #region Find Shop By UserId
    public async Task<IDataResult<Guid>> GetShopIdByUserIdAsync(Guid userId)
    {
        var shopId = await _shopRepo.GetShopIdByUserIdAsync(userId);
        if (!shopId.HasValue)
            return new ErrorDataResult<Guid>(message: Messages.ShopNotFound);

        return new SuccessDataResult<Guid>(shopId.Value);
    }
    #endregion

    #region Check Shop Exist
    public async Task<IResult> CheckShopExistsAsync(string name, string taxNumber)
    {
        var shopNameExists = await _shopRepo.ExistsAsync(s => s.Name == name && !s.IsDeleted);
        if (shopNameExists)
            return new ErrorResult(Messages.AlreadyExistsShopName);

        var taxNumberExists = await _shopRepo.ExistsAsync(s => s.TaxNumber == taxNumber && !s.IsDeleted);
        if (taxNumberExists)
            return new ErrorResult(Messages.AlreadyExistsTaxNumber);

        return new SuccessResult();
    }

    #endregion

}

