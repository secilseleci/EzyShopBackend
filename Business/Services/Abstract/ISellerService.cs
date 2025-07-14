using Core.Utilities.Results;
using Models.Entities.Concrete;
using Models.ViewModels.Auth;

namespace Business.Services.Abstract;

public interface ISellerService
{
    Task<IDataResult<Seller>> CreateSellerApplicationAsync(RegisterSellerViewModel model);
    Task<IDataResult<Seller>> GetActiveSellerByUserIdAsync(Guid userId);

}
