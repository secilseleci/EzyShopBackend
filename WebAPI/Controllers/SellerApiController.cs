using Business.Services.Abstract;
using Core.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Seller;

namespace WebAPI.Controllers;

[Route("api/sellers")]
[ApiController]
[Authorize(Roles = CustomRoles.Admin)]
public class SellerApiController : ControllerBase
{
    private readonly ISellerService _sellerService;

    public SellerApiController(ISellerService sellerService)
    {
        _sellerService = sellerService;
    }

    #region Activate
    [HttpPost("activate-seller")]
    public async Task<IActionResult> ActivateSeller([FromForm] Guid sellerId)
    {
        var result = await _sellerService.ActivateSellerAsync(sellerId);

        if (!result.Success)
        {
            return BadRequest(new { success = false, message = result.Message });
        }
        return Ok(new { success = true, message = result.Message });
    }
    #endregion

    #region Deactivate
    [HttpPost("deactivate-seller")]
    public async Task<IActionResult> DeactivateSeller([FromForm] Guid sellerId)
    {
        var result = await _sellerService.DeactivateSellerAsync(sellerId);

        if (!result.Success)
        {
            return BadRequest(new { success = false, message = result.Message });
        }
        return Ok(new { success = true, message = result.Message });
    }
    #endregion

    #region Ban

    [HttpPost("ban-seller")]
    public async Task<IActionResult> BanSeller([FromForm] Guid sellerId)
    {
        var result = await _sellerService.BanSellerAsync(sellerId);

        if (!result.Success)
        {
            return BadRequest(new { success = false, message = result.Message });
        }
        return Ok(new { success = true, message = result.Message });
    }
    #endregion

    #region List Seller
    [HttpGet("list")]
    public async Task<IActionResult> GetSellers([FromQuery] SellerFilterDto filter)
    {
        var result = await _sellerService.GetFilteredSellerListAsync(filter);

        if (!result.Success)
            return NotFound(new { success = false, message = result.Message });

        return Ok(new
        {
            success = true,
            data = result.Data.Items,
            totalItems = result.Data.TotalItems,
            currentPage = result.Data.Page,
            pageSize = result.Data.PageSize
        });
    }

    #endregion
}
