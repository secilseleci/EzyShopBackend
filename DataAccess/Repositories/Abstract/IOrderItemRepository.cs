using Models.Entities.Concrete;
namespace DataAccess.Repositories.Abstract;

public interface IOrderItemRepository : IBaseRepository<OrderItem>
{
    Task<OrderItem?> GetOrderItemByOrderAndProductId(Guid orderId, Guid productId);
    Task<List<OrderItem>> GetByOrderIdAsync(Guid orderId, bool onlyActive = true);
}
