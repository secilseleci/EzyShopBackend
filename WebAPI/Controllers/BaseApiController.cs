using Core.Constants;
using Core.Utilities.Results;
using Microsoft.AspNetCore.Mvc;
using IResult = Core.Utilities.Results.IResult;

namespace WebAPI.Controllers;

[ApiController]
public class BaseApiController : ControllerBase

{
    protected IActionResult ApiResult(IResult result)
    {
        if (result == null)
            return StatusCode(500, new { success = false, message = "Internal Server Error" });

        if (!result.Success)
        {
            if (result.Message == Messages.LoginUnauthorized)
                return Unauthorized(new { success = false, message = result.Message });

            return BadRequest(new { success = false, message = result.Message });
        }

        return Ok(new { success = true, message = result.Message });
    }

    protected IActionResult ApiResult<T>(IDataResult<T> result)
    {
        if (result == null)
            return StatusCode(500, new { success = false, message = "Internal Server Error" });

        if (!result.Success)
        {
            if (result.Message == Messages.LoginUnauthorized)
                return Unauthorized(new { success = false, message = result.Message });

            return BadRequest(new { success = false, message = result.Message });
        }

        return Ok(new { success = true, message = result.Message, data = result.Data });
    }
}
