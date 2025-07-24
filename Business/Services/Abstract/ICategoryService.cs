using Core.Utilities.Results;
using Models.DTOs.Category;

namespace Business.Services.Abstract;

public interface ICategoryService
{
    Task<IResult> CreateCategoryAsync(CategoryBasicDto model);
    Task<IResult> UpdateCategoryAsync(CategoryBasicDto model);
    Task<IResult> DeleteCategoryAsync(Guid categoryId);
    Task<IDataResult<IEnumerable<CategoryBasicDto>>> GetAllCategoriesAsync();
    Task<IDataResult<CategoryBasicDto>> GetCategoryByIdAsync(Guid categoryId);
}
