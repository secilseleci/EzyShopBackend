using Core.Utilities.Results;
using Models.ViewModels.Category;

namespace Business.Services.Abstract;

public interface ICategoryService
{
    Task<IResult> CreateCategoryAsync(CategoryViewModel model);
    Task<IResult> UpdateCategoryAsync(CategoryViewModel model);
    Task<IResult> DeleteCategoryAsync(Guid categoryId);
    Task<IDataResult<IEnumerable<CategoryViewModel>>> GetAllCategoriesAsync();
    Task<IDataResult<CategoryViewModel>> GetCategoryByIdAsync(Guid categoryId);
}
