using AutoMapper;
using Business.Services.Abstract;
using Core.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.SubscriptionPlan;
using WebAPI.Services;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/subscriptionPlan")]
public class SubscriptionPlanApiController : BaseApiController
{
    private readonly ISubscriptionPlanService _subscriptionPlanService;
    public SubscriptionPlanApiController(ISubscriptionPlanService subscriptionPlanService, IMapper mapper) : base(mapper)
    {
        _subscriptionPlanService = subscriptionPlanService;
    }

    [HttpGet]
    [Authorize]  
    public async Task<IActionResult> GetPlans()
    {
        var result = await _subscriptionPlanService.GetPlansAsync();
        return ApiResult(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> AddPlan([FromBody] CreateSubscriptionPlanDto model)
    {
        var result = await _subscriptionPlanService.CreatePlanAsync(model);
        return ApiResult(result);
    }
}
