using AutoMapper;
using Business.Services.Abstract;
using Core.Constants;
using Core.Interfaces;
using Core.Utilities.Results;
using DataAccess.Repositories.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Models.DTOs.OrderItem;
using Models.Identity;

namespace Business.Services.Concrete;

public class OrderItemService : BaseService, IOrderItemService
{
    private readonly IOrderItemRepository _orderItemRepo;
    private readonly IProductRepository _productRepo;
    public OrderItemService(
      IOrderItemRepository orderItemRepo,
      IProductRepository productRepository,
      IMapper mapper,
      IConfiguration config,
      UserManager<AppUser> userManager,
      RoleManager<AppRole> roleManager,
      ICurrentUserService currentUserService,
      ICategoryRepository categoryRepo) : base(mapper, config, currentUserService)
    {
        _orderItemRepo = orderItemRepo;
        _productRepo=productRepository;
    }

    public async Task<IEnumerable<OrderItemDto>> GetOrderItemsAsync(Guid orderId)
    {
        return await _orderItemRepo.GetOrderItemsAsync(orderId);
    }

    public async Task<IResult> DeleteItemsAsync(Guid orderId)
    {
        var items = await _orderItemRepo.GetByOrderIdAsync(orderId);
        if (!items.Any()) return new SuccessResult(Messages.EmptyEntityList);

        var deleteResult = await _orderItemRepo.SoftDeleteRangeAsync(items);

        return deleteResult > 0
            ? new SuccessResult(Messages.DeleteSuccess)
        : new ErrorResult(Messages.DeleteError);
    }
   
    public async Task<IResult> UpdateOrderItemCountAsync(Guid orderItemId, int deltaCount)
    {
        var item = await _orderItemRepo.GetByIdAsync(orderItemId);
        if (item == null)
            return new ErrorResult(Messages.OrderItemNotFound);

        var product = await _productRepo.GetByIdAsync(item.ProductId);
        if (product == null)
            return new ErrorResult(Messages.ProductNotFound); 
        
        var newCount = item.Count + deltaCount;

        if (newCount is < 1 or > 100)
            return new ErrorResult(newCount < 1
                ? Messages.LessCountItemError
                : Messages.ExceedCountItemError);
        
        if (newCount > product.Stock)
            return new ErrorResult(Messages.StockError);

        item.Count = newCount;

        int updateResult = await _orderItemRepo.UpdateAsync(item);
        if (updateResult <= 0)
            return new ErrorResult(Messages.UpdateError);

        return new SuccessResult(Messages.UpdateSuccess);
    }

    public async Task<IResult> DeleteOrderItemAsync(Guid orderItemId)
    {
        var item = await _orderItemRepo.GetByIdAsync(orderItemId);
        if (item == null)
        {
            return new ErrorResult(Messages.OrderItemNotFound);
        }
        var deleteResult = await _orderItemRepo.SoftDeleteAsync(item.Id);
        return deleteResult > 0
          ? new SuccessResult(Messages.DeleteSuccess)
      : new ErrorResult(Messages.DeleteError);
    }
}
