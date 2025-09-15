using AutoMapper;
using Business.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Order;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize(Roles = "Customer")]  
public class CartApiController : BaseApiController
{
    private readonly IOrderService _orderService;
    public CartApiController(IOrderService orderService, IMapper mapper) : base(mapper)
    {
        _orderService = orderService;
    }

    // Sepeti getir
    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var result = await _orderService.GetCartPageAsync();
        return ApiResult(result);
    }

    // Sepete ürün ekle
    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddToCartDto model)
    {
        var result = await _orderService.AddToCartAsync(model);
        return ApiResult(result);
    }

    // Sepeti tamamen temizle
    [HttpDelete]
    public async Task<IActionResult> RemoveCart()
    {
        var result = await _orderService.RemoveCartAsync();
        return ApiResult(result);
    }

    //  ürün adet güncelle: PATCH /api/cart/items/{orderItemId}
    //   ürün sil: DELETE /api/cart/items/{orderItemId}
}
