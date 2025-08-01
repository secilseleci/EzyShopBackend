using Business.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/sellers")]
[ApiController]
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
}
