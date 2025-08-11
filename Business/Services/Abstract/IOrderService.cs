using Core.Utilities.Results;
using Models.DTOs.Order;

namespace Business.Services.Abstract;

public interface IOrderService
{
    Task<IResult> AddToCartAsync(AddToCartDto model);
    Task<IResult> RemoveCartAsync();
    Task<IDataResult<CartDto>> GetCartPageAsync();
}
