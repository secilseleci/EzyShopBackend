using Core.Utilities.Results;
using Models.DTOs.Auth;
using Models.Entities.Concrete;
namespace Business.Services.Abstract;

public interface IShopService
{
    Task<DataResult<Shop>> CreateShopAsync(RegisterSellerDto model, Guid sellerId);
    Task<IResult> DeleteShopAsync(Guid sellerId);
    Task<IResult> ActivateShopBySellerIdAsync(Guid sellerId);
    Task<IResult> DeactivateShopBySellerIdAsync(Guid sellerId);
    Task<IDataResult<Guid>> GetActiveShopIdByUserIdAsync(Guid userId);
    Task<IDataResult<Guid>> GetShopIdByUserIdAsync(Guid userId);

    Task<IResult> CheckShopExistsAsync(string name, string taxNumber);

}
