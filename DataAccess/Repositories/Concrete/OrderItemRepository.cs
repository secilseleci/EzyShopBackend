using DataAccess.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Models.Entities.Concrete;

namespace DataAccess.Repositories.Concrete;

public class OrderItemRepository(ApplicationDbContext context) : BaseRepository<OrderItem>(context), IOrderItemRepository
{
    public async Task<OrderItem?> GetOrderItemByOrderAndProductId(Guid orderId, Guid productId)
    {
        return await _dataContext.OrderItems
            .AsNoTracking()
            .FirstOrDefaultAsync(oi => oi.OrderId == orderId
                                    && oi.ProductId == productId
                                    && !oi.IsDeleted);  
    }

    public async Task<List<OrderItem>> GetByOrderIdAsync(Guid orderId, bool onlyActive = true)
    {
        var q = _dataContext.OrderItems
            .Where(oi => oi.OrderId == orderId && !oi.IsDeleted && oi.Status == OrderItem.OrderItemStatus.InCart);

        if (onlyActive) q = q.Where(oi => oi.IsActive);

        return await q.AsNoTracking().ToListAsync();
    }
}