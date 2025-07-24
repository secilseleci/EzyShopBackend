using AutoMapper;
using Business.Services.Abstract;
using Business.Services.Concrete;
using Core.Constants;
using Core.Utilities.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Product;
using Models.Entities.Concrete;

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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await _productService.GetProductByIdAsync(id);
        return ApiResult(result);
    }

    [HttpPost]
    [Authorize(Roles = "Seller")]
    public async Task<IActionResult> Add([FromBody] CreateProductDto product)
    {
        var result = await _productService.CreateProductAsync(product);
        return ApiResult(result);
    }

    [HttpPut]
    [Authorize(Roles = "Seller")]
    public async Task<IActionResult> Update([FromBody] UpdateProductDto product)
    {
        var result = await _productService.UpdateProductAsync(product);
        return ApiResult(result);
    }
}


