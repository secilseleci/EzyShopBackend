using Core.Utilities.Results;
using Models.DTOs.Order;
using Models.ViewModels.Cart;

namespace Business.Services.Abstract;

public interface IOrderService
{
    Task<IResult> AddToCartAsync(AddToCartDto model);
    Task<IDataResult<CartPageViewModel>> GetCartPageAsync();
    Task<bool> IsCartEmptyAsync();

}
