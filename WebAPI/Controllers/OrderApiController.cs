using AutoMapper;
using Business.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Order;

namespace WebAPI.Controllers;

[Route("api/orders")]
[ApiController]
public class OrderApiController : BaseApiController
{
    private readonly IOrderService _orderService;
    public OrderApiController(IOrderService orderService, IMapper mapper) : base(mapper)
    {
        _orderService = orderService;
    }


    [HttpPost("add")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartDto model)
    {
        var result = await _orderService.AddToCartAsync(model);
        return ApiResult(result);
    }
}
