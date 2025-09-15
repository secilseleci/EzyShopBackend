using AutoMapper;
using Business.Services.Abstract;
using Core.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Seller;

namespace WebAPI.Controllers;

[Route("api/sellers")]
[ApiController]
public class SellerApiController : BaseApiController
{
    private readonly ISellerService _sellerService;

    public SellerApiController(ISellerService sellerService, IMapper mapper) : base(mapper)
    {
        _sellerService = sellerService;
    }

    #region Approve
    [Authorize(Roles = CustomRoles.Admin)]
    [HttpPost("approve-seller")]
    public async Task<IActionResult> ApproveSeller([FromForm] Guid sellerId)
    {
        var result = await _sellerService.ApproveSellerAsync(sellerId);

        return ApiResult(result);
    }
    #endregion

    #region Reject
    [Authorize(Roles = CustomRoles.Admin)]
    [HttpPost("reject-seller")]
    public async Task<IActionResult> RejectSeller([FromForm] Guid sellerId)
    {
        var result = await _sellerService.RejectSellerAsync(sellerId);

        return ApiResult(result);
    }
    #endregion

    #region Ban
    [Authorize(Roles = CustomRoles.Admin)]
    [HttpPost("ban-seller")]
    public async Task<IActionResult> BanSeller([FromForm] Guid sellerId)
    {
        var result = await _sellerService.BanSellerAsync(sellerId);

        return ApiResult(result);
    }
    #endregion

    #region List Seller
    [Authorize(Roles = CustomRoles.Admin)]
    [HttpGet("list")]
    public async Task<IActionResult> GetSellers([FromQuery] SellerFilterDto filter)
    {
        var result = await _sellerService.GetFilteredSellerListAsync(filter);

        return ApiResult(result);
    }
    #endregion

    #region Profile Seller
    [HttpGet("profile")]
    [Authorize(Roles = CustomRoles.Seller)]
    public async Task<IActionResult> GetProfile()
    {
        var result = await _sellerService.GetOwnProfileAsync();
        return ApiResult(result);
    }
    #endregion
}
