using DataAccess.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Models.DTOs.Order;
using Models.Entities.Concrete;

namespace DataAccess.Repositories.Concrete;
public class OrderRepository(ApplicationDbContext context) : BaseRepository<Order>(context), IOrderRepository
{
    public async Task<Order> CreateOrderAsync(Guid customerId)
    {
        var order = new Order { CustomerId = customerId, Status = OrderStatus.InCart ,IsActive=true};
        await _dataContext.AddAsync(order);
        await _dataContext.SaveChangesAsync();
        return order;
    }
    public async Task<Order?> GetIncartOrderByCustomerIdAsync(Guid customerId)
    {
        return await _dataContext.Orders
                .AsNoTracking()
        .FirstOrDefaultAsync(o => o.CustomerId == customerId
        && o.Status == OrderStatus.InCart
        && o.IsDeleted == false
        && o.IsActive == true);
    }
    public async Task<int> SoftDeleteCartOnlyAsync(Guid orderId)
    {
        var order = await _dataContext.Orders
   .FirstOrDefaultAsync(o => o.Id == orderId && o.Status == OrderStatus.InCart && !o.IsDeleted);
        if (order == null) return 0;

        order.Status = OrderStatus.Cancelled;
        return await SoftDeleteAsync(order.Id);
    }
    public async Task<List<CartFlatRow>> GetCartFlatRowsByOrderIdAsync(Guid orderId)
    {
        return await (
            from oi in _dataContext.OrderItems
                .Where(x => x.OrderId == orderId
                            && !x.IsDeleted
                            && x.IsActive
                            && x.Status == OrderItem.OrderItemStatus.InCart)
            join p in _dataContext.Products.Where(p => !p.IsDeleted && p.IsActive)
                on oi.ProductId equals p.Id
            join s in _dataContext.Shops.Where(s => !s.IsDeleted && s.IsActive)
                on p.ShopId equals s.Id
            select new CartFlatRow
            {
                ShopId = s.Id,
                ShopName = s.Name,
                OrderItemId = oi.Id,
                ProductId = p.Id,
                ProductName = oi.ProductName,   // snapshot
                UnitPrice = oi.ProductPrice,    // snapshot
                Count = oi.Count,
                ImageUrl = oi.ImageUrl,
                Color = oi.Color
            })
            .AsNoTracking()
            .ToListAsync();
    }
}
