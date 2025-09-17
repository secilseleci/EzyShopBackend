using Core.Utilities.Results;
using Models.DTOs.Category;

namespace Business.Services.Abstract;
public interface ICategoryService
{
    Task<IDataResult<CreateCategoryDto>> CreateCategoryAsync(CreateCategoryDto model);
    Task<IDataResult<List<CategoryBasicDto>>> GetCategoriesAsync();
    Task<IDataResult<CategoryBasicDto>> GetCategoryByIdAsync(Guid categoryId);
    Task<IDataResult<CategoryBasicDto>> UpdateCategoryAsync(CategoryBasicDto model);
    Task<IResult> DeactivateCategoryAsync(Guid categoryId);
    Task<IResult> ActivateCategoryAsync(Guid categoryId);
}
