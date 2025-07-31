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
    #region Approve
    [HttpPost("approve")]
    public async Task<IActionResult> ApproveSeller([FromForm] Guid sellerId)
    {
        var result = await _sellerService.ActivateSellerAsync(sellerId);

        if (!result.Success)
        {
            return BadRequest(new { success = false, message = result.Message });
        }
        return Ok(new { success = true, message = result.Message });
    }
#endregion
}
