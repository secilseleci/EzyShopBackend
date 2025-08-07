using AutoMapper;
using Business.Services.Abstract;
using Core.Constants;
using Core.Interfaces;
using Core.Utilities.Results;
using DataAccess;
using DataAccess.Repositories.Abstract;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.DTOs.Order;
using Models.DTOs.OrderItem;
using Models.Entities.Concrete;
using Models.ViewModels.Cart;
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
        var (isValid, error, customer, product) = await ValidateCustomerAndProductAsync(model.ProductId);
        if (!isValid)
            return new ErrorResult(error!);

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

            //Transaction End
            await trx.CommitAsync();
            return new SuccessResult(Messages.ProductAddedSuccess);
        });
    }

    private async Task<(bool isValid, string? errorMessage, Customer? customer, Product? product)>
    ValidateCustomerAndProductAsync(Guid productId)
    {
        if (!CurrentUserService.UserId.HasValue)
            return (false, Messages.LoginUnauthorized, null, null);

        if (CurrentUserService.Role != CustomRoles.Customer)
            return (false, Messages.UnauthorizedAccess, null, null);

        var customerId = CurrentUserService.UserId.Value;

        var customer = await _customerRepo.GetByIdAsync(customerId);
        if (customer == null)
            return (false, Messages.CustomerNotFound, null, null);

        var product = await _productRepo.GetByIdAsync(productId);

        if (product == null)
            return (false, Messages.ProductNotFound, null, null);

        return (true, null, customer, product);
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
        var existingItem = await _orderItemRepo.GetOrderItemByOrderandProductId(order.Id, product.Id);

        if (existingItem != null)
        {
            var newCount = existingItem.Count + count;
            if (newCount > product.Stock)
                throw new Exception(Messages.StockError);

            existingItem.Count = newCount;
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
            ProductName = product.Name,
            ProductPrice = product.Price,
            Color = product.Color,
            ImageUrl = product.ImageUrl,
            Status = OrderItemStatus.InCart,
        };

        var created = await _orderItemRepo.CreateAsync(newItem);
        if (created <= 0)
            throw new Exception(Messages.CreateError);
    }

    public async Task<IDataResult<Order?>> GetInCartOrderAsync()
    {
        if (!CurrentUserService.UserId.HasValue)
            return new ErrorDataResult<Order?>(Messages.LoginUnauthorized);

        var order = await _orderRepo.GetIncartOrderByCustomerIdAsync(CurrentUserService.UserId.Value);

        return new SuccessDataResult<Order?>(order);

    }
    public async Task<IDataResult<CartPageViewModel>> GetCartPageAsync()
    {
        if (!CurrentUserService.UserId.HasValue)
            return new ErrorDataResult<CartPageViewModel>(Messages.LoginUnauthorized);

        var customerId = CurrentUserService.UserId.Value;

        if (!await _customerRepo.ExistsAsync(c => c.Id == customerId))
            return new ErrorDataResult<CartPageViewModel>(Messages.CustomerNotFound);

        var order = await _orderRepo.GetIncartOrderByCustomerIdAsync(customerId);
        if (order == null || !order.OrderItems.Any())
        {
            return new SuccessDataResult<CartPageViewModel>(
                new CartPageViewModel { OrderItems = new List<OrderItemDto>() });
        }

        var items = await _orderItemRepo.GetOrderItemsAsync(order.Id);

        var total = items.Sum(x => x.Count * x.ProductPrice);

        var vm = new CartPageViewModel
        {
            TotalAmount = total,
            OrderItems = items.ToList()
        };

        return new SuccessDataResult<CartPageViewModel>(vm);
    }
    public async Task<bool> IsCartEmptyAsync()
    {
        if (!CurrentUserService.UserId.HasValue)
            return true;

        var customerId = CurrentUserService.UserId.Value;

        var order = await _orderRepo.GetIncartOrderByCustomerIdAsync(customerId);
        if (order == null)
            return true;

        var items = await _orderItemRepo.GetOrderItemsAsync(order.Id);
        bool hasItems = items.Any();

        return !hasItems;
    }

}
