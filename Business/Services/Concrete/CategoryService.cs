using AutoMapper;
using Business.Services.Abstract;
using Core.Constants;
using Core.Interfaces;
using Core.Utilities.Results;
using DataAccess.Repositories.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Models.Entities.Concrete;
using Models.Identity;
using Models.ViewModels.Category;

namespace Business.Services.Concrete;

public class CategoryService : BaseService, ICategoryService
{
    private readonly ICategoryRepository _categoryRepo;
    public CategoryService(
      IMapper mapper,
      IConfiguration config,
      UserManager<AppUser> userManager,
      RoleManager<AppRole> roleManager,
      ICurrentUserService currentUserService,
      ICategoryRepository categoryRepo) : base(mapper, config, currentUserService)
    {
        _categoryRepo = categoryRepo;
    }

    public async Task<IResult> CreateCategoryAsync(CategoryViewModel model)
    {
        if (await _categoryRepo.ExistsAsync(c => c.Name.ToLower() == model.Name.ToLower() && !c.IsDeleted))
            return new ErrorResult(Messages.AlreadyExists);

        var createResult = await _categoryRepo.CreateAsync(Mapper.Map<Category>(model));
        return createResult > 0
            ? new SuccessResult(Messages.CreateSuccess)
            : new ErrorResult(Messages.CreateError);
    }

    public async Task<IResult> UpdateCategoryAsync(CategoryViewModel model)
    {
        var existingCategory = await _categoryRepo.GetByIdAsync(model.Id);

        if (existingCategory == null || existingCategory.IsDeleted)
            return new ErrorResult(Messages.CategoryNotFound);

        var isNameTaken = await _categoryRepo.ExistsAsync(c =>
        c.Name.ToLower() == model.Name.ToLower() &&
        c.Id != model.Id && !c.IsDeleted);

        if (isNameTaken)
            return new ErrorResult(Messages.AlreadyExists);

        existingCategory.Name = model.Name;
        existingCategory.ImageUrl = model.ImageUrl;

        var updateResult = await _categoryRepo.UpdateAsync(existingCategory);

        return updateResult > 0
            ? new SuccessResult(Messages.UpdateSuccess)
            : new ErrorResult(Messages.UpdateError);
    }

    public async Task<IResult> DeleteCategoryAsync(Guid categoryId)
    {
        if (!await _categoryRepo.ExistsAsync(c => c.Id == categoryId && !c.IsDeleted))
            return new ErrorResult(Messages.CategoryNotFound);

        var deleteResult = await _categoryRepo.SoftDeleteAsync(categoryId);

        return deleteResult > 0
            ? new SuccessResult(Messages.DeleteSuccess)
            : new ErrorResult(Messages.DeleteError);
    }

    public async Task<IDataResult<IEnumerable<CategoryViewModel>>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepo.GetAllAsync();

        if (!categories.Any())
            return new ErrorDataResult<IEnumerable<CategoryViewModel>>(Messages.EmptyEntityList);

        var viewModels = Mapper.Map<IEnumerable<CategoryViewModel>>(categories);

        return new SuccessDataResult<IEnumerable<CategoryViewModel>>(viewModels);
    }

    public async Task<IDataResult<CategoryViewModel>> GetCategoryByIdAsync(Guid categoryId)
    {
        var category = await _categoryRepo.GetByIdAsync(categoryId);
        if (category == null)
            return new ErrorDataResult<CategoryViewModel>(Messages.CategoryNotFound);

        var viewModel = Mapper.Map<CategoryViewModel>(category);

        return new SuccessDataResult<CategoryViewModel>(viewModel);
    }
}
