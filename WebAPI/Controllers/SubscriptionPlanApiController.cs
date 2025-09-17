using AutoMapper;
using Business.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.SubscriptionPlan;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/subscriptionPlan")]
[Authorize(Roles = "Admin")]
public class SubscriptionPlanApiController : BaseApiController
{
    private readonly ISubscriptionPlanService _subscriptionPlanService;
    public SubscriptionPlanApiController(ISubscriptionPlanService subscriptionPlanService, IMapper mapper) : base(mapper)
    {
        _subscriptionPlanService = subscriptionPlanService;
    }

    [HttpPost("plans")]
    public async Task<IActionResult> AddPlan([FromBody] CreateSubscriptionPlanDto model)
    {
        var result = await _subscriptionPlanService.CreatePlanAsync(model);
        return ApiResult(result);
    }
}
