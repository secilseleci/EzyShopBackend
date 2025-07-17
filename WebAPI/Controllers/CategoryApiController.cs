using Business.Services.Abstract;
using Business.Services.Concrete;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModels.Category;

namespace WebAPI.Controllers;
[ApiController]
[Route("api/categories")]
public class CategoryApiController:BaseApiController
{
    private readonly ICategoryService _categoryService;
    public CategoryApiController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _categoryService.GetAllCategoriesAsync();
        return ApiResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await _categoryService.GetCategoryByIdAsync(id);
        return ApiResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody]CategoryViewModel model)
    {
        var result = await _categoryService.CreateCategoryAsync(model);
        return ApiResult(result);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] Guid id)
    {
        var result = await _categoryService.DeleteCategoryAsync(id);
        return ApiResult(result);
    }
}
