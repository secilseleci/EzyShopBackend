using Core.Utilities.Results;
using Models.DTOs.Category;

namespace Business.Services.Abstract;
public interface ICategoryService
{
    Task<IDataResult<CreateCategoryDto>> CreateCategoryAsync(CreateCategoryDto model);
    Task<IResult> UpdateCategoryAsync(CategoryBasicDto model);
    Task<IResult> DeleteCategoryAsync(Guid categoryId);
    Task<IDataResult<List<CategoryBasicDto>>> GetCategoriesAsync();
    Task<IDataResult<CategoryBasicDto>> GetCategoryByIdAsync(Guid categoryId);
}
