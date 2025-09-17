using AutoMapper;
using Business.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.SubscriptionPlan;

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
    public async Task<IActionResult> GetAll()
    {
        var result = await _subscriptionPlanService.GetPlansAsync();
        return ApiResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await _subscriptionPlanService.GetPlanByIdAsync(id);
        return ApiResult(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateSubscriptionPlanDto model)
    {
        var result = await _subscriptionPlanService.CreatePlanAsync(model);
        return ApiResult(result);
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update([FromBody] SubscriptionPlanDto model)
    {
        var result = await _subscriptionPlanService.UpdatePlanAsync(model);
        return ApiResult(result);
    }

    [HttpPatch("{id}/deactivate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Deactivate([FromRoute] Guid id)
    {
        var result = await _subscriptionPlanService.DeactivatePlanAsync(id);
        return ApiResult(result);
    }

    [HttpPatch("{id}/activate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Activate([FromRoute] Guid id)
    {
        var result = await _subscriptionPlanService.ActivatePlanAsync(id);
        return ApiResult(result);
    }
}

