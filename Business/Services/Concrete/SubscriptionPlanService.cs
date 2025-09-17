using AutoMapper;
using Business.Services.Abstract;
using Core.Constants;
using Core.Interfaces;
using Core.Utilities.Results;
using DataAccess.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
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

    public Task<IResult> DeactivatePlanAsync(Guid planId)
    {
        throw new NotImplementedException();
    }

    public Task<IDataResult<CreateSubscriptionPlanDto>> GetPlanByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<IDataResult<List<SubscriptionPlanDto>>> GetPlansAsync()
    {
        // login check
        if (!CurrentUserService.UserId.HasValue)
            return new ErrorDataResult<List<SubscriptionPlanDto>>(Messages.LoginUnauthorized);

        IEnumerable<SubscriptionPlan> plans;

        if (CurrentUserService.Role == CustomRoles.Admin)
        {
            // Admin  
            plans = await _subscriptionPlanRepo.GetAllAsync();
        }
        else
        {
            // Seller  
            plans = await _subscriptionPlanRepo.GetWhereAsync(p => p.IsActive);
        }

        if (!plans.Any())
            return new ErrorDataResult<List<SubscriptionPlanDto>>(Messages.EmptyEntityList);

        var dtoList = Mapper.Map<List<SubscriptionPlanDto>>(plans.ToList());
        return new SuccessDataResult<List<SubscriptionPlanDto>>(dtoList);
    }


    public Task<IDataResult<CreateSubscriptionPlanDto>> UpdatePlanAsync(Guid planId, CreateSubscriptionPlanDto model)
    {
        throw new NotImplementedException();
    }
}


