using DataAccess.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Models.Entities.Concrete;

namespace DataAccess.Repositories.Concrete;

public class OrderRepository(ApplicationDbContext context) : BaseRepository<Order>(context), IOrderRepository
{
    public async Task<Order> CreateOrderAsync(Guid customerId)
    {
        var order = new Order { CustomerId = customerId, Status = OrderStatus.InCart };
        await _dataContext.AddAsync(order);
        return order;
    }

    public async Task<Order?> GetIncartOrderByCustomerIdAsync(Guid customerId)
    {
        return await _dataContext.Orders
        .Include(o => o.OrderItems)
        .FirstOrDefaultAsync(o => o.CustomerId == customerId && o.Status == OrderStatus.InCart );
    }
}
