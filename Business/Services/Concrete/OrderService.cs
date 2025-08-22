using AutoMapper;
using Business.Services.Abstract;
using Core.Constants;
using Core.Interfaces;
using Core.Utilities.Results;
using DataAccess;
using DataAccess.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.DTOs.Order;
using Models.Entities.Concrete;
using static Models.Entities.Concrete.OrderItem;

namespace Business.Services.Concrete;

public class OrderService : BaseService, IOrderService
{
    private readonly ApplicationDbContext _context;
    private readonly IOrderRepository _orderRepo;
    private readonly IOrderItemRepository _orderItemRepo;
    private readonly IProductRepository _productRepo;
    private readonly ICustomerRepository _customerRepo;

    public OrderService(ApplicationDbContext context,
          ICustomerRepository customerRepo,
          IProductRepository productRepo,
          IOrderRepository orderRepo,
          IOrderItemRepository orderItemRepo,
          IMapper mapper,
          IConfiguration config,
          ICurrentUserService currentUserService) : base(mapper, config, currentUserService)
    {
        _customerRepo = customerRepo;
        _orderRepo = orderRepo;
        _orderItemRepo = orderItemRepo;
        _productRepo = productRepo;
        _context = context;
    }
    public async Task<IResult> AddToCartAsync(AddToCartDto model)
    {
        //Customer check
        var (isValid, error, customer) = await ValidateCustomerAsync();
        if (!isValid)
            return new ErrorResult(error!);

        //Product check
        var product = await _productRepo.GetByIdAsync(model.ProductId);

        if (product == null)
            return new ErrorResult(Messages.ProductNotFound);
        if (product!.Stock <= 0)
            return new ErrorResult(Messages.StockError);

        //Transaction Start
        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var trx = await _context.Database.BeginTransactionAsync();

            var order = await GetOrCreateCartOrThrowAsync(customer!.Id);
            await AddOrUpdateOrderItemOrThrowAsync(order, product, model.Count);
            await UpdateTotalAmountOrThrowAsync(order);

            await trx.CommitAsync();
            //Transaction End

            return new SuccessResult(Messages.ProductAddedSuccess);
        });
    }
    public async Task<IResult> RemoveCartAsync()
    {
        //Customer check
        var (isValid, error, customer) = await ValidateCustomerAsync();
        if (!isValid)
            return new ErrorResult(error!);

        //Order check
        var existingOrder = await _orderRepo.GetIncartOrderByCustomerIdAsync(customer!.Id);
        if (existingOrder == null)
            return new ErrorResult(Messages.OrderNotFound);

        //Orderitems check
        var existingOrderItems = await _orderItemRepo.GetWhereAsync(i => i.OrderId == existingOrder.Id);

        //Transaction Start
        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var trx = await _context.Database.BeginTransactionAsync();

            await _orderItemRepo.SoftDeleteRangeAsync(existingOrderItems);

            var deleteResult = await _orderRepo.SoftDeleteCartOnlyAsync(existingOrder.Id);
            if (deleteResult <= 0) throw new Exception(Messages.DeleteError);

            await trx.CommitAsync();
            //Transaction end  
            return new SuccessResult(Messages.DeleteSuccess);
        });
    }
    public async Task<IDataResult<CartDto>> GetCartPageAsync()
    {
        // (1-3) Login + Role + CustomerId
        var (isValid, error, customer) = await ValidateCustomerAsync();
        if (!isValid)
            return new ErrorDataResult<CartDto>(error!);

        // (4) InCart order?
        var existingOrder = await _orderRepo.GetIncartOrderByCustomerIdAsync(customer!.Id);
        if (existingOrder is null)
        {
            // GET / empty cart
            return new SuccessDataResult<CartDto>(new CartDto
            {
                OrderId = Guid.Empty,
                TotalAmount = 0,
                TotalItemCount = 0,
                DistinctShopCount = 0,
                Shops = []
            });
        }

        var rows = await _orderRepo.GetCartFlatRowsByOrderIdAsync(existingOrder.Id);
        var shops = rows
      .GroupBy(r => new { r.ShopId, r.ShopName })
      .Select(g => new CartShopDto
      {
          ShopId = g.Key.ShopId,
          ShopName = g.Key.ShopName,
          Subtotal = g.Sum(i => i.UnitPrice * i.Count),
          Items = g.Select(i => new CartItemDto
          {
              OrderItemId = i.OrderItemId,
              ProductId = i.ProductId,
              ProductName = i.ProductName,
              UnitPrice = i.UnitPrice,
              Count = i.Count,
              LineTotal = i.UnitPrice * i.Count,
              ImageUrl = i.ImageUrl,
              Color = i.Color
          }).ToList()
      })
      .ToList();

        var dto = new CartDto
        {
            OrderId = existingOrder.Id,
            TotalAmount = shops.Sum(s => s.Subtotal),
            TotalItemCount = shops.Sum(s => s.Items.Sum(i => i.Count)),
            DistinctShopCount = shops.Count,
            Shops = shops
        };

        return new SuccessDataResult<CartDto>(dto);
    }

    
    private async Task<Order> GetOrCreateCartOrThrowAsync(Guid customerId)
    {
        var existingOrder = await _orderRepo.GetIncartOrderByCustomerIdAsync(customerId);
        if (existingOrder != null)
            return existingOrder;

        var newOrder = await _orderRepo.CreateOrderAsync(customerId);
        if (newOrder == null)
            throw new Exception(Messages.CreateError);

        return newOrder;
    }
    private async Task UpdateTotalAmountOrThrowAsync(Order order)
    {
        var orderItems = await _orderItemRepo.GetWhereAsync(i => i.OrderId == order.Id && !i.IsDeleted);
        order.TotalAmount = orderItems.Sum(i => i.TotalPrice);

        var result = await _orderRepo.UpdateAsync(order);
        if (result == 0)
            throw new Exception(Messages.ProductAddedError);
    }
    private async Task AddOrUpdateOrderItemOrThrowAsync(Order order, Product product, int count)
    {
         var existingItem = await _orderItemRepo
            .GetOrderItemByOrderAndProductId(order.Id, product.Id);

        if (existingItem != null)
        {
            var newCount = existingItem.Count + count;
            if (newCount > product.Stock)
                throw new Exception(Messages.StockError);

            existingItem.Count = newCount;
            existingItem.IsActive = true;

            var updated = await _orderItemRepo.UpdateAsync(existingItem);
            if (updated <= 0)
                throw new Exception(Messages.UpdateError);

            return;
        }

         if (count > product.Stock)
            throw new Exception(Messages.StockError);

        var newItem = new OrderItem
        {
            ProductId = product.Id,
            OrderId = order.Id,
            Count = count,
            ProductName = product.Name,   // snapshot
            ProductPrice = product.Price,  // snapshot
            Color = product.Color,
            ImageUrl = product.ImageUrl,
            Status = OrderItemStatus.InCart,
            IsActive = true
        };

        var created = await _orderItemRepo.CreateAsync(newItem);
        if (created <= 0)
            throw new Exception(Messages.CreateError);
    }
    private async Task<(bool isValid, string? errorMessage, Customer? customer)> ValidateCustomerAsync()
    {
        if (!CurrentUserService.UserId.HasValue)
            return (false, Messages.LoginUnauthorized, null);

        if (CurrentUserService.Role != CustomRoles.Customer)
            return (false, Messages.UnauthorizedAccess, null);

        var customerId = CurrentUserService.UserId.Value;

        var customer = await _customerRepo.GetByIdAsync(customerId);
        if (customer == null)
            return (false, Messages.CustomerNotFound, null);

        return (true, null, customer);
    }


}
