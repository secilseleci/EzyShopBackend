using Core.Utilities.Results;
using Models.DTOs.SubscriptionPlan;

namespace Business.Services.Abstract;

    public interface ISubscriptionPlanService
    {
        Task<IDataResult<CreateSubscriptionPlanDto>> CreatePlanAsync(CreateSubscriptionPlanDto model);

        Task<IDataResult<CreateSubscriptionPlanDto>> UpdatePlanAsync(Guid planId, CreateSubscriptionPlanDto model);

        Task<IResult> DeactivatePlanAsync(Guid planId);

        Task<IDataResult<List<CreateSubscriptionPlanDto>>> GetPlansAsync(bool includePassive = false);

        Task<IDataResult<CreateSubscriptionPlanDto>> GetPlanByIdAsync(Guid id);
    }


