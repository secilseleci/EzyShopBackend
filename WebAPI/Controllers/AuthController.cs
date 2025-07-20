using AutoMapper;
using Core.Constants;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Auth;
using Models.Identity;
using Models.ViewModels.Auth;

namespace WebAPI.Controllers;
[ApiController]
[Route("api/auth")]

public class AuthController : BaseApiController
{
    protected readonly UserManager<AppUser> UserManager;
    protected readonly SignInManager<AppUser> SignInManager;
    protected readonly RoleManager<AppRole> RoleManager;

    private readonly ITokenService _tokenService;
    public AuthController(
     UserManager<AppUser> userManager,
     SignInManager<AppUser> signInManager,
     RoleManager<AppRole> roleManager,
     IMapper mapper,
     ITokenService tokenService
    ) : base(mapper)
    {
        UserManager = userManager;
        SignInManager = signInManager;
        RoleManager = roleManager;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var user = await UserManager.FindByEmailAsync(model.Email);
        if (user == null||user.IsDeleted)  
            return Unauthorized(new { success = false, message = Messages.UserNotFound });

        var isPasswordValid = await UserManager.CheckPasswordAsync(user, model.Password);
        if (!isPasswordValid)
            return Unauthorized(new { success = false, message = Messages.LoginInvalidCredentials});

        var roles= await UserManager.GetRolesAsync(user);
        var token= _tokenService.CreateToken(user, roles);

        var response= new LoginResponseViewModel
        {
            Token = token,
            UserName = user.UserName!,
            Email = user.Email!,
            Roles = roles
        };

        return Ok(new { success = true, message = Messages.LoginSuccess, data = response });
    }
}

