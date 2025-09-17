using Core.Utilities.Results;
using Models.DTOs.SubscriptionPlan;

namespace Business.Services.Abstract;

public interface ISubscriptionPlanService
{
    Task<IDataResult<CreateSubscriptionPlanDto>> CreatePlanAsync(CreateSubscriptionPlanDto model);
    Task<IDataResult<List<SubscriptionPlanDto>>> GetPlansAsync();
    Task<IDataResult<SubscriptionPlanDto>> GetPlanByIdAsync(Guid planId);
    Task<IDataResult<SubscriptionPlanDto>> UpdatePlanAsync(SubscriptionPlanDto model);
    Task<IResult> DeactivatePlanAsync(Guid planId);
    Task<IResult> ActivatePlanAsync(Guid planId);
}


