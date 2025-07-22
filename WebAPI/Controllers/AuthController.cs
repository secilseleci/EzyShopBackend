using AutoMapper;
using Business.Services.Abstract;
using Core.Constants;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
    private readonly ISellerService _sellerService;
    private readonly ICustomerService _customerService;
    protected readonly UserManager<AppUser> UserManager;
    protected readonly SignInManager<AppUser> SignInManager;
    protected readonly RoleManager<AppRole> RoleManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly ITokenService _tokenService;
    public AuthController(
     ISellerService sellerService,
     ICustomerService customerService,
     UserManager<AppUser> userManager,
     SignInManager<AppUser> signInManager,
     RoleManager<AppRole> roleManager,
     IMapper mapper,
     ITokenService tokenService,
     ICurrentUserService currentUserService
    ) : base(mapper)
    {
        UserManager = userManager;
        SignInManager = signInManager;
        RoleManager = roleManager;
        _tokenService = tokenService;
        _customerService = customerService;
        _currentUserService = currentUserService;
        _sellerService = sellerService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var user = await UserManager.FindByEmailAsync(model.Email);
        if (user == null || user.IsDeleted)
            return Unauthorized(new { success = false, message = Messages.UserNotFound });

        var isPasswordValid = await UserManager.CheckPasswordAsync(user, model.Password);
        if (!isPasswordValid)
            return Unauthorized(new { success = false, message = Messages.LoginInvalidCredentials });

        var roles = await UserManager.GetRolesAsync(user);
        var token = _tokenService.CreateToken(user, roles);

        var response = new LoginResponseViewModel
        {
            Token = token,
            UserName = user.UserName!,
            Email = user.Email!,
            Roles = roles
        };

        return Ok(new { success = true, message = Messages.LoginSuccess, data = response });
    }


    [HttpPost("register-customer")]
    public async Task<IActionResult> RegisterCustomer([FromBody] RegisterCustomerDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _customerService.RegisterCustomerAsync(model);

        return ApiResult(result);
    }

    [HttpPost("register-seller")]
    public async Task<IActionResult> RegisterSeller([FromBody] RegisterSellerDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _sellerService.CreateSellerApplicationAsync(model);

        return ApiResult(result);
    }
    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // 1. Kullanıcı kimliği alınır
        var userId = _currentUserService.UserId;
        if (userId == null)
            return Unauthorized();

        // 1. Kullanıcı bulunur
        var user = await UserManager.FindByIdAsync(userId.Value.ToString());
        if (user == null || user.IsDeleted)
            return NotFound(Messages.UserNotFound);

        // 3. Eski şifre doğrulanır
        var oldPasswordValid = await UserManager.CheckPasswordAsync(user, model.OldPassword);
        if (!oldPasswordValid)
            return BadRequest(Messages.OldPasswordError);

        // 4. Şifre değiştirme işlemi
        var result = await UserManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new { Errors = errors });
        }

        // 5. Şifre başarıyla değiştiyse yeni token üret:
        var roles = await UserManager.GetRolesAsync(user);
        var newToken = _tokenService.CreateToken(user, roles);

        return Ok(new
        {
            Message = Messages.PasswordChangeSuccess,
            NewToken = newToken
        });
    }
}

