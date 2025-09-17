using AutoMapper;
using Business.Services.Abstract;
using Core.Constants;
using Core.Interfaces;
using Core.Utilities.Results;
using DataAccess.Repositories.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Models.DTOs.Category;
using Models.DTOs.SubscriptionPlan;
using Models.Entities.Concrete;
using Models.Identity;

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

    public async Task<IDataResult<CreateCategoryDto>> CreateCategoryAsync(CreateCategoryDto model)
    {
        // Login check
        if (!CurrentUserService.UserId.HasValue)
            return new ErrorDataResult<CreateCategoryDto>(Messages.LoginUnauthorized);

        // Role check
        if (CurrentUserService.Role != CustomRoles.Admin)
            return new ErrorDataResult<CreateCategoryDto>(Messages.UnauthorizedAccess);

        // Duplicate check
        if (await _categoryRepo.ExistsAsync(c => c.Name.ToLower() == model.Name.ToLower()))
            return new ErrorDataResult<CreateCategoryDto>(Messages.AlreadyExists);

        // DTO → Entity
        var entity = Mapper.Map<Category>(model);

        // Save
        var createResult = await _categoryRepo.CreateAsync(entity);
        if (createResult <= 0)
            return new ErrorDataResult<CreateCategoryDto>(Messages.CreateError);

        // Entity → DTO
        var dto = Mapper.Map<CreateCategoryDto>(entity);

        return new SuccessDataResult<CreateCategoryDto>(dto, Messages.CreateSuccess);
    }

    public async Task<IDataResult<List<CategoryBasicDto>>> GetCategoriesAsync()
    {
        // login check
        if (!CurrentUserService.UserId.HasValue)
            return new ErrorDataResult<List<CategoryBasicDto>>(Messages.LoginUnauthorized);

        IEnumerable<Category> categories;

        if (CurrentUserService.Role == CustomRoles.Admin)
        {
            // Admin  
            categories = await _categoryRepo.GetAllAsync();
        }
        else
        {
            // Seller & Customer
            categories = await _categoryRepo.GetWhereAsync(p => p.IsActive);
        }

        if (!categories.Any())
            return new ErrorDataResult<List<CategoryBasicDto>>(Messages.EmptyEntityList);

        var dtoList = Mapper.Map<List<CategoryBasicDto>>(categories.ToList());
        return new SuccessDataResult<List<CategoryBasicDto>>(dtoList);
    }

    public async Task<IResult> UpdateCategoryAsync(CategoryBasicDto model)
    {
        var existingCategory = await _categoryRepo.GetByIdAsync(model.Id);

        if (existingCategory == null || existingCategory.IsDeleted)
            return new ErrorResult(Messages.CategoryNotFound);

        var isNameTaken = await _categoryRepo.ExistsAsync(c =>
        c.Name.ToLower() == model.Name.ToLower() &&
        c.Id != model.Id);

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
        if (!await _categoryRepo.ExistsAsync(c => c.Id == categoryId))
            return new ErrorResult(Messages.CategoryNotFound);

        var deleteResult = await _categoryRepo.SoftDeleteAsync(categoryId);

        return deleteResult > 0
            ? new SuccessResult(Messages.DeleteSuccess)
            : new ErrorResult(Messages.DeleteError);
    }

    public async Task<IDataResult<CategoryBasicDto>> GetCategoryByIdAsync(Guid categoryId)
    {
        var category = await _categoryRepo.GetByIdAsync(categoryId);
        if (category == null)
            return new ErrorDataResult<CategoryBasicDto>(Messages.CategoryNotFound);

        var dto = Mapper.Map<CategoryBasicDto>(category);

        return new SuccessDataResult<CategoryBasicDto>(dto);
    }
}
