using AutoMapper;
using Business.Services.Abstract;
using Core.Constants;
using Core.Interfaces;
using Core.Utilities.Results;
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
            return ApiResult(new ErrorDataResult<LoginResponseViewModel>(Messages.UserNotFound));

        var isPasswordValid = await UserManager.CheckPasswordAsync(user, model.Password);
        if (!isPasswordValid)
            return ApiResult(new ErrorDataResult<LoginResponseViewModel>(Messages.LoginInvalidCredentials));

        var roles = await UserManager.GetRolesAsync(user);
        var token = _tokenService.CreateToken(user, roles);

        var response = new LoginResponseViewModel
        {
            Token = token,
            UserName = user.UserName!,
            Email = user.Email!,
            Roles = roles
        };

        return ApiResult(new SuccessDataResult<LoginResponseViewModel>(response, Messages.LoginSuccess));
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
            return ApiResult(new ErrorResult("Invalid model."));

        var userId = _currentUserService.UserId;
        if (userId == null)
            return ApiResult(new ErrorResult(Messages.LoginUnauthorized));

        var user = await UserManager.FindByIdAsync(userId.Value.ToString());
        if (user == null || user.IsDeleted)
            return ApiResult(new ErrorResult(Messages.UserNotFound));

        var oldPasswordValid = await UserManager.CheckPasswordAsync(user, model.OldPassword);
        if (!oldPasswordValid)
            return ApiResult(new ErrorResult(Messages.OldPasswordError));

        var result = await UserManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        if (!result.Succeeded)
        {
            var errorMsg = string.Join(" | ", result.Errors.Select(e => e.Description));
            return ApiResult(new ErrorResult(errorMsg));
        }

        var roles = await UserManager.GetRolesAsync(user);
        var newToken = _tokenService.CreateToken(user, roles);

        return ApiResult(new SuccessDataResult<string>(newToken, Messages.PasswordChangeSuccess));
    }
}

