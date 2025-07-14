using DataAccess.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Models.DTOs.OrderItem;
using Models.Entities.Concrete;

namespace DataAccess.Repositories.Concrete;

public class OrderItemRepository(ApplicationDbContext context) : BaseRepository<OrderItem>(context), IOrderItemRepository
{
    public async Task<Guid> CreateOrderItemAsync(Guid orderId, Guid productId)
    {
        var orderItem = new OrderItem()
        {
            OrderId = orderId,
            ProductId = productId
        };
   
        await _dataContext.AddAsync(orderItem);
        return orderItem.Id;
    }

    public async Task<IEnumerable<OrderItemDto>> GetOrderItemsAsync(Guid orderId)
    {
        var result = await (
            from oi in _dataContext.OrderItems
            join p in _dataContext.Products on oi.ProductId equals p.Id
            join s in _dataContext.Shops on p.ShopId equals s.Id
            where oi.OrderId == orderId && oi.IsDeleted==false
            select new OrderItemDto
            {
                Id = oi.Id,
                ProductId = oi.ProductId,
                ProductName = oi.ProductName,
                ProductPrice = oi.ProductPrice,
                Count = oi.Count,
                Color = oi.Color,
                ImageUrl = oi.ImageUrl,
                ShopName = s.Name
            }).ToListAsync();

        return result;
    }

    public async Task<OrderItem?> GetOrderItemByOrderandProductId(Guid orderId, Guid productId)
    {
        return await _dataContext.OrderItems
       .Where(oi => oi.OrderId == orderId && oi.ProductId == productId && !oi.IsDeleted)
       .FirstOrDefaultAsync();
    }

    public async Task<List<OrderItem>> GetByOrderIdAsync(Guid orderId)
    {
        return await _dataContext.OrderItems
            .Where(oi => oi.OrderId == orderId)
            .ToListAsync();
    }

   
}