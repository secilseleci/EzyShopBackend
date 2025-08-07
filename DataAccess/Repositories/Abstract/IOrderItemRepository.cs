using Models.DTOs.OrderItem;
using Models.Entities.Concrete;
namespace DataAccess.Repositories.Abstract;

public interface IOrderItemRepository : IBaseRepository<OrderItem>
{
    Task<IEnumerable<OrderItemDto>> GetOrderItemsAsync(Guid orderId);
    Task<OrderItem?> GetOrderItemByOrderandProductId(Guid orderId, Guid productId);
    Task<List<OrderItem>> GetByOrderIdAsync(Guid orderId);
}
