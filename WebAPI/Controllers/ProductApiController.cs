using AutoMapper;
using Business.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

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
}


