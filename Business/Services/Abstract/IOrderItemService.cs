using Core.Utilities.Results;
using Models.DTOs.OrderItem;

namespace Business.Services.Abstract;

public interface IOrderItemService
{
    Task<IEnumerable<OrderItemDto>> GetOrderItemsAsync(Guid orderId);
    Task<IResult> DeleteItemsAsync(Guid orderId);
    Task<IResult> UpdateOrderItemCountAsync(Guid orderItemId, int deltaCount);
    Task<IResult> DeleteOrderItemAsync(Guid orderItemId);
}
