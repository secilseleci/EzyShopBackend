using AutoMapper;
using Business.Services.Abstract;
using Core.Constants;
using Core.Interfaces;
using Core.Utilities.Results;
using DataAccess.Repositories.Abstract;
using Microsoft.Extensions.Configuration;
using Models.DTOs.SubscriptionPlan;
using Models.Entities.Concrete;

namespace Business.Services.Concrete;

public class SubscriptionPlanService : BaseService, ISubscriptionPlanService
{
    private readonly ISubscriptionPlanRepository _subscriptionPlanRepo;
    public SubscriptionPlanService(
        ISubscriptionPlanRepository subscriptionPlanRepo,
        IMapper mapper,
        IConfiguration config,
        ICurrentUserService currentUserService) : base(mapper, config, currentUserService)
    { _subscriptionPlanRepo = subscriptionPlanRepo; }

    public async Task<IDataResult<CreateSubscriptionPlanDto>> CreatePlanAsync(CreateSubscriptionPlanDto model)
    {
        // Login check
        if (!CurrentUserService.UserId.HasValue)
            return new ErrorDataResult<CreateSubscriptionPlanDto>(Messages.LoginUnauthorized);

        // Role check
        if (CurrentUserService.Role != CustomRoles.Admin)
            return new ErrorDataResult<CreateSubscriptionPlanDto>(Messages.UnauthorizedAccess);

        // Duplicate check
        if (await _subscriptionPlanRepo.ExistsAsync(sp =>
        sp.Name.ToLower() == model.Name.ToLower()))
        {
            return new ErrorDataResult<CreateSubscriptionPlanDto>(Messages.AlreadyExists);
        }

        // Map DTO -> Entity
        var entity = Mapper.Map<SubscriptionPlan>(model);

        // Save Entity
        await _subscriptionPlanRepo.CreateAsync(entity);

        // Map Entity -> DTO
        var dto = Mapper.Map<CreateSubscriptionPlanDto>(entity);

        // Return Dto
        return new SuccessDataResult<CreateSubscriptionPlanDto>(dto, Messages.CreateSuccess);
    }
    public async Task<IDataResult<List<SubscriptionPlanDto>>> GetPlansAsync()
    {
        IEnumerable<SubscriptionPlan> plans;

        if (CurrentUserService.Role == CustomRoles.Admin)
        {
            // Admin  
            plans = await _subscriptionPlanRepo.GetAllAsync();
        }

        else
        {
            // Seller & Customer & Guest
            plans = await _subscriptionPlanRepo.GetWhereAsync(p => p.IsActive);
        }

        if (!plans.Any())
            return new ErrorDataResult<List<SubscriptionPlanDto>>(Messages.EmptyEntityList);

        var dtoList = Mapper.Map<List<SubscriptionPlanDto>>(plans.ToList());
        return new SuccessDataResult<List<SubscriptionPlanDto>>(dtoList);
    }
    public async Task<IDataResult<SubscriptionPlanDto>> GetPlanByIdAsync(Guid planId)
    {
        SubscriptionPlan? plan;

        if (CurrentUserService.Role == CustomRoles.Admin)
        {
            // Admin 
            plan = await _subscriptionPlanRepo.GetAsync(p => p.Id == planId);
        }
        else
        {
            // Seller & Customer & Guest
            plan = await _subscriptionPlanRepo.GetAsync(p => p.Id == planId && p.IsActive);
        }

        if (plan == null)
            return new ErrorDataResult<SubscriptionPlanDto>(Messages.SubscriptionPlanNotFound);

        var dto = Mapper.Map<SubscriptionPlanDto>(plan);
        return new SuccessDataResult<SubscriptionPlanDto>(dto);
    }
    public async Task<IDataResult<SubscriptionPlanDto>> UpdatePlanAsync(SubscriptionPlanDto model)
    {
        // Login check
        if (!CurrentUserService.UserId.HasValue)
            return new ErrorDataResult<SubscriptionPlanDto>(Messages.LoginUnauthorized);

        // Role check
        if (CurrentUserService.Role != CustomRoles.Admin)
            return new ErrorDataResult<SubscriptionPlanDto>(Messages.UnauthorizedAccess);

        // Category check
        var existingPlan = await _subscriptionPlanRepo.GetByIdAsync(model.Id);
        if (existingPlan == null || existingPlan.IsDeleted)
            return new ErrorDataResult<SubscriptionPlanDto>(Messages.SubscriptionPlanNotFound);

        // Duplicate check
        var isNameTaken = await _subscriptionPlanRepo.ExistsAsync(sp =>
            sp.Name.ToLower() == model.Name.ToLower() && sp.Id != model.Id);
        if (isNameTaken)
            return new ErrorDataResult<SubscriptionPlanDto>(Messages.AlreadyExists);

        // Update fields
        existingPlan.Name = model.Name;
        existingPlan.Price = model.Price;
        existingPlan.MaxProducts = model.MaxProducts;

        var updateResult = await _subscriptionPlanRepo.UpdateAsync(existingPlan);

        if (updateResult <= 0)
            return new ErrorDataResult<SubscriptionPlanDto>(Messages.UpdateError);

        // Map entity → dto
        var dto = Mapper.Map<SubscriptionPlanDto>(existingPlan);
        return new SuccessDataResult<SubscriptionPlanDto>(dto, Messages.UpdateSuccess);
    }
    public async Task<IResult> DeactivatePlanAsync(Guid planId)
    {
        // Login check
        if (!CurrentUserService.UserId.HasValue)
            return new ErrorResult(Messages.LoginUnauthorized);

        // Role check
        if (CurrentUserService.Role != CustomRoles.Admin)
            return new ErrorResult(Messages.UnauthorizedAccess);

        // Plan existing check
        var existingPlan = await _subscriptionPlanRepo.GetByIdAsync(planId);
        if (existingPlan == null || existingPlan.IsDeleted)
            return new ErrorResult(Messages.SubscriptionPlanNotFound);

        // Already deactive check
        if (!existingPlan.IsActive)
            return new ErrorResult(Messages.AlreadyDeactive);

        existingPlan.IsActive = false;
        var updateResult = await _subscriptionPlanRepo.UpdateAsync(existingPlan);

        return updateResult > 0
            ? new SuccessResult(Messages.UpdateSuccess)
            : new ErrorResult(Messages.UpdateError);
    }
    public async Task<IResult> ActivatePlanAsync(Guid planId)
    {
        // Login check
        if (!CurrentUserService.UserId.HasValue)
            return new ErrorResult(Messages.LoginUnauthorized);

        // Role check
        if (CurrentUserService.Role != CustomRoles.Admin)
            return new ErrorResult(Messages.UnauthorizedAccess);

        // Plan existing check
        var existingPlan = await _subscriptionPlanRepo.GetByIdAsync(planId);
        if (existingPlan == null || existingPlan.IsDeleted)
            return new ErrorResult(Messages.SubscriptionPlanNotFound);

        // Already active check
        if (existingPlan.IsActive)
            return new ErrorResult(Messages.AlreadyActive);

        existingPlan.IsActive = true;
        var updateResult = await _subscriptionPlanRepo.UpdateAsync(existingPlan);

        return updateResult > 0
            ? new SuccessResult(Messages.UpdateSuccess)
            : new ErrorResult(Messages.UpdateError);
    }
}


