using Core.Utilities.Results;
using Models.DTOs.Auth;
using Models.Entities.Concrete;

namespace Business.Services.Abstract;

public interface ISellerService
{
    Task<IDataResult<Seller>> CreateSellerApplicationAsync(RegisterSellerDto model);
    Task<IDataResult<Seller>> GetActiveSellerByUserIdAsync(Guid userId);

}
