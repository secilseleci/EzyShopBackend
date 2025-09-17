using AutoMapper;
using Business.Services.Abstract;
using Core.Constants;
using Core.Interfaces;
using Core.Utilities.Results;
using DataAccess.Repositories.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Models.DTOs.Category;
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
        IEnumerable<Category> categories;

        if (CurrentUserService.Role == CustomRoles.Admin)
        {
            // Admin  
            categories = await _categoryRepo.GetAllAsync();
        }
        else
        {
            // Seller & Customer & Guest
            categories = await _categoryRepo.GetWhereAsync(c =>c.IsActive);
        }

        if (!categories.Any())
            return new ErrorDataResult<List<CategoryBasicDto>>(Messages.EmptyEntityList);

        var dtoList = Mapper.Map<List<CategoryBasicDto>>(categories.ToList());
        return new SuccessDataResult<List<CategoryBasicDto>>(dtoList);
    }

    public async Task<IDataResult<CategoryBasicDto>> GetCategoryByIdAsync(Guid categoryId)
    {
        Category? category;

        if (CurrentUserService.Role == CustomRoles.Admin)
        {
            // Admin 
            category = await _categoryRepo.GetAsync(c => c.Id == categoryId);
        }
        else
        {
            // Seller & Customer & Guest
            category = await _categoryRepo.GetAsync(c =>c.Id == categoryId && c.IsActive);
        }

        if (category == null)
            return new ErrorDataResult<CategoryBasicDto>(Messages.CategoryNotFound);

        var dto = Mapper.Map<CategoryBasicDto>(category);

        return new SuccessDataResult<CategoryBasicDto>(dto);
    }

    public async Task<IDataResult<CategoryBasicDto>> UpdateCategoryAsync(CategoryBasicDto model)
    {
        // Login check
        if (!CurrentUserService.UserId.HasValue)
            return new ErrorDataResult<CategoryBasicDto>(Messages.LoginUnauthorized);

        // Role check
        if (CurrentUserService.Role != CustomRoles.Admin)
            return new ErrorDataResult<CategoryBasicDto>(Messages.UnauthorizedAccess);

        // Category check
        var existingCategory = await _categoryRepo.GetByIdAsync(model.Id);
        if (existingCategory == null || existingCategory.IsDeleted)
            return new ErrorDataResult<CategoryBasicDto>(Messages.CategoryNotFound);

        // Duplicate check
        var isNameTaken = await _categoryRepo.ExistsAsync(c =>
            c.Name.ToLower() == model.Name.ToLower() && c.Id != model.Id);
        if (isNameTaken)
            return new ErrorDataResult<CategoryBasicDto>(Messages.AlreadyExists);

        // Update fields
        existingCategory.Name = model.Name;
        existingCategory.ImageUrl = model.ImageUrl;

        var updateResult = await _categoryRepo.UpdateAsync(existingCategory);

        if (updateResult <= 0)
            return new ErrorDataResult<CategoryBasicDto>(Messages.UpdateError);

        // Map entity → dto
        var dto = Mapper.Map<CategoryBasicDto>(existingCategory);
        return new SuccessDataResult<CategoryBasicDto>(dto, Messages.UpdateSuccess);
    }

    public async Task<IResult> DeactivateCategoryAsync(Guid categoryId)
    {
        // Login check
        if (!CurrentUserService.UserId.HasValue)
            return new ErrorResult(Messages.LoginUnauthorized);

        // Role check
        if (CurrentUserService.Role != CustomRoles.Admin)
            return new ErrorResult(Messages.UnauthorizedAccess);

        // Category existing check
        var existingCategory = await _categoryRepo.GetByIdAsync(categoryId);
        if (existingCategory == null || existingCategory.IsDeleted)
            return new ErrorResult(Messages.CategoryNotFound);

        // Already deactive check
        if (!existingCategory.IsActive)
            return new ErrorResult(Messages.AlreadyDeactive);

        existingCategory.IsActive = false;
        var updateResult = await _categoryRepo.UpdateAsync(existingCategory);

        return updateResult > 0
            ? new SuccessResult(Messages.UpdateSuccess)
            : new ErrorResult(Messages.UpdateError);
    }

    public async Task<IResult> ActivateCategoryAsync(Guid categoryId)
    {
        // Login check
        if (!CurrentUserService.UserId.HasValue)
            return new ErrorResult(Messages.LoginUnauthorized);

        // Role check
        if (CurrentUserService.Role != CustomRoles.Admin)
            return new ErrorResult(Messages.UnauthorizedAccess);

        // Category existing check
        var existingCategory = await _categoryRepo.GetByIdAsync(categoryId);
        if (existingCategory == null || existingCategory.IsDeleted)
            return new ErrorResult(Messages.CategoryNotFound);

        // Already active check
        if (existingCategory.IsActive)
            return new ErrorResult(Messages.AlreadyActive);

        existingCategory.IsActive = true;
        var updateResult = await _categoryRepo.UpdateAsync(existingCategory);

        return updateResult > 0
            ? new SuccessResult(Messages.UpdateSuccess)
            : new ErrorResult(Messages.UpdateError);
    }
}
