using AutoMapper;
using Business.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Customer;

namespace WebAPI.Controllers;

[Route("api/customers")]
[ApiController]
public class CustomerApiController : BaseApiController
{
    private readonly ICustomerService _customerService;
    public CustomerApiController(ICustomerService customerService, IMapper mapper) : base(mapper)
    {
        _customerService = customerService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetCustomer([FromQuery] CustomerSearchDto model)
    {
        var result = await _customerService.GetCustomerBySearchAsync(model);
        return ApiResult(result);
    }

    [HttpDelete("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCustomerByAdmin([FromQuery] Guid customerId)
    {
        var result = await _customerService.DeleteCustomerByAdminAsync(customerId);
        return ApiResult(result);
    }

    [HttpDelete("me")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> DeleteOwnCustomerAccount()
    {
        var result = await _customerService.DeleteOwnCustomerAccountAsync();
        return ApiResult(result);
    }
    [HttpGet("count")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CountCustomer()
    {
        var result = await _customerService.CountAsync();
        return ApiResult(result);
    }
}
