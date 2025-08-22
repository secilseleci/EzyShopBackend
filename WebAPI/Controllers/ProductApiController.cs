using AutoMapper;
using Business.Services.Abstract;
using Core.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Product;
using Models.ViewModels.Product;

namespace WebAPI.Controllers;
[ApiController]
[Route("api/products")]
public class ProductApiController : BaseApiController
{
    private readonly IProductService _productService;

    public ProductApiController(IProductService productService, IMapper mapper) : base(mapper)
    {
        _productService = productService;
    }

    [Authorize(Roles = CustomRoles.Seller)]
    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateProductDto dto)
    {
        var result = await _productService.CreateProductAsync(dto);
        return ApiResult(result);
    }


    [Authorize(Roles = CustomRoles.Seller)]
    [HttpPut("update")]
    public async Task<IActionResult> Update(UpdateProductDto dto)
    {
        var result = await _productService.UpdateProductAsync(dto);
        return ApiResult(result);
    }


    [Authorize(Roles = CustomRoles.Seller)]
    [HttpDelete("{productId}")]
    public async Task<IActionResult> Delete(Guid productId)
    {
        var result = await _productService.DeleteProductAsync(productId);
        return ApiResult(result);
    }


    [Authorize(Roles = CustomRoles.Seller)]
    [HttpGet("details-seller/{productId}")]
    public async Task<IActionResult> GetDetails(Guid productId)
    {
        var result = await _productService.GetProductDetailsForSellerAsync(productId);
        return ApiResult(result);
    }


    [Authorize(Roles = CustomRoles.Seller)]
    [HttpPost("deactivate/{productId}")]
    public async Task<IActionResult> Deactivate(Guid productId)
    {
        var result = await _productService.DeactivateProductAsync(productId);
        return ApiResult(result);
    }


    [Authorize(Roles = CustomRoles.Seller)]
    [HttpPost("reactivate/{productId}/{stock}")]
    public async Task<IActionResult> Reactivate(Guid productId, int stock)
    {
        var result = await _productService.ReactivateProductAsync(productId, stock);
        return ApiResult(result);
    }


    [Authorize(Roles = CustomRoles.Seller)]
    [HttpPost("list")]
    public async Task<IActionResult> GetList([FromBody] ProductFilterForSellerViewModel model)
    {
        var result = await _productService.GetProductsAsync(model);
        return ApiResult(result);
    }

    [Authorize(Roles = CustomRoles.Customer)]
    [HttpGet("details-customer/{productId}")]
    public async Task<IActionResult> GetDetailsProduct(Guid productId)
    {
        var result = await _productService.GetProductDetailsForCustomerAsync(productId);
        return ApiResult(result);
    }

    [AllowAnonymous]
    [HttpPost("list-public")]
    public async Task<IActionResult> GetListProduct([FromBody] ProductFilterViewModel model)
    {
        var result = await _productService.GetFilteredProductsAsync(model);
        return ApiResult(result);
    }
}


