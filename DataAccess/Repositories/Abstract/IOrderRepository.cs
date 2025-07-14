using Models.Entities.Concrete;

namespace DataAccess.Repositories.Abstract;

public interface IOrderRepository : IBaseRepository<Order>
{
    Task<Order?> GetIncartOrderByCustomerIdAsync(Guid customerId);
    Task<Order> CreateOrderAsync(Guid customerId);
 
}
