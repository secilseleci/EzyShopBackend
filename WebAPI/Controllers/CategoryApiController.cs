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

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update([FromBody] CategoryBasicDto model)
    {
        var result = await _categoryService.UpdateCategoryAsync(model);
        return ApiResult(result);
    }

    [HttpPatch("{id}/deactivate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Deactivate([FromRoute] Guid id)
    {
        var result = await _categoryService.DeactivateCategoryAsync(id);
        return ApiResult(result);
    }

    [HttpPatch("{id}/activate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Activate([FromRoute] Guid id)
    {
        var result = await _categoryService.ActivateCategoryAsync(id);
        return ApiResult(result);
    }
}
