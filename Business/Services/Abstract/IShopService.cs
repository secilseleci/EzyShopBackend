using Core.Utilities.Results;
using Models.DTOs.Auth;
using Models.DTOs.Shop;
using Models.Entities.Concrete;
namespace Business.Services.Abstract;

public interface IShopService
{
    Task<IDataResult<ShopDetailsDto>> GetShopDetailsAsync(Guid shopId);
    Task<bool> IsShopExistsAsync(string name, string taxNumber);
    Task<IResult> DeleteShopAsync(Guid shopId, Guid sellerId);
    Task<DataResult<Shop>> CreateShopAsync(RegisterSellerDto model, Guid sellerId);
    Task<IResult> ActivateShopBySellerIdAsync(Guid sellerId);
    Task<IDataResult<Guid>> GetShopIdByUserIdAsync(Guid userId);
    Task<IDataResult<Guid>> GetActiveShopIdByUserIdAsync(Guid userId);
}
