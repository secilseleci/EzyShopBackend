using AutoMapper;
using Business.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Category;

namespace WebAPI.Controllers;
[ApiController]
[Route("api/categories")]
public class CategoryApiController : BaseApiController
{
    private readonly ICategoryService _categoryService;
    public CategoryApiController(ICategoryService categoryService, IMapper mapper) : base(mapper)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _categoryService.GetCategoriesAsync();
        return ApiResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await _categoryService.GetCategoryByIdAsync(id);
        return ApiResult(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Add([FromBody] CreateCategoryDto model)
    {
        var result = await _categoryService.CreateCategoryAsync(model);
        return ApiResult(result);
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete([FromBody] Guid id)
    {
        var result = await _categoryService.DeleteCategoryAsync(id);
        return ApiResult(result);
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update([FromBody] CategoryBasicDto model)
    {
        var result = await _categoryService.UpdateCategoryAsync(model);
        return ApiResult(result);
    }
}
