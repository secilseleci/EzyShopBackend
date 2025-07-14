using Core.Utilities.Results;
using Models.Entities.Concrete;
using Models.ViewModels.Cart;

namespace Business.Services.Abstract;

public interface IOrderService
{ 
    Task<IDataResult<Order?>> GetInCartOrderAsync();
    Task<IDataResult<Order>> AddToCartAsync(Guid productId);
    Task<IDataResult<CartPageViewModel>> GetCartPageAsync();
    Task<bool> IsCartEmptyAsync();

}
